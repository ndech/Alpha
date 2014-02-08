using PlaneSimulator.Toolkit.Math;

namespace PlaneSimulator
{
    public class State : IIntegrable<State>
    {
        public Vector3D Position { get; set; }
        public Vector3D Speed { get; set; }
        public Vector3D Acceleration { get; set; }
        public Vector3D AngularPosition { get; set; }
        public Vector3D AngularSpeed { get; set; }
        public Vector3D AngularAcceleration { get; set; }

        public State(double altitude, double speed)
        {
            Position = new Vector3D(0, 0, -altitude);
            Speed = new Vector3D(speed, 0, 0);
            AngularPosition = new Vector3D(0, 0, 0);
            Acceleration = new Vector3D(0);
            AngularSpeed = new Vector3D(0);
        }
        public State(double x, double y, double altitude, double speedX, double speedY)
        {
            Position = new Vector3D(x, y, -altitude);
            Speed = new Vector3D(speedX, speedY, 0);
            AngularPosition = new Vector3D(0, 0, 0);
            Acceleration = new Vector3D(0);
            AngularSpeed = new Vector3D(0);
        }

        public State(Vector3D position, Vector3D speed, Vector3D angularPosition, Vector3D angularSpeed)
        {
            Position = position;
            Speed = speed;
            AngularPosition = angularPosition;
            AngularSpeed = angularSpeed;
        }

        public static State operator +(State s1, State s2)
        {
            return new State(s1.Position + s2.Position,
                s1.Speed + s2.Speed,
                s1.AngularPosition + s2.AngularPosition,
                s1.AngularSpeed + s2.AngularSpeed
                );
        }

        public static State operator *(State s1, double factor)
        {
            return new State(s1.Position*factor,
                s1.Speed*factor,
                s1.AngularPosition*factor,
                s1.AngularSpeed*factor
                );
        }

        public static State operator *(double factor, State s1)
        {
            return new State(s1.Position*factor,
                s1.Speed*factor,
                s1.AngularPosition*factor,
                s1.AngularSpeed*factor
                );
        }

        public State Times(double multiplicator)
        {
            return this*multiplicator;
        }

        public State Add(State other)
        {
            return this + other;
        }
    }
}