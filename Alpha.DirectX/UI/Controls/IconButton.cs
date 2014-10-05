using System;
using System.Windows.Input;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Styles;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Controls
{
    class IconButton : Control, IStylable<IconButton, IconStyle>
    {
        private TexturedRectangle _texturedRectangle;
        private ShaderResourceView _baseTexture;
        private ShaderResourceView _hoveredTexture;
        private ShaderResourceView _clickedTexture;
        private ShaderResourceView _currentTexture;
        public event CustomEventHandler Clicked;
        public Key? Shortcut { get; set; }


        public IconButton(IContext context, String id)
            : base(context, id, new UniRectangle())
        {
        }

        public override void Initialize()
        {
            IconStyle style = Context.UiManager.StyleManager.GetStyle(this);
            _baseTexture = Context.TextureManager.Create(style.BaseTexture, "").TextureResource;
            _clickedTexture = Context.TextureManager.Create(style.ClickedTexture, "").TextureResource;
            _hoveredTexture = Context.TextureManager.Create(style.HoveredTexture, "").TextureResource;
            _texturedRectangle = new TexturedRectangle(Context, _baseTexture ,Size);
        }

        public override void OnMouseEntered()
        {
            _currentTexture = _hoveredTexture;
        }

        public override void OnMouseLeft()
        {
            _currentTexture = _baseTexture;
        }

        public override void OnMouseClicked()
        {
            _currentTexture = _clickedTexture;
        }

        public override void OnMouseReleasedInBounds()
        {
            _currentTexture = _hoveredTexture;
            Clicked.Raise();
        }

        public override void OnMouseReleasedOutOfBounds()
        {
            OnMouseLeft();
        }

        public override string ComponentType
        {
            get { return "icon"; }
        }

        public UiComponent Component { get { return this; }}

        protected override bool OnKeyPressed(Key key, char? character, bool repeat)
        {
            if (Shortcut == null || repeat || Shortcut != key)
                return false;
            Clicked.Raise();
            return true;
        }

        protected override void DisposeItem()
        {
            _texturedRectangle.Dispose();
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _texturedRectangle.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix, _currentTexture);
        }
    }
}