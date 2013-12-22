using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlaneSimulator.Toolkit.Math;

namespace PlaneSimulator
{
    class State
    {
        public Airplane Airplane { get; private set; }
        public Vector3 Position { get; set; }
        public Vector3 Speed { get; set; }
        public Vector3 Acceleration { get; set; }
        public Vector3 AngularPosition { get; set; }
        public Vector3 AngularSpeed { get; set; }
        public Vector3 AngularAcceleration { get; set; }
        public State(Airplane airplane, double altitude, double speed)
        {
            Airplane = airplane;
            Position = new Vector3(0, 0, -altitude);
            Speed = new Vector3(speed, 0, 0);
            AngularPosition = new Vector3(0, 0, 0);
            Acceleration = new Vector3(0);
            AngularSpeed = new Vector3(0);
        }
    }
}
