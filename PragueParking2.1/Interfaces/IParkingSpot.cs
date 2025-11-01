using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._1
{
    public interface IParkingSpot
    {
        int SpotNumber { get; set; }
        int Capacity { get; set; }
        List<IVehicle> ParkedVehicles { get; }

        int GetUsedSpace();
        int GetAvailableSpace();
        bool CanFit(IVehicle vehicle);
        bool ParkVehicle(IVehicle vehicle);
        bool RemoveVehicle(string registrationNumber);
        IVehicle FindVehicle(string registrationNumber);
        bool IsEmpty();
    }
}
