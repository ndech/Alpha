using Alpha.Graphics;
using Alpha.Toolkit.Math;
using Alpha.UI.Coordinates;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Controls
{
    using System;
    class Button : Control
    {
        private String _textValue;
        //public String Text
        //{
        //    get { return _textValue; }
        //    set
        //    {
        //        _textValue = value;
        //        _text.Content = value;
        //    }
        //}
        
        private Text _text;
        private PlainRectangle _plainRectangle;

        public Button(IGame game, UniRectangle coordinates, String text)
            : base(game, coordinates)
        {
            _textValue = text;
        }
        
        public override void Initialize()
        {
            IRenderer renderer = Game.Services.GetService<IRenderer>();
            _plainRectangle = new PlainRectangle(renderer, new Vector2I(0,0), Size, Color.White);
            _text = renderer.TextManager.Create("Arial", 20, _textValue, Size, Color.Red);
            _text.Content = _textValue;
        }

        public override void OnMouseEntered()
        {
            _plainRectangle.Color = Color.Khaki.ToVector4();
        }

        public override void OnMouseLeft()
        {
            _plainRectangle.Color = new Vector4(1, 1, 1, 1);
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _plainRectangle.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
            _text.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
        }
    }
}