using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._0
{
    public class ParkingSpot
    {
        public int SpotNumber { get; set; }
        public int Capacity { get; set; } = 4;
        public List<Vehicle> ParkedVehicles { get; set; }

        public ParkingSpot()
        {
            ParkedVehicles = new List<Vehicle>();
        }

        public ParkingSpot(int spotNumber) : this()
        {
            SpotNumber = spotNumber;
        }

        public int GetUsedSpace()
        {
            int total = 0;
            foreach (Vehicle vehicle in ParkedVehicles)
            {
                total += vehicle.GetSize();
            }
            return total;
        }

        public int GetAvailableSpace()
        {
            return Capacity - GetUsedSpace();
        }

        public bool CanFit(Vehicle vehicle)
        {
            return vehicle.GetSize() <= GetAvailableSpace();
        }

        public bool ParkVehicle(Vehicle vehicle)
        {
            if (CanFit(vehicle))
            {
                ParkedVehicles.Add(vehicle);
                return true;
            }
            return false;
        }

        public bool RemoveVehicle(string registrationNumber)
        {
            Vehicle vehicleToRemove = null;

            foreach (Vehicle vehicle in ParkedVehicles)
            {
                if (vehicle.RegistrationNumber == registrationNumber)
                {
                    vehicleToRemove = vehicle;
                    break;
                }
            }

            if (vehicleToRemove != null)
            {
                ParkedVehicles.Remove(vehicleToRemove);
                return true;
            }
            return false;
        }

        public Vehicle FindVehicle(string registrationNumber)
        {
            foreach (Vehicle vehicle in ParkedVehicles)
            {
                if (vehicle.RegistrationNumber == registrationNumber)
                {
                    return vehicle;
                }
            }
            return null;
        }
        public bool IsEmpty()
        {
            return ParkedVehicles.Count == 0;
        }
    }
}
