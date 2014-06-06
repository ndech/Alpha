using System;
using Alpha.Graphics;
using Alpha.UI.Coordinates;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Controls
{
    class Panel : Control
    {
        protected PlainRectangle Rectangle;
        protected Color Color;
        
        public Panel(IGame game, String id, UniRectangle coordinates, Color color)
            : base(game, id, coordinates)
        {
            Color = color;
        }

        public override string ComponentType
        {
            get { return "panel"; }
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
            Rectangle = new PlainRectangle(renderer, Size, Color);
        }
    }
}
