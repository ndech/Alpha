using System;
using Alpha.DirectX.UI.Coordinates;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Controls
{
    class ScrollBar : Control
    {
        public const int Width = 15;
        public event CustomEventHandler<int> Moved;
        private TexturedExtensibleRectangle _fixedPart;
        private TexturedExtensibleRectangle _movingPart;
        private readonly Texture _fixedTexture;
        private readonly Texture _movingTexture;
        private const int MinHeight = 10;
        private int _position;

        public ScrollBar(IContext context)
            : base(context, "scrollbar", new UniRectangle(new UniScalar(1.0f, -Width), 0, Width, 1.0f))
        {
            _fixedTexture = Context.TextureManager.Create("scrollbar_fix.png", @"Data/UI/");
            _movingTexture = Context.TextureManager.Create("scrollbar_mobile.png", @"Data/UI/");
            _position = Width;
        }

        public override string ComponentType
        {
            get { return "scrollBar"; }
        }

        protected override void DisposeItem()
        {
            _fixedPart.Dispose();
        }

        public override void Initialize()
        {
            _fixedPart = new TexturedExtensibleRectangle(Context, new Vector2I(Width, Size.Y), _fixedTexture, Width);
            _movingPart = new TexturedExtensibleRectangle(Context, new Vector2I(Width, 160), _movingTexture, 3);
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _fixedPart.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
            _movingPart.Render(deviceContext, worldMatrix * Matrix.Translation(0, _position, 0), viewMatrix, projectionMatrix);
        }

        public void Refresh(float totalNumberOfItems, float visibleNumberOfItems, float firstVisible)
        {
            int size;
            if (visibleNumberOfItems >= totalNumberOfItems)
            {
                size = Size.Y - 2 * Width;
                _position = Width;
            }
            else
            {
                size = Math.Max(MinHeight, (int)((Size.Y - 2 * Width) * (visibleNumberOfItems / totalNumberOfItems)));
                _position = Width + (int)((Size.Y - 2 * Width) * (firstVisible / totalNumberOfItems));
            }
            _movingPart.Size = new Vector2I(Width, size);
        }

        public override void OnMouseClicked()
        {
            int relativeYPosition = Context.UiManager.MousePosition.Y - Position.Y;
            if (relativeYPosition < Width)
                Moved.Raise(-1);
            else if (relativeYPosition > Size.Y - Width)
                Moved.Raise(1);
        }

        public override void OnResize()
        {
            _fixedPart.Size = new Vector2I(Width, Size.Y);
        }
    }
}
