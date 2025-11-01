using System;
using System.Text.Json.Serialization;

namespace PragueParking2._1
{
    [JsonDerivedType(typeof(Vehicle), typeDiscriminator: "vehicle")]
    [JsonDerivedType(typeof(Car), typeDiscriminator: "car")]
    [JsonDerivedType(typeof(MC), typeDiscriminator: "mc")]
    [JsonDerivedType(typeof(Bicycle), typeDiscriminator: "bicycle")]
    [JsonDerivedType(typeof(Bus), typeDiscriminator: "bus")]
    public interface IVehicle
    {
        string RegistrationNumber { get; set; }
        DateTime EntryTime { get; set; }
        TimeSpan GetParkingDuration();
        int GetSize();
        decimal GetHourlyRate();
    }
}