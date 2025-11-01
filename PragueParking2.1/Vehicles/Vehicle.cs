using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PragueParking2._1
{
    public class Vehicle : IVehicle
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

