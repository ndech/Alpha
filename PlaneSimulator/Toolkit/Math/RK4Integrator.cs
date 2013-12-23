namespace PlaneSimulator.Toolkit.Math
{
    internal class Rk4Integrator<T> : Integrator<T> where T : IIntegrable<T>
    {
        private T _k1, _k2, _k3, _k4;

        public Rk4Integrator(Derived integrationSystem) : base(integrationSystem)
        {
        }

        public override T Integrate(T current, double step)
        {
            // k1 = f(yn)
            _k1 = _integrationSystem(current);
            // k2 = f(yn + k1 * (h/2))
            _k2 = _integrationSystem(current.Add(_k1.Times(step/2)));
            // k3 = f(yn + k2 * (h/2))
            _k3 = _integrationSystem(current.Add(_k2.Times(step/2)));
            // k4 = f(yn + k3 * h)
            _k4 = _integrationSystem(current.Add(_k3.Times(step)));
            // y(n+1) = yn + (k1 + 2*k2 + 2*k3 + k4) * (h/6)
            return current.Add((_k1.Add(_k2.Times(2).Add(_k3.Times(2)).Add(_k4))).Times(step/6));
        }
    }
}