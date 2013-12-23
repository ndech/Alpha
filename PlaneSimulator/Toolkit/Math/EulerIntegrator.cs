using System;

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