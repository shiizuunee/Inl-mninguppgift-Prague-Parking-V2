using PragueParking2._1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._1
{
    public class ParkingSpot : IParkingSpot
    {
        public int SpotNumber { get; set; }
        public int Capacity { get; set; } = 4;

        private List<IVehicle> _parkedVehicles;
        public List<IVehicle> ParkedVehicles
        {
            get => _parkedVehicles;
            set => _parkedVehicles = value;
        }

        public ParkingSpot()
        {
            _parkedVehicles = new List<IVehicle>();
        }

        public ParkingSpot(int spotNumber) : this()
        {
            SpotNumber = spotNumber;
        }

        public int GetUsedSpace()
        {
            int total = 0;
            foreach (IVehicle vehicle in _parkedVehicles)
            {
                total += vehicle.GetSize();
            }
            return total;
        }

        public int GetAvailableSpace()
        {
            return Capacity - GetUsedSpace();
        }

        public bool CanFit(IVehicle vehicle)
        {
            return vehicle.GetSize() <= GetAvailableSpace();
        }

        public bool ParkVehicle(IVehicle vehicle)
        {
            if (CanFit(vehicle))
            {
                _parkedVehicles.Add(vehicle);
                return true;
            }
            return false;
        }

        public bool RemoveVehicle(string registrationNumber)
        {
            IVehicle vehicleToRemove = null;

            foreach (IVehicle vehicle in _parkedVehicles)
            {
                if (vehicle.RegistrationNumber == registrationNumber)
                {
                    vehicleToRemove = vehicle;
                    break;
                }
            }

            if (vehicleToRemove != null)
            {
                _parkedVehicles.Remove(vehicleToRemove);
                return true;
            }
            return false;
        }

        public IVehicle FindVehicle(string registrationNumber)
        {
            foreach (IVehicle vehicle in _parkedVehicles)
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
            return _parkedVehicles.Count == 0;
        }

        public bool ForceParkVehicle(IVehicle vehicle)
        {
            _parkedVehicles.Add(vehicle);
            return true;
        }
    }
}