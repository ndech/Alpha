using System;
using Alpha.Graphics;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Controls
{
    class Label : UiComponent
    {
        private readonly Text _text;
        public String Text
        {
            get { return _text.Content; }
            set { _text.Content = value; }
        }

        public Label(IGame game, Vector2I size, Vector2I position, String text)
            : base(game, size, position)
        {
            IRenderer renderer = game.Services.GetService<IRenderer>();
            _text = renderer.TextManager.Create("Arial", 20, text.Length, Color.SteelBlue);
            _text.Content = text;
            _text.Position = Position;
        }

        protected override void RenderComponent(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _text.Render(deviceContext, Matrix.Identity, viewMatrix, projectionMatrix);
        }

        protected override void UpdateComponent(double delta) { }
    }
}
