using Alpha.DirectX.UI;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX
{
    class MousePointer
    {
        private IContext _context;
        private TexturedRectangle _rectangle;
        public CursorType Type { get; set; }

        public enum CursorType
        {
            Default,
            None
        }
        
        public void Initialize(IContext context)
        {
            _context = context;
            Texture texture = context.TextureManager.Create("default.png", @"Data/MousePointers/");
            _rectangle = new TexturedRectangle(context, texture.Size, texture);
        }
        
        public void Dispose()
        {
            _rectangle.Dispose();
        }

        public void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            if (Type == CursorType.None) return;
            int drawX = -(_context.ScreenSize.X >> 1) + _context.Input.AbsoluteMousePosition.X;
            int drawY = -(_context.ScreenSize.Y >> 1) + _context.Input.AbsoluteMousePosition.Y;
            Matrix position = Matrix.Translation(drawX, drawY, 0);
            _rectangle.Render(deviceContext, position, viewMatrix, projectionMatrix);
        }
    }
}
