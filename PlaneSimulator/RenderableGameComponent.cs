using System;
using SharpDX;
using SharpDX.Direct3D11;

namespace PlaneSimulator
{
    public abstract class RenderableGameComponent : GameComponent, IRenderable, IComparable<RenderableGameComponent>
    {
        public abstract bool IsUi { get; }
        public bool DisplayWireframe { get; private set; }
        public bool BlendingEnabled { get; private set; }
        public bool ZBufferEnabled { get; private set; }
        public abstract void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix);
        public int CompareTo(RenderableGameComponent other)
        {
            return 1; //Todo
        }
    }
}