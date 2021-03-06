﻿using System;
using System.Windows.Input;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Styles;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Controls
{
    class TextInput : Control, IStylable<TextInput, TextInputStyle>
    {

        private string _textValue;
        public string Text
        {
            get { return _textValue; }
            set
            {
                _textValue = value;
                _text.Content = UI.Text.Text.Escape(value);
            }
        }

        private Text.Text _text;
        private PlainRectangle _plainRectangle;
        public CustomEventHandler<string> OnSubmit;

        public void Submit()
        {
            OnSubmit.Raise(Text);
            Text = "";
        }
        public TextInput(IContext context, string id, UniRectangle coordinates) 
            : base(context, id, coordinates)
        {
            _textValue = "";
        }

        public override string ComponentType
        {
            get { return "textInput"; }
        }

        public UiComponent Component { get { return this; } }

        public override void Initialize()
        {
            TextInputStyle style = Context.UiManager.StyleManager.GetStyle(this);
            _plainRectangle = new PlainRectangle(Context, Size, Color.BlanchedAlmond);
            _text = Context.TextManager.Create(style.Font, style.FontSize, _textValue, Size, style.TextColor,
                style.HorizontalAlignment, style.VerticalAlignment, style.Padding);
            _text.Content = _textValue;
        }

        public override void OnMouseReleasedOutOfBounds()
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
