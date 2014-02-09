using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator
{
    public interface IAirplaneCommands
    {
        void Update(double time);

        float Thrust { get; }
        float LeftAileron { get; }
        float RightAileron { get; }

        float Elevator { get; }
    }
}
