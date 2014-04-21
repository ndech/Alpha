using Alpha.Graphics;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Controls
{
    class Panel: UiComponent
    {
        private PlainRectangle _rectangle;
        public Panel(IGame game, Vector2I size, Vector2I position, Color color) 
            : base(game, size, position)
        {
            IRenderer renderer = game.Services.GetService<IRenderer>();
            _rectangle = new PlainRectangle(renderer, position, size, color);
        }

        protected override void RenderComponent(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _rectangle.Render(deviceContext, Matrix.Identity, viewMatrix, projectionMatrix);
        }
    }
}
