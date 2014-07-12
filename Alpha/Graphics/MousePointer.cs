using System;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.Graphics
{
    interface IMousePointer : IService
    {
        MousePointer.CursorType Type { set; }
    }
    class MousePointer : RenderableGameComponent, IMousePointer
    {
        private Texture _texture;
        private IInput _input;
        private TexturedRectangle _rectangle;
        private Vector2I _screenSize;
        public CursorType Type { get; set; }

        public enum CursorType
        {
            Default,
            None
        }

        public MousePointer(IGame game) 
            : base(game, -5000, false, true)
        {

        }

        public override void Initialize(Action<string> feedback)
        {
            IRenderer renderer = Game.Services.Get<IRenderer>();
            _texture = renderer.TextureManager.Create("default.png", @"Data/MousePointers/");
            _input = Game.Services.Get<IInput>();
            _screenSize = renderer.ScreenSize;
            _rectangle = new TexturedRectangle(renderer, new Vector2I(_texture.Width, _texture.Height), _texture);
        }

        public override void Update(double delta)
        {
        }

        public override void Dispose()
        {
            _rectangle.Dispose();
        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            if (Type == CursorType.None) return;
            int drawX = -(_screenSize.X >> 1) + _input.AbsoluteMousePosition.X;
            int drawY = -(_screenSize.Y >> 1) + _input.AbsoluteMousePosition.Y;
            Matrix position = Matrix.Translation(drawX, drawY, 0);
            _rectangle.Render(deviceContext, position, viewMatrix, projectionMatrix);
        }

        public void RegisterAsService()
        {
            Game.Services.Register<IMousePointer>(this);
        }
    }
}
