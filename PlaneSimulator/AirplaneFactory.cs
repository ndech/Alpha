using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator
{
    class AirplaneFactory
    {
        public static Airplane Create(World world)
        {
            Airplane plane = new Airplane(world);
            plane.PhysicalModel.Tanks.Add(new Tank(100, 500));
            plane.PhysicalModel.Tanks.Add(new Tank(100, 500));
            plane.PhysicalModel.Thrusters.Add(new Thruster());
            return plane;
        }
    }
}
