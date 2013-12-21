using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator
{
    class Tank : Component
    {

        public double Capacity { get; private set; }
        public double Level { get; private set; }
        public double RemainingFuelAmount { get { return Capacity * Level; } }
        public double BaseMass { get; private set; }
        override public double Mass { get { return BaseMass + RemainingFuelAmount *0.8;} }
        public void Fill()
        {
            Level = 1;
        }
        public void Fill(double liters)
        {
            Level = Math.Min(1.0, Level + (liters / Capacity));
        }
        public void Consume(double liters)
        {
            Level = Math.Max(0.0, Level - (liters / Capacity));
        }
        public bool IsEmpty()
        {
            return Level <= 0.0;
        }
        public Tank(double mass, double capacity, bool full = true)
        {
            BaseMass = mass;
            Capacity = capacity;
            if (full) Fill();
        }
    }
}
