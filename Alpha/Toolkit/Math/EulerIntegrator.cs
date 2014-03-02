using System;

namespace Alpha.Toolkit.Math
{
    internal class EulerIntegrator<T> : Integrator<T> where T : IIntegrable<T>
    {
        private T _k1;
        public EulerIntegrator(Derived integrationSystem) : base(integrationSystem)
        {
        }

        public override T Integrate(T current, double step)
        {
            // k1 = f(yn)
            _k1 = _integrationSystem(current);
            // y(n+1) = yn + (k1 * h)
            return current.Add(_k1.Times(step));
        }
    }
}