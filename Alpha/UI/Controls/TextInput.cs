using System;
using System.Windows.Input;
using Alpha.Graphics;
using Alpha.UI.Coordinates;
using Alpha.UI.Styles;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Controls
{
    class TextInput : Control
    {

        private String _textValue;
        public String Text
        {
            get { return _textValue; }
            set
            {
                _textValue = value;
                _text.Content = Graphics.Text.Escape(value);
            }
        }

        private Text _text;
        private PlainRectangle _plainRectangle;
        public CustomEventHandler<String> OnSubmit;

        public void Submit()
        {
            OnSubmit.Raise(Text);
            Text = "";
        }
        public TextInput(IGame game, string id, UniRectangle coordinates) : base(game, id, coordinates)
        {
            _textValue = "";
        }

        public override string ComponentType
        {
            get { return "textInput"; }
        }

        public override void Initialize()
        {
            IRenderer renderer = Game.Services.Get<IRenderer>();
            TextInputStyle style = UiManager.StyleManager.GetStyle(this);
            _plainRectangle = new PlainRectangle(renderer, Size, Color.BlanchedAlmond);
            _text = renderer.TextManager.Create(style.Font, style.FontSize, _textValue, Size, style.TextColor,
                style.HorizontalAlignment, style.VerticalAlignment, style.Padding);
            _text.Content = _textValue;
        }

        public override void OnMouseClickCanceled()
        {
            OnMouseLeft();
        }


        protected override bool OnKeyPressed(Key key, char? character, bool repeat)
        {
            if (key == Key.Back)
            {
                if (Text.Length >= 1)
                {
                    Text = Text.Substring(0, Text.Length - 1);
                }
            }
            else if(key == Key.Enter)
            {
                Submit();
            }
            else if(character != null && character>=32)
                Text += character;
            else
                return false;
            return true;
        }

        protected override void DisposeItem()
        {
            _text.Dispose();
            _plainRectangle.Dispose();
            OnSubmit = null;
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _plainRectangle.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
            _text.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
        }
    }
}
