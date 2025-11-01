using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace PragueParking2._0
{
    [JsonDerivedType(typeof(Vehicle), typeDiscriminator: "vehicle")]
    [JsonDerivedType(typeof(Car), typeDiscriminator: "car")]
    [JsonDerivedType(typeof(MC), typeDiscriminator: "mc")]
    public class Vehicle
    {
        public string RegistrationNumber { get; set; }
        public DateTime EntryTime { get; set; }

        public Vehicle() { }

        public Vehicle(string registrationNumber)
        {
            RegistrationNumber = registrationNumber;
            EntryTime = DateTime.Now;
        }

        public TimeSpan GetParkingDuration()
        {
            return DateTime.Now - EntryTime;
        }

        public virtual int GetSize()
        {
            return 0;
        }

        public virtual decimal GetHourlyRate()
        {
            return 0;
        }
    }
}

