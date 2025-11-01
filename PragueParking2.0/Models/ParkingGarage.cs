namespace PragueParking2._0
{
    public class ParkingGarage
    {
        public List<ParkingSpot> ParkingSpots { get; set; }

        public ParkingGarage()
        {
            ParkingSpots = new List<ParkingSpot>();
        }

        public ParkingGarage(int numberOfSpots) : this()
        {
            for (int i = 0; i < numberOfSpots; i++)
            {
                ParkingSpots.Add(new ParkingSpot(i + 1));
            }
        }

        public ParkingSpot FindVehicleSpot(string registrationNumber)
        {
            foreach (ParkingSpot spot in ParkingSpots)
            {
                if (spot.FindVehicle(registrationNumber) != null)
                {
                    return spot;
                }
            }
            return null;
        }

        public bool VehicleExists(string registrationNumber)
        {
            return FindVehicleSpot(registrationNumber) != null;
        }

        public ParkingSpot FindAvailableSpot(Vehicle vehicle)
        {
            foreach (ParkingSpot spot in ParkingSpots)
            {
                if (spot.CanFit(vehicle))
                {
                    return spot;
                }
            }
            return null;
        }

        public bool CheckInVehicle(Vehicle vehicle)
        {
            if (VehicleExists(vehicle.RegistrationNumber))
            {
                return false;
            }

            ParkingSpot availableSpot = FindAvailableSpot(vehicle);

            if (availableSpot != null)
            {
                return availableSpot.ParkVehicle(vehicle);
            }

            return false;
        }
        public Vehicle CheckOutVehicle(string registrationNumber)
        {
            ParkingSpot spot = FindVehicleSpot(registrationNumber);

            if (spot != null)
            {
                Vehicle vehicle = spot.FindVehicle(registrationNumber);
                spot.RemoveVehicle(registrationNumber);
                return vehicle;
            }

            return null;
        }

        public bool MoveVehicle(string registrationNumber, int targetSpotNumber)
        {
            ParkingSpot currentSpot = FindVehicleSpot(registrationNumber);
            if (currentSpot == null)
            {
                return false;
            }

            ParkingSpot targetSpot = ParkingSpots.Find(s => s.SpotNumber == targetSpotNumber);
            if (targetSpot == null)
            {
                return false;
            }

            Vehicle vehicle = currentSpot.FindVehicle(registrationNumber);

            if (!targetSpot.CanFit(vehicle))
            {
                return false;
            }

            currentSpot.RemoveVehicle(registrationNumber);
            targetSpot.ParkVehicle(vehicle);

            return true;
        }
        public (int empty, int partial, int full) GetStatistics()
        {
            int empty = 0;
            int partial = 0;
            int full = 0;

            foreach (ParkingSpot spot in ParkingSpots)
            {
                if (spot.IsEmpty())
                {
                    empty++;
                }
                else if (spot.GetAvailableSpace() == 0)
                {
                    full++;
                }
                else
                {
                    partial++;
                }
            }

            return (empty, partial, full);
        }
    }
}