using System.Globalization;
using System.Windows.Input;
using Alpha.Core.Commands;
using Alpha.DirectX.UI.Controls;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.World;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Screens
{
    class GameScreen : Screen
    {
        private readonly Sun _sun;
        private readonly Water _water;
        private readonly Terrain _terrain;
        private readonly Sky _sky;
        private readonly FleetRenderer _fleetRenderer;
        private readonly FleetMoveOrderRenderer _fleetMoveOrderRenderer;
        private readonly FpsCounter _counter;
        public GameScreen(IContext context) : base(context, "game_screen", false)
        {
            _sun = new Sun();
            _water = new Water(context, context.World.ProvinceManager.SeaProvinces);
            _terrain = new Terrain(context, context.World.ProvinceManager.LandProvinces);
            _sky = new Sky(context);
            _fleetRenderer = new FleetRenderer(context);
            _fleetMoveOrderRenderer = new FleetMoveOrderRenderer(context);
            _counter = new FpsCounter();
            Register(new DynamicLabel(context, "calendar", new UniRectangle(new UniScalar(1.0f, -100), 0, 100, 50),
                () => context.World.Calendar.CurrentDate.ToString()));
            Register(new DynamicLabel(context, "fps", new UniRectangle(100, 0, 100, 50),
                () => _counter.Value + "FPS"));
            Register(new DynamicLabel(context, "treasury", new UniRectangle(new UniScalar(0.5f, -50), 0, 100, 50),
                () => context.Realm.Economy.Treasury.ToString(CultureInfo.InvariantCulture)));
            Button button;
            Register(button = new Button(context, "boost_treasury", new UniRectangle(new UniScalar(0.5f, -50), 50, 100, 50),
                "Click"));
            button.Clicked += b => context.RegisterCommand(new ChangeTreasuryCommand(context.Realm, 10));
        }

        protected override void Update(double delta)
        {
            _water.Update(delta);
            _terrain.Update(delta);
            _counter.Update(delta);
            _fleetMoveOrderRenderer.Update(delta);
            UpdateCameraFromInput();
        }

        private void UpdateCameraFromInput()
        {
            if (Context.UiManager.IsAnyKeyPressed(Key.LeftShift, Key.RightShift))
            {
                if (Context.UiManager.IsKeyPressed(Key.Left))
                    Context.Camera.Rotate(1);
                if (Context.UiManager.IsKeyPressed(Key.Right))
                    Context.Camera.Rotate(-1);
            }
            else
            {
                if (Context.UiManager.IsKeyPressed(Key.Left))
                    Context.Camera.Move(-1, 0);
                if (Context.UiManager.IsKeyPressed(Key.Right))
                    Context.Camera.Move(1, 0);
                if (Context.UiManager.IsKeyPressed(Key.Up))
                    Context.Camera.Move(0, 1);
                if (Context.UiManager.IsKeyPressed(Key.Down))
                    Context.Camera.Move(0, -1);
            }
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _sky.Render(deviceContext, Context.Camera.ViewMatrix, Context.DirectX.ProjectionMatrix, _sun, Context.Camera);
            _water.Render(deviceContext, Matrix.Identity, Context.Camera.ViewMatrix, Context.DirectX.ProjectionMatrix, _sun);
            _terrain.Render(deviceContext, Matrix.Identity, Context.Camera.ViewMatrix, Context.DirectX.ProjectionMatrix);
            _fleetRenderer.Render3D(deviceContext, Context.Camera.ViewMatrix,
                Context.DirectX.ProjectionMatrix, _sun, Context.Camera);
            _fleetMoveOrderRenderer.Render(deviceContext, Context.Camera.ViewMatrix, Context.DirectX.ProjectionMatrix);
            _fleetRenderer.RenderOverlay(deviceContext, viewMatrix, projectionMatrix);
        }
    }
}