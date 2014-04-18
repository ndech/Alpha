using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.Graphics
{
    class MousePointer : RenderableGameComponent
    {
        private Texture _texture;
        private IInput _input;
        private TexturedRectangle _rectangle;
         
        public MousePointer(IGame game) 
            : base(game, -5000, false, true)
        {

        }

        public override void Initialize()
        {
            IRenderer renderer = Game.Services.GetService<IRenderer>();
            _texture = renderer.TextureManager.Create("default.png", @"Data/MousePointers/");
            _input = Game.Services.GetService<IInput>();
            _rectangle = new TexturedRectangle(renderer, _input.AbsoluteMousePosition, new Vector2I(_texture.Width, _texture.Height), _texture);
        }

        public override void Update(double delta)
        {
            _rectangle.Position = _input.AbsoluteMousePosition;
        }

        public override void Dispose()
        {
            _rectangle.Dispose();
        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _rectangle.Render(deviceContext, Matrix.Identity, viewMatrix, projectionMatrix);
        }
    }
}
