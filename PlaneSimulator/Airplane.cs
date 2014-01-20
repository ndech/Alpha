using System.Linq;
using PlaneSimulator.Toolkit.IO;
using SharpDX;
using SharpDX.Direct3D11;

namespace PlaneSimulator
{
    using System;
    using System.Collections.Generic;
    using Toolkit.Math;
    using System.Globalization;
    public class Airplane : RenderableGameComponent, ICsvLoggable
    {
        protected Integrator<State> Integrator;
        public World World { get; private set; }
        public State CurrentState { get; private set; }
        public List<Thruster> Thrusters { get; private set; }
        public List<Tank> Tanks { get; private set; }
        public float Altitude { get { return (float) -CurrentState.Position.Z; } }

        public double Mass
        {
            get
            {
                return Tanks.Sum(tank => tank.Mass) + Thrusters.Sum(thruster => thruster.Mass);
            }
        }

        public Airplane(World world)
        {
            World = world;
            Tanks = new List<Tank>();
            Thrusters = new List<Thruster>();
            Integrator = new Rk4Integrator<State>(CalculateDerivedState);
        }

        public void Initialize(double altitude, double speed)
        {
            CurrentState = new State(altitude, speed);
        }

        public override void Update(double step)
        {
            CurrentState = Integrator.Integrate(CurrentState, step);

            DistributeFuelConsumption((ImmediateHourlyFuelConsumption()/3600)*step);
        }

        public override void Dispose()
        {
            
        }

        public State CalculateDerivedState(State state)
        {
            return new State(state.Speed, CalculateForces(state)/Mass, state.AngularSpeed, CalculateMoments(state));
        }

        public Vector3 CalculateForces(State state)
        {
            Vector3 weight = new Vector3(0.0, 0.0, World.Gravity*Mass);
            Vector3 thrust = new Vector3();
            thrust = Thrusters.Aggregate(thrust, (current, thruster) => current + thruster.Trust);
            return weight + thrust;
        }

        public Vector3 CalculateMoments(State state)
        {
            return new Vector3(0, 0, 0);
        }

        private double ImmediateHourlyFuelConsumption()
        {
            return Thrusters.Sum(thruster => thruster.HourlyConsumption);
        }

        public void DistributeFuelConsumption(double liters)
        {
            foreach (Tank tank in Tanks)
            {
                if (!tank.IsEmpty())
                {
                    tank.Consume(liters);
                    return;
                }
            }
        }

        public bool IsCrashed()
        {
            return Altitude < 0;
        }

        public override String ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "Position : ({0}, {1})\nAltitude : {2}\nSpeed : {3}",
                CurrentState.Position.X, CurrentState.Position.Y, -CurrentState.Position.Z, CurrentState.Speed.Magnitude);
        }

        public override bool IsUi
        {
            get { return true; }
        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {

        }

        public string ToCsv()
        {
            return string.Format(CultureInfo.CurrentCulture,"{0};{1};{2};{3}",
                CurrentState.Position.X, CurrentState.Position.Y, -CurrentState.Position.Z, CurrentState.Speed.Magnitude);
        }
    }
}