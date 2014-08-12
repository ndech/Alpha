using Alpha.DirectX.UI;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX
{
    class MousePointer : RenderableComponent
    {
        private TexturedRectangle _rectangle;
        public CursorType Type { get; set; }

        public enum CursorType
        {
            Default,
            None
        }

        public MousePointer(IContext game) 
            : base(game, -5000, 1000, false, true)
        { }

        public override void Initialize()
        {
            Texture texture = Context.TextureManager.Create("default.png", @"Data/MousePointers/");
            _rectangle = new TexturedRectangle(Context, texture.Size, texture);
        }

        public override void Update(double delta)
        { }

        public override void Dispose()
        {
            _rectangle.Dispose();
        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            if (Type == CursorType.None) return;
            int drawX = -(Context.ScreenSize.X >> 1) + Context.Input.AbsoluteMousePosition.X;
            int drawY = -(Context.ScreenSize.Y >> 1) + Context.Input.AbsoluteMousePosition.Y;
            Matrix position = Matrix.Translation(drawX, drawY, 0);
            _rectangle.Render(deviceContext, position, viewMatrix, projectionMatrix);
        }
    }
}
