using System;
using Alpha.Graphics;
using Alpha.Toolkit.Math;
using Alpha.UI.Coordinates;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Controls
{
    class Label : Control
    {
        private Text _text;
        private String _textValue;
        public String Text
        {
            get { return _textValue; }
            set
            {
                _textValue = value;
                _text.Content = value;
            }
        }

        public Label(IGame game, UniRectangle coordinates, String text)
            : base(game, coordinates)
        {
            _textValue = text;
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _text.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
        }

        public override void Initialize()
        {
            IRenderer renderer = Game.Services.GetService<IRenderer>();
            _text = renderer.TextManager.Create("Arial", 20, _textValue, Size, Color.SteelBlue);
        }
    }
}
