using System;
using PlaneSimulator.Graphics;
using PlaneSimulator.Toolkit.Math;

namespace PlaneSimulator
{
    static class AirplaneFactory
    {
        public static Random random;
        public static int i = 0;
        public static Airplane Create(World world, Game game, Renderer renderer, bool isPlayer = false, Airplane playerPlane = null)
        {
            if(random == null)
                random = new Random();

            State state;
            if (isPlayer)
                state = new State(400, 200);
            else
                state = new State(200 + random.Next(-1000,2000), random.Next(-1500,1500), 500 + random.Next(0, 1000), 200+ random.Next(0,100), random.Next(-15,15));
            Airplane plane = new Airplane(world, state, game, renderer, isPlayer, playerPlane, "F-14", "Plane " + i++);
            plane.PhysicalModel.Tanks.Add(new Tank(100, 500));
            plane.PhysicalModel.Tanks.Add(new Tank(100, 500));
            plane.PhysicalModel.Thrusters.Add(new Thruster());
            plane.CurrentState.Position = new Vector3(plane.CurrentState.Position.X - 2500,
                plane.CurrentState.Position.Y, plane.CurrentState.Position.Z);
            return plane;
        }
    }
}
