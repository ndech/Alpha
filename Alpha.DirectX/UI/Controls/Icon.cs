using System;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Styles;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Controls
{
    internal class Icon : Control, IStylable<Icon, IconStyle>
    {
        private TexturedRectangle _texturedRectangle;

        private ShaderResourceView _baseTexture;
        public ShaderResourceView BaseTexture
        {
            get { return _baseTexture; }
            set
            {
                if (CurrentTexture == _baseTexture)
                    CurrentTexture = value;
                _baseTexture = value;
            }
        }

        public ShaderResourceView HoveredTexture;
        protected ShaderResourceView CurrentTexture;

        public Icon(IContext context, String id)
            : base(context, id, new UniRectangle())
        {
            Overlay = true;
            Visible = true;
        }

        public override void Initialize()
        {
            IconStyle style = Context.UiManager.StyleManager.GetStyle(this);
            BaseTexture = Context.TextureManager.Create(style.BaseTexture, "").TextureResource;
            HoveredTexture = Context.TextureManager.Create(style.HoveredTexture, "").TextureResource;
            _texturedRectangle = new TexturedRectangle(Context, BaseTexture, Size);
        }

        public override void OnMouseEntered()
        {
            CurrentTexture = HoveredTexture;
        }

        public override void OnMouseLeft()
        {
            CurrentTexture = BaseTexture;
        }
        
        public override string ComponentType
        {
            get { return "icon"; }
        }

        public UiComponent Component
        {
            get { return this; }
        }

        protected override void DisposeItem()
        {
            _texturedRectangle.Dispose();
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix,
            Matrix projectionMatrix)
        {
            _texturedRectangle.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix, CurrentTexture);
        }
    }
}
