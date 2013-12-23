using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator.Toolkit.Math
{
    internal abstract class Integrator<T> where T : IIntegrable<T>
    {
        protected readonly Derived _integrationSystem;

        protected Integrator(Derived integrationSystem)
        {
            this._integrationSystem = integrationSystem;
        }

        public delegate T Derived(T current);

        public abstract T Integrate(T current, double step);
    }
}