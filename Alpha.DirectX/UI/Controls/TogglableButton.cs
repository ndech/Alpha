using System;
using System.Windows.Input;
using Alpha.DirectX.UI.Coordinates;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Controls
{
    class TogglableButton : Control
    {
        private readonly string _normalTexturePath;
        private readonly string _toggledTexturePath;
        private TexturedRectangle _iconToggled;
        private TexturedRectangle _iconNormal;
        public event CustomEventHandler<Button> Clicked;
        private State _currentState;
        public Key? Shortcut { get; set; }
        public CustomEventHandler Toggled;

        public enum State
        {
            Normal,
            Toggled
        }

        public TogglableButton(IContext context, String id, UniRectangle coordinates, String normalTexturePath, String toggledTexturePath)
            : base(context, id, coordinates)
        {
            _normalTexturePath = normalTexturePath;
            _toggledTexturePath = toggledTexturePath;
            _currentState = State.Normal;
        }

        public override void Initialize()
        {
            _iconNormal = new TexturedRectangle(Context, Context.TextureManager.Create(_normalTexturePath,""), Size);
            _iconToggled = new TexturedRectangle(Context, Context.TextureManager.Create(_toggledTexturePath,""), Size);
        }
        
        public override void OnMouseClicked()
        {
            if (_currentState == State.Normal)
            {
                _currentState = State.Toggled;
                Toggled.Raise();
            }
            else
                _currentState = State.Normal;
        }

        public override string ComponentType
        {
            get { return "button"; }
        }

        protected override bool OnKeyPressed(Key key, char? character, bool repeat)
        {
            if (Shortcut == null || repeat || Shortcut != key)
                return false;
            OnMouseClicked();
            return true;
        }

        protected override void DisposeItem()
        {
            _iconNormal.Dispose();
            _iconToggled.Dispose();
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            if(_currentState == State.Normal)
                _iconNormal.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
            else
                _iconToggled.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
        }
    }
}
