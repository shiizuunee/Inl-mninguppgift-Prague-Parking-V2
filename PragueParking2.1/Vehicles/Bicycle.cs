using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._1
{
    public class Bicycle : Vehicle
    {
        public Bicycle() : base() { }

        public Bicycle(string registrationNumber) : base(registrationNumber) { }

        public override int GetSize()
        {
            return 1;
        }

        public override decimal GetHourlyRate()
        {
            return 5;
        }
    }
}
