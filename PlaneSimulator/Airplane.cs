using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlaneSimulator.Toolkit.Math;

namespace PlaneSimulator
{
    class Airplane
    {
        public Vector3 Position { get; private set; }
        public Vector3 Speed { get; private set; }
        public List<Thruster> Thrusters { get; private set; }
        public List<Tank> Tanks { get; private set; }

        public double Mass
        {
            get
            {
                double mass = 0;
                foreach (Tank tank in Tanks)
                    mass += tank.Mass;
                foreach (Thruster thruster in Thrusters)
                    mass += thruster.Mass;
                return mass;
            }
        }

        public Airplane()
        {
            Tanks = new List<Tank>();
            Thrusters = new List<Thruster>();
        }
        public void Initialize(double altitude, double speed)
        {
            Position = new Vector3(0, 0, -altitude);
            Speed = new Vector3(speed, 0, 0);
        }

        public void Update(double step)
        {
            //Fuel consumption management :
            distributeFuelConsumption((ImmediateHourlyFuelConsumption() / 3600) * step);
        }

        private double ImmediateHourlyFuelConsumption()
        {
            double hourlyConsumption = 0;
            foreach (Thruster thruster in Thrusters)
                hourlyConsumption += thruster.HourlyConsumption;
            return hourlyConsumption;
        }
        public void distributeFuelConsumption(double liters)
        {
            foreach(Tank tank in Tanks)
                if(!tank.IsEmpty())
                {
                    tank.Consume(liters);
                    return;
                }
        }
    }
}
