using Alpha.Graphics;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Controls
{
    internal class Tooltip : UiComponent
    {
        private bool _visible;
        private bool _focused;
        private TexturedExtensibleRectangle _rectangle;
        private double _delay;
        private double _cooldown;
        private Text _text;

        public Tooltip(IGame game, Control button, double delay, string text)
            : base(game, Vector2I.Zero, Vector2I.Zero)
        {
            IRenderer renderer = game.Services.GetService<IRenderer>();

            _text = renderer.TextManager.Create("Arial", 20, 1080, Color.Wheat);
            _text.Content = text;

            Texture texture = renderer.TextureManager.Create("tooltip.png", @"Data/UI/");
            _rectangle = new TexturedExtensibleRectangle(
                renderer, 
                new Vector2I(0, 0),
                new Vector2I(_text.Size.X + 16, _text.Size.Y + 16), texture, 8);
            _delay = delay;


            IInput input = game.Services.GetService<IInput>();
            CustomEventHandler<Vector2I> mouseMouvedEventHandler = arg =>
            {
                input.MouseMoved += k =>
                {
                    _rectangle.Position = new Vector2I(k.X + 20, k.Y + 20);
                    _text.Position = new Vector2I(k.X + 28, k.Y + 28);
                };
            };

            button.FocusGained += d =>
            {
                _focused = true;
                input.MouseMoved += mouseMouvedEventHandler;
            };
            button.FocusLost += d =>
            {
                _visible = false;
                _focused = false;
                _cooldown = 0;
                input.MouseMoved -= mouseMouvedEventHandler;
            };
        }

        protected override void RenderComponent(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            if (!_visible) return;
            _rectangle.Render(deviceContext, Matrix.Identity, viewMatrix, projectionMatrix);
            _text.Render(deviceContext, Matrix.Identity, viewMatrix, projectionMatrix);
        }

        protected override void UpdateComponent(double delta)
        {
            if (_focused && !_visible)
            {
                _cooldown += delta;
                if (_cooldown > _delay)
                {
                    _visible = true;
                }
            }
        }
    }
}