using System.Linq;
using Alpha.Common;
using Alpha.Core.Provinces;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.World;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class ProvinceMap : Control
    {
        private TexturedRectangle _rectangle;
        private RenderTexture _texture;
        private Terrain _terrain;
        private Matrix _orthoProjectionMatrix;
        private LandProvince _selectedProvince;

        private int _ratioX = 10;
        private int _ratioY = 10;
        private int _screenX;
        private int _screenY;
        private const float Scaling = 0.02f;

        public LandProvince SelectedProvince
        {
            set
            {
                _selectedProvince = value;
                MoveCamera((Vector3)value.Center);
            }
        }

        public void MoveCamera(Vector3 worldPosition, bool immediate = true)
        {
            Vector3 cameraPosition = worldPosition*Scaling + new Vector3(_ratioX, 200, -_ratioY) +
                                     new Vector3((float)-_ratioX * Size.X / _screenX, 0,
                                         (float) _ratioY * Size.Y / _screenY);
            if (immediate)
                _camera.ForcePosition(cameraPosition);
            else
                _camera.Position = cameraPosition;
        }

        private Camera _camera;

        public ProvinceMap(IContext context) : base(context, "province_map", new UniRectangle())
        { }

        public override void Initialize()
        {
            _screenX = ConfigurationManager.Config.ScreenSize.X;
            _screenY = ConfigurationManager.Config.ScreenSize.Y;
            _texture = new RenderTexture(Context.DirectX.Device, Size);
            _rectangle = new TexturedRectangle(Context, _texture.ShaderResourceView, Size);
            _camera = new Camera(new Vector3(0,200,0), new Vector3(0, MathUtil.PiOverTwo, 0));
            _terrain = new Terrain(Context, Context.World.ProvinceManager.LandProvinces.ToList());
            _orthoProjectionMatrix = Matrix.OrthoLH(2*_ratioX, 2*_ratioY, ConfigurationManager.Config.NearLimit,
                ConfigurationManager.Config.FarLimit);
            IconButton centerButton = new IconButton(Context, "province_map_center");
            centerButton.Coordinates = new UniRectangle(new UniScalar(1.0f, -35), 5, 30, 30);
            centerButton.Clicked += () => MoveCamera((Vector3) _selectedProvince.Center);
            Register(centerButton);
        }

        public override string ComponentType => "province_map";

        protected override void Update(double delta)
        {
            _camera.Update(delta);
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _texture.ClearRenderTarget(Context.DirectX.DeviceContext, 0.1f, 0.1f, 0.5f, 1);
            _texture.SetRenderTarget(Context.DirectX.DeviceContext);
            
            _terrain.Render(deviceContext, Matrix.Scaling(Scaling), _camera.ViewMatrix, _orthoProjectionMatrix);

            Context.DirectX.SetBackBufferAsRenderTarget();
            _rectangle.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
        }

        public override void OnMouseReleasedInBounds()
        {
            Vector2I relativePosition = Context.UiManager.MousePosition - Position ;

            Vector3 screenSize = new Vector3((float) _ratioX * Size.X /  _screenX, 0,
                                         (float) _ratioY * Size.Y / _screenY)*(2/Scaling);

            Vector2 offset2 = new Vector2(relativePosition.X * screenSize.X / Size.X, relativePosition.Y * screenSize.Z / Size.Y);

            Vector3D pickingPosition = new Vector3D((_camera.CurrentPosition.X-_ratioX) / Scaling + offset2.X, 0, (_camera.CurrentPosition.Z+_ratioY ) /Scaling - offset2.Y);

            Province province = ProvincePicker.ClosestProvince(pickingPosition);

            if (province is LandProvince)
                MoveCamera((Vector3) province.Center, false);
        }

        protected override void DisposeItem()
        {
            _texture.Dispose();
            _rectangle.Dispose();
        }
    }
}
