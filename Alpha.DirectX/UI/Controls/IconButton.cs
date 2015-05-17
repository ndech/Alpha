using System;
using System.Windows.Input;
using Alpha.DirectX.UI.Styles;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Controls
{
    class IconButton : Icon
    {
        private ShaderResourceView _clickedTexture;
        public event CustomEventHandler Clicked;
        public Key? Shortcut { get; set; }

        public IconButton(IContext context, string id)
            : base(context, id)
        {
            Overlay = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            IconStyle style = Context.UiManager.StyleManager.GetStyle(this);
            _clickedTexture = Context.TextureManager.Create(style.ClickedTexture, "").TextureResource;
        }

        public override void OnMouseClicked()
        {
            CurrentTexture = _clickedTexture;
        }

        public override void OnMouseReleasedInBounds()
        {
            CurrentTexture = HoveredTexture;
            Clicked.Raise();
        }

        public override void OnMouseReleasedOutOfBounds()
        {
            OnMouseLeft();
        }
        
        protected override bool OnKeyPressed(Key key, char? character, bool repeat)
        {
            if (Shortcut == null || repeat || Shortcut != key)
                return false;
            Clicked.Raise();
            return true;
        }
    }
}