using SharpDX;
using SharpDX.Direct3D11;

namespace PlaneSimulator
{
    interface IRenderable
    {
        bool IsUi { get; }
        bool DisplayWireframe { get; }
        bool BlendingEnabled { get; }
        bool ZBufferEnabled { get; }
        void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix);
    }
}