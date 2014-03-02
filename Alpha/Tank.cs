namespace Alpha
{
    using System;

    public class Tank : Component
    {
        public Tank(double mass, double capacity, bool full = true)
        {
            this.BaseMass = mass;
            Capacity = capacity;
            if (full) Fill();
        }

        public double Capacity { get; private set; }

        public double Level { get; private set; }

        public double RemainingFuelAmount
        {
            get { return this.Capacity*this.Level; }
        }

        public double BaseMass { get; private set; }

        public override double Mass
        {
            get { return BaseMass + RemainingFuelAmount*0.8; }
        }

        public void Fill()
        {
            Level = 1;
        }

        public void Fill(double liters)
        {
            this.Level = Math.Min(1.0, this.Level + (liters/this.Capacity));
        }

        public void Consume(double liters)
        {
            this.Level = Math.Max(0.0, this.Level - (liters/this.Capacity));
        }

        public bool IsEmpty()
        {
            return this.Level <= 0.0;
        }
    }
}