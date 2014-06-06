using System;
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

        public Icon(IGame game, String id, UniRectangle coordinates, Texture texture)
            : base(game, id, coordinates)
        {
            _texture = texture;
            Overlay = true;
        }

        public override string ComponentType
        {
            get { return "icon"; }
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            Rectangle.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
        }

        protected override void DisposeItem()
        {
            Rectangle.Dispose();
        }

        public override void Initialize()
        {
            IRenderer renderer = Game.Services.GetService<IRenderer>();
            Rectangle = new TexturedRectangle(renderer, Size, _texture);
        }
    }
}
