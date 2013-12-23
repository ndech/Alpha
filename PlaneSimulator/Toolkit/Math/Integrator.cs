using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator.Toolkit.Math
{
    abstract class Integrator <T> where T : IIntegrable <T>
    {

        public delegate T derived(T current);
        public abstract T Integrate(T current, double step);
    }
}
