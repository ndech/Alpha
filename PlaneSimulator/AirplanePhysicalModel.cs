using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using PlaneSimulator.Toolkit.Math;
using SharpDX.DirectInput;

namespace PlaneSimulator
{
    public class AirplanePhysicalModel
    {
        private readonly Integrator<State> Integrator;
        public List<Thruster> Thrusters { get; private set; }
        public List<Tank> Tanks { get; private set; }
        private Airplane _airplane;
        private Input _input;
        public double Mass { get { return Tanks.Sum(tank => tank.Mass) + Thrusters.Sum(thruster => thruster.Mass); } }

        public State Update(double step, State currentState)
        {
            State tempState = Integrator.Integrate(currentState, step);
            DistributeFuelConsumption((ImmediateHourlyFuelConsumption() / 3600) * step);
            return tempState;
        }

        public AirplanePhysicalModel(Airplane airplane, Input input)
        {
            _input = input;
            _airplane = airplane;
            Tanks = new List<Tank>();
            Thrusters = new List<Thruster>();
            Integrator = new Rk4Integrator<State>(CalculateDerivedState);
        }

        public State CalculateDerivedState(State state)
        {
            return new State(state.Speed, CalculateForces(state) / Mass, state.AngularSpeed, CalculateMoments(state));
        }

        private Vector3 CalculateForces(State state)
        {
            Vector3 weight = new Vector3(0.0, 0.0, _airplane.World.Gravity * Mass);
            Vector3 thrust = new Vector3();
            thrust = Thrusters.Aggregate(thrust, (current, thruster) => current + thruster.Trust);
            return weight + thrust;
        }

        private Vector3 CalculateMoments(State state)
        {
            Vector3 vector = new Vector3(0);
            if (_input == null)
                return vector;
            if (_input.IsKeyPressed(Key.Left))
                vector.Z += 0.1f;
            if (_input.IsKeyPressed(Key.Right))
                vector.Z -= 0.1f;
            if (_input.IsKeyPressed(Key.Up))
                vector.Y += 0.3f;
            if (_input.IsKeyPressed(Key.Down))
                vector.Y -= 0.3f;
            return vector;
        }

        private double ImmediateHourlyFuelConsumption()
        {
            return Thrusters.Sum(thruster => thruster.HourlyConsumption);
        }

        public void DistributeFuelConsumption(double liters)
        {
            foreach (Tank tank in Tanks.Where(tank => !tank.IsEmpty()))
            {
                tank.Consume(liters);
                return;
            }
        }
    }
}
