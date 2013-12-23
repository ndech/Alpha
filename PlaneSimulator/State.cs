using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlaneSimulator.Toolkit.Math;

namespace PlaneSimulator
{
    class State : IIntegrable<State>
    {
        public Vector3 Position { get; set; }
        public Vector3 Speed { get; set; }
        public Vector3 Acceleration { get; set; }
        public Vector3 AngularPosition { get; set; }
        public Vector3 AngularSpeed { get; set; }
        public Vector3 AngularAcceleration { get; set; }
        public State(double altitude, double speed)
        {
            Position = new Vector3(0, 0, -altitude);
            Speed = new Vector3(speed, 0, 0);
            AngularPosition = new Vector3(0, 0, 0);
            Acceleration = new Vector3(0);
            AngularSpeed = new Vector3(0);
        }
        public State(Vector3 position, Vector3 speed, Vector3 angularPosition, Vector3 angularSpeed )
        {
            this.Position = position;
            this.Speed = speed;
            this.AngularPosition = angularPosition;
            this.AngularSpeed = angularSpeed;
        }

        public static State operator+(State s1, State s2)
        {
            return new State(s1.Position + s2.Position,
                s1.Speed + s2.Speed,
                s1.AngularPosition + s2.AngularPosition,
                s1.AngularSpeed + s2.AngularSpeed
                );
        }

        public static State operator *(State s1, double factor)
        {
            return new State(s1.Position * factor,
                s1.Speed * factor,
                s1.AngularPosition * factor,
                s1.AngularSpeed * factor
                );
        }

        public static State operator *(double factor, State s1)
        {
            return new State(s1.Position * factor,
                s1.Speed * factor,
                s1.AngularPosition * factor,
                s1.AngularSpeed * factor
                );
        }
    }
}
