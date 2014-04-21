using Alpha.Graphics;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Controls
{
    using System;
    class Button : Control
    {
        public VerticalAlignment VerticalAlignment { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }

        public String Text
        {
            get { return _text.Content; }
            set { _text.Content = value; }
        }
        
        private readonly Text _text;
        private readonly PlainRectangle _plainRectangle;

        public Button(IGame game, Vector2I size, Vector2I position, String text)
            : base(game, size, position)
        {
            IRenderer renderer = game.Services.GetService<IRenderer>();
            _plainRectangle = new PlainRectangle(renderer, new Vector2I(100, 100), Size, Color.White, 0);
            _text = renderer.TextManager.Create("Arial", 20, 80, Color.SteelBlue);
            _text.Content = text;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Middle;
            FocusGained += (p) => { _plainRectangle.Color = new Vector4(1, 0, 0, 1); };
            FocusLost += (p) => { _plainRectangle.Color = new Vector4(1, 1, 1, 1); };
        }

        protected override void RenderComponent(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            int positionX = Position.X;
            int positionY = Position.Y;
            _plainRectangle.Position = new Vector2I(positionX, positionY);
            if (VerticalAlignment == VerticalAlignment.Middle)
                positionY += (Height - _text.Size.Y) >> 1;
            if (VerticalAlignment == VerticalAlignment.Bottom)
                positionY += (Height - _text.Size.Y);
            if (HorizontalAlignment == HorizontalAlignment.Center)
                positionX += (Width - _text.Size.X) >> 1;
            else if (HorizontalAlignment == HorizontalAlignment.Right)
                positionX += (Width - _text.Size.X);
            _text.Position = new Vector2I(positionX, positionY);

            _plainRectangle.Render(deviceContext, Matrix.Identity, viewMatrix, projectionMatrix);
            _text.Render(deviceContext, Matrix.Identity, viewMatrix, projectionMatrix);
        }
    }
}