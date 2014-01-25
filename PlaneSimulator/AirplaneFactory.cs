using PlaneSimulator.Graphics;
using PlaneSimulator.Toolkit.Math;

namespace PlaneSimulator
{
    static class AirplaneFactory
    {
        public static Airplane Create(World world, Game game, Renderer renderer, bool isPlayer = false)
        {
            State state = new State(1000, 200);
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
