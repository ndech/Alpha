using System;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI
{
    enum VerticalAlignment
    {
        Top,
        Bottom,
        Middle
    }

    enum HorizontalAlignment
    {
        Left,
        Right,
        Center
    }

    internal abstract class UiComponent
    {
        protected IGame Game;

        protected UiComponent(IGame game)
        {
            Game = game;
        }

        public abstract void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix);
        public virtual void Update(double delta) { }
    }
}
