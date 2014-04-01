using System;
using Alpha.Graphics;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha
{
    public abstract class RenderableGameComponent : GameComponent, IRenderable, IComparable<RenderableGameComponent>
    {
        protected RenderableGameComponent(IGame game, int updateOrder = 0, bool zBufferEnabled = true, bool blendingEnabled = false, bool displayWireframe = false) 
            : base(game, updateOrder: updateOrder)
        {
            DisplayWireframe = displayWireframe;
            BlendingEnabled = blendingEnabled;
            ZBufferEnabled = zBufferEnabled;
        }

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