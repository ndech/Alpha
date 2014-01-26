using System;
using PlaneSimulator.Graphics;
using PlaneSimulator.Toolkit.Math;

namespace PlaneSimulator
{
    static class AirplaneFactory
    {
        public static Random random;
        public static Airplane Create(World world, Game game, Renderer renderer, bool isPlayer = false)
        {
            if(random == null)
                random = new Random();

            State state;
            if (isPlayer)
                state = new State(1400, 200);
            else
                state = new State(200 + random.Next(0,2000), random.Next(-500,500), 500 + random.Next(0, 1000), 200+ random.Next(0,100), random.Next(-5,5));
            Airplane plane = new Airplane(world, state, game, renderer, isPlayer);
            plane.PhysicalModel.Tanks.Add(new Tank(100, 500));
            plane.PhysicalModel.Tanks.Add(new Tank(100, 500));
            plane.PhysicalModel.Thrusters.Add(new Thruster());
            plane.CurrentState.Position = new Vector3(plane.CurrentState.Position.X - 2500,
                plane.CurrentState.Position.Y, plane.CurrentState.Position.Z);
            return plane;
        }
    }
}
