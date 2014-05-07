using System;
using Alpha.Graphics;
using Alpha.Toolkit.Math;
using Alpha.UI.Coordinates;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Controls
{
    class Tooltip : Control
    {
        private bool _visible;
        private TexturedExtensibleRectangle _rectangle;
        private double _delay;
        private double _cooldown;
        private SimpleText _text;
        private Control _control;
        private Vector2I _size;
        private Vector2I _position;

        public override Vector2I Size
        {
            get { return _size; }
        }

        public override Vector2I Position
        {
            get { return _position; }
        }

        public override Vector2I RelativePosition
        {
            get { return _position; }
        }

        public Tooltip(IGame game, String id, Control control, double delay, string text)
            : base(game, id, new UniRectangle())
        {
            IRenderer renderer = game.Services.GetService<IRenderer>();

            _control = control;
            _text = renderer.TextManager.Create("Arial", 20, 1080, Color.Wheat);
            _text.Content = text;
            _size = new Vector2I(_text.Size.X + 16, _text.Size.Y + 16);
            Texture texture = renderer.TextureManager.Create("tooltip.png", @"Data/UI/");
            _rectangle = new TexturedExtensibleRectangle(
                renderer,_size, texture, 8);
            _delay = delay;
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            if (!_visible) return;
            _rectangle.Render(deviceContext, worldMatrix * Matrix.Translation(-8,-8,0), viewMatrix, projectionMatrix);
            _text.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
        }
        
        public override string ComponentType
        {
            get { return "tooltip"; }
        }

        protected override void Update(double delta)
        {
            if (!_control.Hovered)
            {
                _visible = false;
                _cooldown = 0;
                return;
            }
            if (!_visible)
            {
                _cooldown += delta;
                if (_cooldown > _delay)
                    _visible = true;
            }
            if (_visible)
            {
                _position = UiManager.MousePosition + new Vector2I(20, 20);
            }
        }

        public override void Initialize()
        {

        }
    }
}