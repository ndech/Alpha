using System.Linq;
using Alpha.Common;
using Alpha.Core.Provinces;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.World;
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

        public LandProvince SelectedProvince
        {
            set { _camera.Position = (Vector3) value.Center + new Vector3(125, 200, -90) /*+ new Vector3(-125,0,90)*/ ; }
        }

        private Camera _camera;

        public ProvinceMap(IContext context) : base(context, "province_map", new UniRectangle())
        {
        }

        public override void Initialize()
        {
            _texture = new RenderTexture(Context.DirectX.Device, Size);
            _rectangle = new TexturedRectangle(Context, _texture.ShaderResourceView, Size);
            _camera = new Camera(new Vector3(0,200,0), new Vector3(0, MathUtil.PiOverTwo, 0));
            _terrain = new Terrain(Context, Context.World.ProvinceManager.LandProvinces.ToList());
            _orthoProjectionMatrix = Matrix.OrthoLH(250, 180, ConfigurationManager.Config.NearLimit,
                ConfigurationManager.Config.FarLimit);
        }

        public override string ComponentType => "province_map";

        protected override void Update(double delta)
        {
            _camera.Update(delta);
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _texture.ClearRenderTarget(Context.DirectX.DeviceContext, 0, 0, 0.5f, 1);
            _texture.SetRenderTarget(Context.DirectX.DeviceContext);
            
            _terrain.Render(deviceContext, Matrix.Identity, _camera.ViewMatrix, _orthoProjectionMatrix);

            Context.DirectX.SetBackBufferAsRenderTarget();
            _rectangle.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
        }

        protected override void DisposeItem()
        {
            _texture.Dispose();
            _rectangle.Dispose();
        }
    }
}
