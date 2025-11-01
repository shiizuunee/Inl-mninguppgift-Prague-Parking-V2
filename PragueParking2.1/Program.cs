using PragueParking2._1;
using PragueParking.DataAccess;
using Spectre.Console;

Console.Clear();

FileManager fileManager = new FileManager("../../../parkingdata.json", "../../../config.json");
Config config = fileManager.LoadConfig<Config>();
ParkingGarage garage = fileManager.LoadFromJson<ParkingGarage>();

if (garage.ParkingSpots == null || garage.ParkingSpots.Count == 0)
{
    garage = new ParkingGarage(config.NumberOfSpots);

    AnsiConsole.MarkupLine("[yellow]Genererar testdata...[/]");
    garage.CheckInVehicle(new Car("CAR001"));
    garage.CheckInVehicle(new MC("MC001"));
    garage.CheckInVehicle(new Bicycle("CYK001"));
    garage.CheckInVehicle(new Bus("BUS001"));

    fileManager.SaveToJson(garage);
    AnsiConsole.MarkupLine("[green]Testdata skapad![/]");
    Thread.Sleep(1000);
}

AnsiConsole.MarkupLine($"[green]Prags Parkeringssystem Redo![/]");
AnsiConsole.MarkupLine($"[blue]{config.NumberOfSpots} platser, {config.VehicleTypes.Count} fordonstyper[/]\n");
Thread.Sleep(1000);

MenuManager menuManager = new MenuManager(garage, fileManager, config);
menuManager.ShowMainMenu();

fileManager.SaveToJson(garage);
AnsiConsole.MarkupLine("\n[blue]Data sparad. \nTack för att du använder Prague Parking. \nHej då![/]");