using System.Windows.Input;
using Alpha.Graphics;
using Alpha.UI.Coordinates;
using Alpha.UI.Styles;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Controls
{
    using System;
    class Button : Control
    {
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

        private ButtonStyle _style;
        private Text _text;
        private PlainRectangle _plainRectangle;
        public event CustomEventHandler<Button> Clicked;
        public Key? Shortcut { get; set; }


        public Button(IGame game, String id, UniRectangle coordinates, String text)
            : base(game, id, coordinates)
        {
            _textValue = text;
        }
        
        public override void Initialize()
        {
            IRenderer renderer = Game.Services.GetService<IRenderer>();
            _style = UiManager.StyleManager.GetStyle(this);
            _plainRectangle = new PlainRectangle(renderer, Size, Color.White);
            _text = renderer.TextManager.Create("Arial", 20, _textValue, Size, _style.TextColor, 
                _style.HorizontalAlignment, _style.VerticalAlignment);
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

        public override void OnMouseClicked()
        {
            _plainRectangle.Color = Color.SaddleBrown.ToVector4();
        }

        public override void OnMouseReleased()
        {
            _plainRectangle.Color = Color.Khaki.ToVector4();
            Clicked.Raise(this);
        }

        public override void OnMouseClickCanceled()
        {
            OnMouseLeft();
        }

        public override string ComponentType
        {
            get { return "button"; }
        }

        protected override bool OnKeyPressed(Key key, bool repeat)
        {
            if (Shortcut == null || repeat || Shortcut != key)
                return false;
            Clicked.Raise(this);
            return true;
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _plainRectangle.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
            _text.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
        }
    }
}