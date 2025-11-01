using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._1
{
    public class Config
    {
        public int NumberOfSpots { get; set; } = 100;
        public Dictionary<string, VehicleConfig> VehicleTypes { get; set; }

        public Config()
        {
            VehicleTypes = new Dictionary<string, VehicleConfig>
            {
                { "Car", new VehicleConfig { Size = 4, HourlyRate = 20 } },
                { "MC", new VehicleConfig { Size = 2, HourlyRate = 10 } },
                { "Bicycle", new VehicleConfig { Size = 1, HourlyRate = 5 } },
                { "Bus", new VehicleConfig { Size = 16, HourlyRate = 80 } }
            };
        }
    }

    public class VehicleConfig
    {
        public int Size { get; set; }
        public decimal HourlyRate { get; set; }
    }
}