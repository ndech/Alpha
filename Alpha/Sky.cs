using Alpha.Graphics;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha
{
    class Sky
    {
        private readonly ObjModel _skydome;
        private float _angle;

        public Sky(IRenderer renderer)
        {
            _skydome = new ObjModel(renderer.Device, "skydome.obj", renderer.TextureManager.Create("Sky.png"));
        }

        public void Render(IRenderer renderer, DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix, Light light, ICamera camera)
        {
            _skydome.Render(deviceContext);
            renderer.LightShader.Render(deviceContext, _skydome.IndexCount,
                Matrix.Scaling(10000)*Matrix.RotationY(MathUtil.PiOverTwo - _angle/8), viewMatrix,
                projectionMatrix, _skydome.Texture, light, camera);
        }

        public void Update(double delta)
        {
            _angle += (float)delta*0.04f;
        }
    }
}
