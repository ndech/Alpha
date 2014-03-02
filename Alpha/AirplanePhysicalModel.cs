using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Toolkit.Math;

namespace Alpha
{
    public class AirplanePhysicalModel
    {
        private readonly Integrator<State> Integrator;
        public List<Thruster> Thrusters { get; private set; }
        public List<Tank> Tanks { get; private set; }
        private Airplane _airplane;
        private IAirplaneCommands _commands;
        public double Mass { get { return Tanks.Sum(tank => tank.Mass) + Thrusters.Sum(thruster => thruster.Mass); } }

        public State Update(double step, State currentState)
        {
            State tempState = Integrator.Integrate(currentState, step);
            DistributeFuelConsumption((ImmediateHourlyFuelConsumption() / 3600) * step);
            return tempState;
        }

        public AirplanePhysicalModel(Airplane airplane, IAirplaneCommands commands)
        {
            _commands = commands;
            _airplane = airplane;
            Tanks = new List<Tank>();
            Thrusters = new List<Thruster>();
            Integrator = new Rk4Integrator<State>(CalculateDerivedState);
        }

        public State CalculateDerivedState(State state)
        {
            return new State(state.Speed, CalculateForces(state) / Mass, state.AngularSpeed, CalculateMoments(state));
        }

        private Vector3D CalculateForces(State state)
        {
            Vector3D weight = new Vector3D(0.0, 0.0, _airplane.World.Gravity * Mass);
            Vector3D thrust = new Vector3D();
            thrust = Thrusters.Aggregate(thrust, (current, thruster) => current + thruster.Trust);
            thrust = new Vector3D(thrust.X * Math.Cos(state.AngularPosition.Y), thrust.X * Math.Sin(state.AngularPosition.Y), 0);
            return weight+thrust;
        }

        private Vector3D CalculateMoments(State state)
        {
            Vector3D vector = new Vector3D(0);
            vector.Z += 0.2f * (_commands.LeftAileron - _commands.RightAileron);
            vector.Y += 0.1f * (_commands.Elevator);
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
