using Spectre.Console;

namespace PragueParking2._0
{
    public class MenuManager
    {
        private readonly ParkingGarage _garage;

        public MenuManager(ParkingGarage garage)
        {
            _garage = garage;
        }

        public void ShowMainMenu()
        {
            while (true)
            {
                ShowHeader("PRAGUE PARKING SYSTEM V2.0");

                var stats = _garage.GetStatistics();
                Console.WriteLine($"Status: {100 - stats.empty} parkerade fordon | {stats.empty} tillgängliga platser\n");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .AddChoices(new[] {
                    "1. Checka in Fordon",
                    "2. Checka ut Fordon",
                    "3. Flytta Fordon",
                    "4. Sök Fordon",
                    "5. Visa Parkering",
                    "6. Avsluta"
                        }));

                string action = choice.Substring(0, 1);

                switch (action)
                {
                    case "1": CheckInVehicle(); break;
                    case "2": CheckOutVehicle(); break;
                    case "3": MoveVehicle(); break;
                    case "4": SearchVehicle(); break;
                    case "5": ViewParkingOverview(); break;
                    case "6":
                        if (AnsiConsole.Confirm("Avsluta programmet?"))
                            return;
                        break;
                }
            }
        }

        private void CheckInVehicle()
        {
            ShowHeader("CHECKA IN FORDON");

            var vehicleType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Välj fordonstyp:")
                    .AddChoices(new[] { "Bil:        20 CZK/timme", 
                        "Motorcykel: 10 CZK/timme",
                        "Tillbaka till huvudmenyn"}));
            if (vehicleType == "Tillbaka till huvudmenyn")
            {
                return;
            }

            var regNumber = AskRegistrationNumberOrCancel();

            if (regNumber == "X")
                return;

            if (regNumber.Length > 10)
            {
                ShowError("Max 10 tecken.");
                WaitForKey();
                return;
            }

            Vehicle vehicle = vehicleType == "Bil:        20 CZK/timme" ? new Car(regNumber) : new MC(regNumber);

            if (_garage.CheckInVehicle(vehicle))
            {
                var spot = _garage.FindVehicleSpot(regNumber);
                ShowSuccess($"Incheckad på plats {spot.SpotNumber}");
                AnsiConsole.MarkupLine("[dim]De första 10 minuterna är gratis![/]");
            }

            else
            {
                if (_garage.VehicleExists(regNumber))
                    ShowError("Redan parkerad!");
                else
                    ShowError("Ingen parkeringsplats tillgänglig!");
            }

            WaitForKey();
        }

        private void CheckOutVehicle()
        {
            ShowHeader("CHECKA UT FORDON");

            var regNumber = AskRegistrationNumberOrCancel();

            if (regNumber == "X")
                return;

            var spot = _garage.FindVehicleSpot(regNumber);

            if (spot == null)
            {
                ShowError("Fordonet hittades inte!");
                WaitForKey();
                return;
            }

            Vehicle vehicle = spot.FindVehicle(regNumber);
            TimeSpan duration = vehicle.GetParkingDuration();
            int totalMinutes = (int)Math.Ceiling(duration.TotalMinutes);
            int chargeableMinutes = Math.Max(0, totalMinutes - 10);
            int hours = (int)Math.Ceiling(chargeableMinutes / 60.0);
            decimal cost = hours * vehicle.GetHourlyRate();

            Console.WriteLine();
            ShowSuccess($"Registreringsnummer: {vehicle.RegistrationNumber}\n" +
                $"Parkerad: {duration.Days}d {duration.Hours}h {duration.Minutes}m\n" +
                $"Kostnad: {cost} CZK");

            Console.WriteLine();

            bool confirm = AnsiConsole.Confirm("Fortsätt med utcheckning?");

            if (!confirm)
            {
                AnsiConsole.MarkupLine("\n[yellow]Utcheckning avbruten.[/]");
                WaitForKey();
                return;
            }

            _garage.CheckOutVehicle(regNumber);

            ShowSuccess("\nUtcheckad!");

            WaitForKey();
        }

        private void MoveVehicle()
        {
            ShowHeader("FLYTTA FORDON");

            var regNumber = AskRegistrationNumberOrCancel();

            if (regNumber == "X")
                return;

            var currentSpot = _garage.FindVehicleSpot(regNumber);

            if (currentSpot == null)
            {
                ShowError("Fordon hittades inte!");
                WaitForKey();
                return;
            }

            ShowSuccess($"För närvarande på plats {currentSpot.SpotNumber}");
            var targetSpot = AnsiConsole.Ask<int>("Ange målplats (1-100): ");

            if (targetSpot == currentSpot.SpotNumber)
            {
                ShowError("Fordonet är redan på denna plats! Välj en annan plats.");
                WaitForKey();
                return;
            }

            if (targetSpot < 1 || targetSpot > 100)
                ShowError("Ogiltigt platsnummer!");

            else if (_garage.MoveVehicle(regNumber, targetSpot))
                ShowSuccess($"Flyttad till plats {targetSpot}");

            else
                ShowError("Kan inte flytta! Plats inte tillgänglig!");

            WaitForKey(); ;
        }

        private void SearchVehicle()
        {
            ShowHeader("SÖK FORDON");

            var regNumber = AskRegistrationNumberOrCancel();

            if (regNumber == "X")
                return;

            var spot = _garage.FindVehicleSpot(regNumber);

            if (spot != null)
            {
                var vehicle = spot.FindVehicle(regNumber);
                TimeSpan duration = vehicle.GetParkingDuration();
                
                ShowSuccess($"Fordon hittades!\n" +
                     $"Registreringsnummer: {vehicle.RegistrationNumber}\n" +
                     $"Typ: {vehicle.GetType().Name}\n" +
                     $"Plats: {spot.SpotNumber}\n" +
                     $"Parkerad: {duration.Days}d {duration.Hours}h {duration.Minutes}m\n" +
                     $"Pris: {vehicle.GetHourlyRate()} CZK/hour\n");
            }

            else
                ShowError("Fordon hittades inte!");

            WaitForKey();
        }

        private void ViewParkingOverview()
        {
            ShowHeader("PARKERINGSÖVERSIKT");

            int count = 0;
            for (int i = 0; i < _garage.ParkingSpots.Count; i++)
            {
                var spot = _garage.ParkingSpots[i];
                if (!spot.IsEmpty())
                {
                    foreach (var vehicle in spot.ParkedVehicles)
                    {
                        Console.WriteLine($"Plats {spot.SpotNumber}: {vehicle.RegistrationNumber}");
                        count++;
                    }
                }
            }

            Console.WriteLine($"\nTotalt: {count} fordon parkerade");

            WaitForKey();
        }

        private void ShowHeader(string title)
        {
            Console.Clear();
            AnsiConsole.Write(
                new Rule($"[bold yellow]{title}[/]")
                    .RuleStyle("grey")
                    .Centered());
            AnsiConsole.WriteLine();
        }

        private void WaitForKey()
        {
            Console.Write("\nTryck valfri tangent för att fortsätta... ");
            Console.ReadKey(true);
        }

        private void ShowSuccess(string message)
        {
            AnsiConsole.MarkupLine($"[green]{message}[/]");
        }

        private void ShowError(string message)
        {
            AnsiConsole.MarkupLine($"[red]OBS! {message}[/]");
        }

        private string AskRegistrationNumberOrCancel()
        {
            Console.WriteLine("(Skriv 'X' för att gå tillbaka till huvudmenyn)\n");
            Console.Write("Ange registreringsnummer: ");
            return Console.ReadLine().ToUpper().Trim();
        }
    }
}

