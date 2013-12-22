using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlaneSimulator.Toolkit.Math;
using System.Globalization;

namespace PlaneSimulator
{
    class Airplane
    {
        public World World { get; private set; }
        public State CurrentState { get; private set; }
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

        public Airplane(World world)
        {
            World = world;
            Tanks = new List<Tank>();
            Thrusters = new List<Thruster>();
        }
        public void Initialize(double altitude, double speed)
        {
            CurrentState = new State(altitude, speed);
        }

        public void Update(double step)
        {
            //Runge kutta integration of the state :
            State k1 = CalculateDerivedState(CurrentState);
            State k2 = CalculateDerivedState(CurrentState + (k1 * (step / 2)));
            State k3 = CalculateDerivedState(CurrentState + (k2 * (step / 2)));
            State k4 = CalculateDerivedState(CurrentState + (k3 * step));
            CurrentState += (k1 + 2 * k2 + 2 * k3 + k4) * (step / 6);
            //Fuel consumption management :
            DistributeFuelConsumption((ImmediateHourlyFuelConsumption() / 3600) * step);
        }

        public State CalculateDerivedState(State state)
        {
            return new State(state.Speed, CalculateForces(state) / Mass, state.AngularSpeed, CalculateMoments(state));
        }

        public Vector3 CalculateForces(State state)
        {
            Vector3 weight = new Vector3(0.0, 0.0, World.Gravity * Mass);
            Vector3 thrust = new Vector3(1000, 0, 0);
            return weight + thrust;
        }

        public Vector3 CalculateMoments(State state)
        {
            return new Vector3(0, 0, 0);
        }

        private double ImmediateHourlyFuelConsumption()
        {
            double hourlyConsumption = 0;
            foreach (Thruster thruster in Thrusters)
                hourlyConsumption += thruster.HourlyConsumption;
            return hourlyConsumption;
        }

        public void DistributeFuelConsumption(double liters)
        {
            foreach(Tank tank in Tanks)
                if(!tank.IsEmpty())
                {
                    tank.Consume(liters);
                    return;
                }
        }

        public bool IsCrashed()
        {
            return CurrentState.Position.Z > 0;
        }

        public override String ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "Position : ({0}, {1})\nAltitude : {2}\nSpeed : {3}", 
                CurrentState.Position.X, CurrentState.Position.Y, -CurrentState.Position.Z, CurrentState.Speed.Magnitude);
        }
    }
}
