using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._1
{
    public class Bus : Vehicle
    {
        public Bus() : base() { }

        public Bus(string registrationNumber) : base(registrationNumber) { }

        public override int GetSize()
        {
            return 16; 
        }

        public override decimal GetHourlyRate()
        {
            return 80; 
        }
    }
}
