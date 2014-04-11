using System.Xml.Serialization.Advanced;
using Alpha.Graphics;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Controls
{
    using System;
    class Button : Control
    {
        public Vector2I Size { get; set; }
        public Vector2I Position { get; set; }

        public Int32 Height
        {
            get { return Size.Y; }
            set { Size = new Vector2I(value, Size.Y); }
        }
        public Int32 Width
        {
            get { return Size.X; }
            set { Size = new Vector2I(Size.X, value); }
        }
        public String Text
        {
            get { return _text.Content; }
            set { _text.Content = value; }
        }
        public VerticalAlignment VerticalAlignment { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }
        private readonly Text _text;
        private readonly PlainRectangle _plainRectangle;
        private bool _focused;

        public Button(IGame game, string text, int width, int height)
            : base(game)
        {
            IRenderer renderer = game.Services.GetService<IRenderer>();
            Size = new Vector2I(width, height);
            _plainRectangle = new PlainRectangle(renderer, new Vector2I(100, 100), Size, new Vector4(1, 1, 1, 1), 0);
            _text = renderer.TextManager.Create("Arial", 20, 80, new Vector4(0, 0, 1, 0.5f));
            _text.Content = text;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Middle;
            _focused = false;
        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
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

        public void OnFocus()
        {
            if (!_focused)
            {
                _plainRectangle.Color = new Vector4(1, 0, 0, 1);
                _focused = true;
            }
        }

        public void OnFocusLost()
        {
            if (_focused)
            {
                _plainRectangle.Color = new Vector4(1, 1, 1, 1);
                _focused = false;
            }
        }

        public void OnMouseMoved(Vector2I position)
        {
            if (position.X >= Position.X
                && position.X < Position.X + Size.X
                && position.Y >= Position.Y
                && position.Y < Position.Y + Size.Y)
                OnFocus();
            else
                OnFocusLost();
        }
    }
}