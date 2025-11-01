using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._1
{
    public class Car : Vehicle
    {
        public Car() : base() { }

        public Car(string registrationNumber) : base(registrationNumber) { }

        public override int GetSize()
        {
            return 4;
        }

        public override decimal GetHourlyRate()
        {
            return 20;
        }
    }
}