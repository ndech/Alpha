using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha
{
    class ComputerAirplaneCommands : IAirplaneCommands
    {
        public void Update(double time)
        {
            
        }

        public float Thrust { get { return 0; } }
        public float LeftAileron { get { return 0; } }
        public float RightAileron { get { return 0; } }
        public float Elevator { get { return 0; } }
    }
}
