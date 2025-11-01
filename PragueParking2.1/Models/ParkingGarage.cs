namespace PragueParking2._1
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
                ParkingSpot spot = new ParkingSpot(i + 1);
                spot.Capacity = 4;
                ParkingSpots.Add(spot);
            }
        }

        public IParkingSpot FindAvailableSpot(IVehicle vehicle)
        {
            int maxSpot = vehicle is Bus ? Math.Min(50, ParkingSpots.Count) : ParkingSpots.Count;

            for (int i = 0; i < maxSpot; i++)
            {
                if (ParkingSpots[i].CanFit(vehicle))
                {
                    return ParkingSpots[i];
                }
            }
            return null;
        }

        public bool CheckInVehicle(IVehicle vehicle)
        {
            if (VehicleExists(vehicle.RegistrationNumber))
            {
                return false;
            }

            if (vehicle is Bus)
            {
                return CheckInBus(vehicle);
            }
            else
            {
                IParkingSpot availableSpot = FindAvailableSpot(vehicle);
                if (availableSpot != null)
                {
                    return availableSpot.ParkVehicle(vehicle);
                }
                return false;
            }
        }

        private bool CheckInBus(IVehicle bus)
        {
            for (int i = 0; i <= 46; i++)
            {
                if (ParkingSpots[i].IsEmpty() &&
                    ParkingSpots[i + 1].IsEmpty() &&
                    ParkingSpots[i + 2].IsEmpty() &&
                    ParkingSpots[i + 3].IsEmpty())
                {
                    ((ParkingSpot)ParkingSpots[i]).ForceParkVehicle(bus);
                    ((ParkingSpot)ParkingSpots[i + 1]).ForceParkVehicle(bus);
                    ((ParkingSpot)ParkingSpots[i + 2]).ForceParkVehicle(bus);
                    ((ParkingSpot)ParkingSpots[i + 3]).ForceParkVehicle(bus);
                    return true;
                }
            }
            return false;
        }

        public IVehicle CheckOutVehicle(string registrationNumber)
        {
            IVehicle vehicle = null;
            List<IParkingSpot> spotsWithVehicle = new List<IParkingSpot>();

            foreach (IParkingSpot spot in ParkingSpots)
            {
                IVehicle found = spot.FindVehicle(registrationNumber);
                if (found != null)
                {
                    vehicle = found;
                    spotsWithVehicle.Add(spot);
                }
            }

            if (vehicle != null)
            {
                foreach (IParkingSpot spot in spotsWithVehicle)
                {
                    spot.RemoveVehicle(registrationNumber);
                }
            }

            return vehicle;
        }

        public bool MoveVehicle(string registrationNumber, int targetSpotNumber)
        {
            IVehicle vehicle = null;
            List<IParkingSpot> currentSpots = new List<IParkingSpot>();

            foreach (IParkingSpot spot in ParkingSpots)
            {
                IVehicle found = spot.FindVehicle(registrationNumber);
                if (found != null)
                {
                    vehicle = found;
                    currentSpots.Add(spot);
                }
            }

            if (vehicle == null)
            {
                return false;
            }

            if (vehicle is Bus)
            {
                return MoveBus(registrationNumber, targetSpotNumber, currentSpots);
            }

            IParkingSpot targetSpot = ParkingSpots.Find(s => s.SpotNumber == targetSpotNumber);
            if (targetSpot == null || !targetSpot.CanFit(vehicle))
            {
                return false;
            }
            currentSpots[0].RemoveVehicle(registrationNumber);

            targetSpot.ParkVehicle(vehicle);
            return true;
        }

        private bool MoveBus(string registrationNumber, int targetStartSpot, List<IParkingSpot> currentSpots)
        {
            if (targetStartSpot < 1 || targetStartSpot > 47)
            {
                return false;
            }

            int targetIndex = targetStartSpot - 1;

            for (int i = 0; i < 4; i++)
            {
                if (targetIndex + i >= ParkingSpots.Count)
                {
                    return false;
                }

                IParkingSpot spot = ParkingSpots[targetIndex + i];

                bool isCurrentSpot = currentSpots.Any(cs => cs.SpotNumber == spot.SpotNumber);

                if (!isCurrentSpot && !spot.IsEmpty())
                {
                    return false;
                }
            }

            IVehicle bus = currentSpots[0].FindVehicle(registrationNumber);

            foreach (IParkingSpot spot in currentSpots)
            {
                spot.RemoveVehicle(registrationNumber);
            }

            for (int i = 0; i < 4; i++)
            {
                ((ParkingSpot)ParkingSpots[targetIndex + i]).ForceParkVehicle(bus);
            }

            return true;
        }

        public IParkingSpot FindVehicleSpot(string registrationNumber)
        {
            foreach (IParkingSpot spot in ParkingSpots)
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

        public (int empty, int partial, int full) GetStatistics()
        {
            int empty = 0;
            int partial = 0;
            int full = 0;

            foreach (IParkingSpot spot in ParkingSpots)
            {
                if (spot.IsEmpty())
                {
                    empty++;
                }
                else if (spot.GetAvailableSpace() == 0 || spot.ParkedVehicles.Any(v => v is Bus))
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