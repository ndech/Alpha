using System;
using PlaneSimulator.Graphics;
using SharpDX;
using SharpDX.Direct3D11;

namespace PlaneSimulator
{
    public abstract class RenderableGameComponent : GameComponent, IRenderable, IComparable<RenderableGameComponent>
    {
        protected RenderableGameComponent(Game game, Renderer renderer, int updateOrder = 0, bool zBufferEnabled = true, bool blendingEnabled = false, bool displayWireframe = false) 
            : base(game, updateOrder)
        {
            Renderer = renderer;
            DisplayWireframe = displayWireframe;
            BlendingEnabled = blendingEnabled;
            ZBufferEnabled = zBufferEnabled;
        }

        protected readonly Renderer Renderer;
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