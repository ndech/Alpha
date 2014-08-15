using Alpha.DirectX.UI.Controls;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.World;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Screens
{
    class GameScreen : Screen
    {
        private readonly Sun _sun;
        private readonly Water _water;
        private readonly Sky _sky;
        public GameScreen(IContext context) : base(context, "game_screen", false)
        {
            _sun = new Sun();
            _water = new Water(context, context.World.ProvinceManager.SeaProvinces);
            _sky = new Sky(context);
            Register(new DynamicLabel(context, "calendar", new UniRectangle(new UniScalar(1.0f, -100), 0, 100, 50),
                () => context.World.Calendar.CurrentDate.ToString()));
        }

        protected override void Update(double delta)
        {
            _water.Update(delta);
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _sky.Render(deviceContext, Context.Camera.ViewMatrix, Context.DirectX.ProjectionMatrix, _sun, Context.Camera);
            _water.Render(deviceContext, Matrix.Identity, Context.Camera.ViewMatrix, Context.DirectX.ProjectionMatrix, _sun);
        }
    }
}