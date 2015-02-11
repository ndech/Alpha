using Alpha.DirectX.Shaders;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.World
{
    class Sky
    {
        private readonly ObjModel _skydome;
        private float _angle;
        private readonly LightShader _shader;

        public Sky(IContext context)
        {
            _skydome = new ObjModel(context.DirectX.Device, "skydome.obj", context.TextureManager.Create("Sky.png"));
            _shader = context.Shaders.Get<LightShader>();
        }

        public void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix, Light light, ICamera camera)
        {
            _skydome.Render(deviceContext);
            _shader.Render(deviceContext, _skydome.IndexCount,
                Matrix.Scaling(10000) * Matrix.RotationY(MathUtil.PiOverTwo - _angle / 8), viewMatrix,
                projectionMatrix, _skydome.Texture, light, camera);
        }

        public void Update(double delta)
        {
            _angle += (float)delta * 0.04f;
        }
    }
}
