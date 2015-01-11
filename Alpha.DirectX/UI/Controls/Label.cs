using System;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Styles;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Controls
{
    class Label : Control, IStylable<Label, LabelStyle>
    {
        private LabelStyle _style;
        private Text.Text _text;
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

        public Color TextColor { get { return _text.BaseColor; } set { _text.BaseColor = value; } }

        public Label(IContext context, String id, UniRectangle coordinates, String text)
            : base(context, id, coordinates)
        {
            _textValue = text;
        }

        public override string ComponentType
        {
            get { return "label"; }
        }

        public UiComponent Component { get { return this; } }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _text.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
        }

        protected override void DisposeItem()
        {
            _text.Dispose();
        }

        public override void Initialize()
        {
            _style = Context.UiManager.StyleManager.GetStyle(this);
            _text = Context.TextManager.Create(_style.Font, _style.FontSize, _textValue, Size, _style.TextColor,
                _style.HorizontalAlignment, _style.VerticalAlignment, _style.Padding);
        }

        public override void OnResize()
        {
            _text.Size = Size;
        }
    }
}
