using PragueParking.DataAccess;
using Spectre.Console;
using System.Linq;

namespace PragueParking2._1
{
    public class MenuManager
    {
        private readonly ParkingGarage _garage;
        private readonly FileManager _fileManager;
        private Config _config;

        public MenuManager(ParkingGarage garage, FileManager fileManager, Config config)
        {
            _garage = garage;
            _fileManager = fileManager;
            _config = config;
        }

        public void ShowMainMenu()
        {
            while (true)
            {
                ShowHeader("PRAGUE PARKING SYSTEM V2.1");

                var stats = _garage.GetStatistics();
                int totalSpots = _garage.ParkingSpots.Count;
                int occupied = totalSpots - stats.empty;
                Console.WriteLine($"Status: {occupied} parkerade fordon | {stats.empty} tillgängliga platser\n");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .AddChoices(new[] {
                            "1. Checka in Fordon",
                            "2. Checka ut Fordon",
                            "3. Flytta Fordon",
                            "4. Sök Fordon",
                            "5. Visa Parkering",
                            "6. Ladda om Konfiguration",
                            "7. Avsluta"
                        }));

                string action = choice.Substring(0, 1);

                switch (action)
                {
                    case "1": CheckInVehicle(); break;
                    case "2": CheckOutVehicle(); break;
                    case "3": MoveVehicle(); break;
                    case "4": SearchVehicle(); break;
                    case "5": ViewParkingOverview(); break;
                    case "6": ReloadConfiguration(); break;
                    case "7":
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
                        "Cykel:       5 CZK/timme", 
                        "Buss:       80 CZK/timme", 
                        "Tillbaka till huvudmenyn" }));

            if (vehicleType == "Tillbaka till huvudmenyn")
                return;

            var regNumber = AskRegistrationNumberOrCancel();

            if (regNumber == "X")
                return;

            if (regNumber.Length > 10)
            {
                ShowError("Max 10 tecken.");
                WaitForKey();
                return;
            }

            IVehicle vehicle = vehicleType switch
            {
                "Bil:        20 CZK/timme" => new Car(regNumber),
                "Motorcykel: 10 CZK/timme" => new MC(regNumber),
                "Cykel:       5 CZK/timme" => new Bicycle(regNumber),
                "Buss:       80 CZK/timme" => new Bus(regNumber),
                _ => new Car(regNumber)
            };

            if (_garage.CheckInVehicle(vehicle))
            {
                var spot = _garage.FindVehicleSpot(regNumber);

                if (vehicle is Bus)
                {
                    int startSpot = spot.SpotNumber;
                    int endSpot = startSpot + 3;
                    ShowSuccess($"Incheckad på plats {startSpot}-{endSpot}");
                }
                else
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

            IVehicle vehicle = spot.FindVehicle(regNumber);
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

            var vehicle = currentSpot.FindVehicle(regNumber);

            if (vehicle is Bus)
            {
                int startSpot = currentSpot.SpotNumber;
                int endSpot = startSpot + 3;
                ShowSuccess($"För närvarande på plats {startSpot}-{endSpot}");
                AnsiConsole.MarkupLine("[yellow]Buss behöver 4 lediga platser i rad (1-47)[/]");
            }
            else
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
            {
                if (vehicle is Bus)
                {
                    int endSpot = targetSpot + 3;
                    ShowSuccess($"Flyttad till plats {targetSpot}-{endSpot}");
                }
                else
                    ShowSuccess($"Flyttad till plats {targetSpot}");
            }

            else
                ShowError("Kan inte flytta! Plats inte tillgänglig!");

            WaitForKey();
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
                    $"Typ: {vehicle.GetType().Name}");

                if (vehicle is Bus)
                {
                    int startSpot = spot.SpotNumber;
                    int endSpot = startSpot + 3;
                    ShowSuccess($"Plats: {startSpot}-{endSpot}");
                }
                else
                    ShowSuccess($"Plats: {spot.SpotNumber}");

                ShowSuccess($"Parkerad: {duration.Days}d {duration.Hours}h {duration.Minutes}m\n" +
                    $"Pris: {vehicle.GetHourlyRate()} CZK/hour\n");
            }

            else
                ShowError("Fordon hittades inte!");

            WaitForKey();
        }

        private void ViewParkingOverview()
        {
            ShowHeader("PARKERINGSÖVERSIKT");

            for (int i = 0; i < _garage.ParkingSpots.Count; i++)
            {
                var spot = _garage.ParkingSpots[i];
                int num = spot.SpotNumber;

                string status;
                ConsoleColor color;

                if (spot.IsEmpty())
                {
                    status = "TOM";
                    color = ConsoleColor.DarkRed;
                }
                
                else if (spot.ParkedVehicles.Any(v => v is Bus))
                {
                    status = "FULL";
                    color = ConsoleColor.DarkGreen;
                }
               
                else if (spot.GetAvailableSpace() == 0)
                {
                    status = "FULL";
                    color = ConsoleColor.DarkGreen;
                }
               
                else
                {
                    status = "DELVIS";
                    color = ConsoleColor.DarkYellow;
                }

                Console.ForegroundColor = color;
                Console.Write($"[{num,3}:{status,-6}] ");
                Console.ResetColor();

                if ((i + 1) % 10 == 0)
                    Console.WriteLine();
            }

            Console.WriteLine("\n");
            var stats = _garage.GetStatistics();
            Console.WriteLine($"Tom: {stats.empty}  |  Delvis: {stats.partial}  |  Full: {stats.full}");
            
            int totalSpots = _garage.ParkingSpots.Count;
            int occupied = totalSpots - stats.empty;
            Console.WriteLine($"Totalt upptaget: {occupied}/{totalSpots}\n");

            var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Vad vill du göra?")
                .AddChoices(new[] {
                    "Visa fordons lista",
                    "Tillbaka till huvudmenyn"
                }));

            if (choice == "Tillbaka till huvudmenyn")
                return;

            ShowVehicleList();
        }
        private void ShowVehicleList()
        {
            ShowHeader("PARKERADE FORDON");

            var allVehicles = new List<(int spotNumber, IVehicle vehicle)>();

            foreach (var spot in _garage.ParkingSpots)
            {
                if (!spot.IsEmpty())
                {
                    foreach (var vehicle in spot.ParkedVehicles)
                    {
                        allVehicles.Add((spot.SpotNumber, vehicle));
                    }
                }
            }

            if (allVehicles.Count == 0)
                Console.WriteLine("Inga fordon parkerade.\n");
            
            else
            {
                var uniqueVehicles = allVehicles
                    .GroupBy(v => v.vehicle.RegistrationNumber)
                    .Select(g => g.First())
                    .OrderBy(v => v.spotNumber)
                    .ToList();

                Console.WriteLine($"{"Regnr",-12} {"Typ",-12} {"Plats",-12} {"Storlek",-10} {"Incheckad",-20}");
                Console.WriteLine("────────────────────────────────────────────────────────────────────────");

                foreach (var item in uniqueVehicles)
                {
                    string vehicleType = item.vehicle.GetType().Name;
                    string spotDisplay;

                    if (item.vehicle is Bus)
                        spotDisplay = $"{item.spotNumber}-{item.spotNumber + 3}";
                    
                    else
                        spotDisplay = item.spotNumber.ToString();

                    Console.WriteLine($"{item.vehicle.RegistrationNumber,-12} " +
                        $"{vehicleType,-12} " +
                        $"{spotDisplay,-12} " +
                        $"{item.vehicle.GetSize(),-10} " +
                        $"{item.vehicle.EntryTime:yyyy-MM-dd HH:mm}");
                }
                Console.WriteLine("────────────────────────────────────────────────────────────────────────");
                Console.WriteLine($"\nTotalt antal fordon: {uniqueVehicles.Count}");
            }
            Console.WriteLine();
            WaitForKey();
        }


        private void ReloadConfiguration()
        {
            ShowHeader("LADDA OM KONFIGURATION");

            Config newConfig = _fileManager.LoadConfig<Config>();

            if (newConfig.NumberOfSpots < _garage.ParkingSpots.Count)
            {
                bool hasVehiclesInRemovedSpots = false;
                for (int i = newConfig.NumberOfSpots; i < _garage.ParkingSpots.Count; i++)
                {
                    if (!_garage.ParkingSpots[i].IsEmpty())
                    {
                        hasVehiclesInRemovedSpots = true;
                        break;
                    }
                }

                if (hasVehiclesInRemovedSpots)
                {
                    ShowError("Kan inte minska platser, en fordon har parkerats där!");
                    WaitForKey();
                    return;
                }

                _garage.ParkingSpots.RemoveRange(newConfig.NumberOfSpots,
                    _garage.ParkingSpots.Count - newConfig.NumberOfSpots);
            }
            else if (newConfig.NumberOfSpots > _garage.ParkingSpots.Count)
            {
                for (int i = _garage.ParkingSpots.Count; i < newConfig.NumberOfSpots; i++)
                {
                    _garage.ParkingSpots.Add(new ParkingSpot(i + 1));
                }
            }

            _config = newConfig;
            _fileManager.SaveToJson(_garage);

            ShowSuccess("Konfiguration omladdad!");
            ShowInfo($"Nya värden: {_config.NumberOfSpots} platser, {_config.VehicleTypes.Count} fordonstyper");

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

        private void ShowInfo(string message)
        {
            AnsiConsole.MarkupLine($"[blue]{message}[/]");
        }

        private string AskRegistrationNumberOrCancel()
        {
            Console.WriteLine("(Skriv 'X' för att gå tillbaka till huvudmenyn)\n");
            Console.Write("Ange registreringsnummer: ");
            return Console.ReadLine().ToUpper().Trim();
        }
    }
}