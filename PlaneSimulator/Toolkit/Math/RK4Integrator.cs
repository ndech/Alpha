using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator.Toolkit.Math
{
    class RK4Integrator<T> : Integrator<T> where T : IIntegrable<T>
    {
        T k1;
        T k2;
        T k3;
        T k4;
        derived integrationSystem;

        public RK4Integrator(derived _integrationSystem)
        {
            this.integrationSystem = _integrationSystem;
        }
        public T integrate(T current, double step)
        {
            // k1 = f(yn)
            k1 = integrationSystem(current);
            // k2 = f(yn + k1 * (h/2))
            k2 = integrationSystem(current.Add(k1.Times(step / 2)));
            // k3 = f(yn + k2 * (h/2))
            k3 = integrationSystem(current.Add(k2.Times(step / 2)));
            // k4 = f(yn + k3 * h)
            k4 = integrationSystem(current.Add(k3.Times(step)));
            // y(n+1) = yn + (k1 + 2*k2 + 2*k3 + k4) * (h/6)
            return current.Add((k1.Add(k2.Times(2).Add(k3.Times(2)).Add(k4))).Times(step / 6));
        }
    }
}
