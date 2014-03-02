namespace PlaneSimulator.Toolkit.Math
{
    public abstract class Integrator<T> where T : IIntegrable<T>
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