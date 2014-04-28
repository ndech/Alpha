using Alpha.Graphics;
using Alpha.UI.Coordinates;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Controls
{
    class Icon : Control
    {
        protected TexturedRectangle Rectangle;
        protected Texture _texture;

        public Icon(IGame game, UniRectangle coordinates, Texture texture)
            : base(game, coordinates)
        {
            _texture = texture;
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            Rectangle.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
        }

        public override void Initialize()
        {
            IRenderer renderer = Game.Services.GetService<IRenderer>();
            Rectangle = new TexturedRectangle(renderer, Size, _texture);
        }
    }
}
