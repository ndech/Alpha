﻿using System;
using System.Windows.Input;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Styles;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Controls
{
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
        private Text.Text _text;
        private PlainRectangle _plainRectangle;
        public event CustomEventHandler<Button> Clicked;
        public Key? Shortcut { get; set; }


        public Button(IContext context, String id, UniRectangle coordinates, String text)
            : base(context, id, coordinates)
        {
            _textValue = text;
        }
        
        public override void Initialize()
        {
            _style = Context.UiManager.StyleManager.GetStyle(this);
            _plainRectangle = new PlainRectangle(Context, Size, Color.White);
            _text = Context.TextManager.Create(_style.Font, _style.FontSize, _textValue, Size, _style.TextColor, 
                _style.HorizontalAlignment, _style.VerticalAlignment, _style.Padding);
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

        protected override bool OnKeyPressed(Key key, char? character, bool repeat)
        {
            if (Shortcut == null || repeat || Shortcut != key)
                return false;
            Clicked.Raise(this);
            return true;
        }

        protected override void DisposeItem()
        {
            _text.Dispose();
            _plainRectangle.Dispose();
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _plainRectangle.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
            _text.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
        }
    }
}