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
        public Vector3 Position {get; set;}
        public Vector3 Speed { get; set; }
        public Vector3 Acceleration { get; set; }
        public Vector3 AngularPosition { get; set; }
        public Vector3 AngularSpeed { get; set; }
        public Vector3 AngularAcceleration { get; set; }
    }
}
