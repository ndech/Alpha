using System;
using Alpha.Graphics;
using Alpha.Toolkit.Math;
using Alpha.UI.Controls;
using Alpha.UI.Coordinates;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Scrollable
{
    class ScrollBar : Control
    {
        public event CustomEventHandler<int> Moved; 
        private TexturedExtensibleRectangle _fixedPart;
        private TexturedExtensibleRectangle _movingPart;
        private readonly Texture _fixedTexture;
        private readonly Texture _movingTexture;
        private readonly int _width;
        private const int MinHeight = 10;
        private int _position;
        public ScrollBar(IGame game, String id) : base(game, id, new UniRectangle())
        {
            IRenderer renderer = Game.Services.GetService<IRenderer>();
            _fixedTexture = renderer.TextureManager.Create("SlidingBarConsole_fix.png", @"Data/UI/");
            _movingTexture = renderer.TextureManager.Create("SlidingBarConsole_mobile.png", @"Data/UI/");
            _width = _fixedTexture.Width;
            Coordinates = new UniRectangle(new UniScalar(1.0f, -_width), 0, _width, 1.0f);
            _position = _width;
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
            IRenderer renderer = Game.Services.GetService<IRenderer>();
            _fixedPart = new TexturedExtensibleRectangle(renderer, new Vector2I(_width, Size.Y), _fixedTexture, _width);
            _movingPart = new TexturedExtensibleRectangle(renderer, new Vector2I(_width, 160), _movingTexture, 3);
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _fixedPart.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
            _movingPart.Render(deviceContext, worldMatrix * Matrix.Translation(0,_position,0), viewMatrix, projectionMatrix);
        }
        
        public void Refresh(int totalNumberOfItems, int visibleNumberOfItems, int firstVisible)
        {
            int size;
            if (visibleNumberOfItems >= totalNumberOfItems)
            {
                size = Size.Y - 2*_width;
                _position = _width;
            }
            else
            {
                size = Math.Max(MinHeight, (int)((Size.Y - 2 * _width) * ((float)visibleNumberOfItems / totalNumberOfItems)));
                _position = _width + (int) ((Size.Y - 2*_width)*((float) firstVisible/totalNumberOfItems));
            }
            _movingPart.Size = new Vector2I(_width, size);
        }

        public override void OnMouseClicked()
        {
            int relativeYPosition = UiManager.MousePosition.Y - Position.Y;
            if(relativeYPosition < _width)
                Moved.Raise(1);
            else if (relativeYPosition > Size.Y - _width)
                Moved.Raise(-1);
        }
    }
}
