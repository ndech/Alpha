using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator
{
    public class Thruster : Component
    {
        public override double Mass { get { return 1000; } }
        public double HourlyConsumption { get { return 100.0; } }
    }
}
