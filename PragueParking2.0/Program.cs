using PragueParking2._0;
using PragueParking.DataAccess;
using Spectre.Console;

Console.Clear();

FileManager fileManager = new FileManager("../../../parkingdata.json", "../../../config.json");
Config config = fileManager.LoadConfig<Config>();
ParkingGarage garage = fileManager.LoadFromJson<ParkingGarage>();

if (garage.ParkingSpots == null || garage.ParkingSpots.Count == 0)
    garage = new ParkingGarage(config.NumberOfSpots);

AnsiConsole.MarkupLine($"[green]Prags Parkeringssystem Redo![/]");
AnsiConsole.MarkupLine($"[blue]{config.NumberOfSpots} platser, {config.VehicleTypes.Count} fordonstyper[/]\n");
Thread.Sleep(1000);

MenuManager menuManager = new MenuManager(garage);
menuManager.ShowMainMenu();

fileManager.SaveToJson(garage);
AnsiConsole.MarkupLine("\n[blue]Data sparad. \nTack för att du använder Prague Parking. \nHej då![/]");