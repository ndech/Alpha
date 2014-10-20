using System.Globalization;
using System.Windows.Input;
using Alpha.Core.Provinces;
using Alpha.DirectX.UI.Controls;
using Alpha.DirectX.UI.Controls.Custom;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Layouts;
using Alpha.DirectX.UI.Styles;
using Alpha.DirectX.UI.World;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;
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
        private readonly ProvinceDetailPanel _provinceDetailPanel;
        private readonly SettlementDetailPanel _settlementDetailPanel;
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
            new PositionLayout(this, 200, 100, HorizontalAlignment.Center, VerticalAlignment.Top)
                .Create(new DynamicLabel(Context, "money", new UniRectangle(),
                    () => Context.Realm.Economy.Treasury.ToString(CultureInfo.InvariantCulture)));
            MinimapPanel minimapPanel = new MinimapPanel(context, _terrain);
            new PositionLayout(this, 300, 200, HorizontalAlignment.Right, VerticalAlignment.Bottom).Create(minimapPanel);
            ExtraMinimapButtonPanel extraMinimapButtonPanel = new ExtraMinimapButtonPanel(context, () => minimapPanel.ExtraPanelVisible);
            new PositionLayout(this, 300, 200, HorizontalAlignment.Center, VerticalAlignment.Bottom).Create(extraMinimapButtonPanel);
            _settlementDetailPanel = new SettlementDetailPanel(context);
            new PositionLayout(this, 500, 600, HorizontalAlignment.Left, VerticalAlignment.Bottom)
                .Create(_provinceDetailPanel = new ProvinceDetailPanel(context, _settlementDetailPanel));
            new PositionLayout(this, 500, 600, HorizontalAlignment.Center, VerticalAlignment.Middle)
                .Create(_settlementDetailPanel);
            Register(new MapTooltip(context, this));
            new PositionLayout(this, 100, 25, HorizontalAlignment.Left, VerticalAlignment.Top).Create(new RealmInfo(context));
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

        public override bool OnMouseScrolled(int delta)
        {
            Context.Camera.Zoom(-2*delta);
            return true;
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

        public Zone HoveredZone()
        {
            if (HoveredControl != null)
                return null;
            return Context.World.ProvinceManager.ClosestZone(
                     (Vector3D)new Picker(Context, Context.UiManager.MousePosition).GroundIntersection);
        }

        protected override void OnMouseClicked(Vector2I position, int button)
        {
            if (button == 0)
            {
                Province province = Context.World.ProvinceManager.ClosestProvince(
                    (Vector3D) new Picker(Context, Context.UiManager.MousePosition).GroundIntersection);
                if (province is LandProvince && (province as LandProvince).Owner == Context.Realm)
                    _provinceDetailPanel.ShowProvince(province as LandProvince);
            }
        }
    }
}