using Alpha.Graphics;
using Alpha.Toolkit.Math;
using Alpha.UI.Coordinates;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Controls
{
    class Panel : Control
    {
        protected PlainRectangle Rectangle;
        protected Color Color;
        
        public Panel(IGame game, UniRectangle coordinates, Color color)
            : base(game, coordinates)
        {
            Color = color;
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            Rectangle.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
        }

        public override void Initialize()
        {
            IRenderer renderer = Game.Services.GetService<IRenderer>();
            Rectangle = new PlainRectangle(renderer, new Vector2I(0,0), Size, Color);
        }
    }
}
