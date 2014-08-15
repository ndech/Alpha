using Alpha.DirectX.UI.World;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Screens
{
    class GameScreen : Screen
    {
        private readonly Sun _sun;
        private readonly Water _water;
        public GameScreen(IContext context) : base(context, "game_screen", false)
        {
            _sun = new Sun();
            _water = new Water(context, context.World.ProvinceManager.SeaProvinces);
        }

        protected override void Update(double delta)
        {
            _water.Update(delta);
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _water.Render(deviceContext, Matrix.Identity, Context.Camera.ViewMatrix, Context.DirectX.ProjectionMatrix, _sun);
        }
    }
}