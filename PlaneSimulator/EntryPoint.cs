using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator
{
    class EntryPoint
    {
        public static void Main()
        {
            Airplane plane = new Airplane();
            plane.Tanks.Add(new Tank(100,500));
        }
    }
}
