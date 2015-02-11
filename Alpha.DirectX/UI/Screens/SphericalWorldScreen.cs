using Alpha.DirectX.UI.World;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Screens
{
    class SphericalWorldScreen : Screen
    {
        private readonly Sphere _sphere;
        private readonly Sphere _sphere2;
        private readonly Sky _sky;
        private readonly Sun _sun;
        private readonly SphericalWorldCamera _camera;
        public SphericalWorldScreen(IContext context) : base(context, "spherical_world")
        {
            _sphere = new Sphere(Context, Color.Red, 5, 1200);
            _sphere2 = new Sphere(Context, Color.Blue, 5, 0);
            _sky = new Sky(Context);
            _sun = new Sun();
            _camera = new SphericalWorldCamera();
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            Context.DirectX.EnableZBuffer();
            _sky.Render(deviceContext, _camera.ViewMatrix, Context.DirectX.ProjectionMatrix, _sun ,_camera);
            _sphere.Render(deviceContext, Matrix.Identity, _camera.ViewMatrix, Context.DirectX.ProjectionMatrix);
            _sphere2.Render(deviceContext, Matrix.Identity, _camera.ViewMatrix, Context.DirectX.ProjectionMatrix);
            Context.DirectX.DisableZBuffer();
        }
        protected override void DisposeItem()
        {
            DisposeHelper.DisposeAndSetToNull(_sphere, _sphere2);
        }
    }
}
