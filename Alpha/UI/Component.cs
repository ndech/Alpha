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
    abstract class Component
    {
        public abstract void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix);
        
        protected Component(IGame game)
        {

        }
    }
}
