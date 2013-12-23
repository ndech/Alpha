using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator.Toolkit.Math
{
    internal class EulerIntegrator<T> : Integrator<T> where T : IIntegrable<T>
    {
        public EulerIntegrator(Derived integrationSystem) : base(integrationSystem)
        {
        }

        public override T Integrate(T current, double step)
        {
            throw new NotImplementedException();
        }
    }
}