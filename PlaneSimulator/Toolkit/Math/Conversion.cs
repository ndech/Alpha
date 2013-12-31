using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator.Toolkit.Math
{
    class Conversion
    {
        public static float AngleToRadian(float degrees)
        {
            return degrees*0.0174532925f;
        }
    }
}
