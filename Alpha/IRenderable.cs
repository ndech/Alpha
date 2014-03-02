using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha
{
    interface IRenderable
    {
        bool DisplayWireframe { get; }
        bool BlendingEnabled { get; }
        bool ZBufferEnabled { get; }
        void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix);
    }
}