using System;
using Alpha.DirectX.UI.Coordinates;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Controls
{
    class Panel : Control
    {
        protected PlainRectangle Rectangle;
        protected Color Color;
        
        public Panel(IContext context, String id, UniRectangle coordinates, Color color)
            : base(context, id, coordinates)
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
            Rectangle = new PlainRectangle(Context, Size, Color);
        }
    }
}
