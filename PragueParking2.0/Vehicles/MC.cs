using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._0
{
    public class MC : Vehicle
    {
        public MC() : base() { }

        public MC(string registrationNumber) : base(registrationNumber) { }

        public override int GetSize()
        {
            return 2;
        }

        public override decimal GetHourlyRate()
        {
            return 10;
        }
    }
}