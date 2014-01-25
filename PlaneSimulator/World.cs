using PlaneSimulator.Graphics;
using SharpDX;
using SharpDX.Direct3D11;

namespace PlaneSimulator
{
    public class World : RenderableGameComponent
    {
        private readonly Terrain _terrain;
        public World(Game game, Renderer renderer) 
            : base(game, renderer, 0)
        {
            _terrain = new Terrain(renderer.DirectX.Device, "Heightmap.png", 100);
        }

        public override void Update(double delta) { }

        public override void Dispose() { }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _terrain.Render(Renderer.DirectX.DeviceContext, 
                Renderer.DirectX.WorldMatrix, 
                Renderer.Camera.ViewMatrix, 
                Renderer.DirectX.ProjectionMatrix, 
                Renderer.Light);
        }

        public double Gravity
        {
            get { return 0.81; }
        }
    }
}