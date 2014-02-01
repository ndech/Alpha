using PlaneSimulator.Graphics;
using PlaneSimulator.Graphics.Models;
using SharpDX;
using SharpDX.Direct3D11;
using Vector3 = PlaneSimulator.Toolkit.Math.Vector3;

namespace PlaneSimulator
{
    public class World : RenderableGameComponent
    {
        private readonly Terrain _terrain;
        public World(Game game, Renderer renderer) 
            : base(game, renderer, 0)
        {
            _terrain = new Terrain(renderer.DirectX.Device, "Heightmap.png", 100, Renderer);
        }

        public override void Update(double delta)
        {
            _terrain.Update((float)delta);
        }

        public override void Dispose() { }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _terrain.Render(Renderer.DirectX.DeviceContext, 
                Matrix.Identity, 
                Renderer.Camera.ViewMatrix, 
                Renderer.DirectX.ProjectionMatrix, 
                Renderer.Light);
        }

        public double Gravity
        {
            get { return 0.81; }
        }

        public double Altitude(Vector3 position)
        {
            return _terrain.GetHeight(position.X, position.Y);
        }
    }
}