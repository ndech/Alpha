using SharpDX;
using SharpDX.Direct3D11;

namespace PlaneSimulator
{
    interface IRenderable
    {
        void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix);
    }
}
