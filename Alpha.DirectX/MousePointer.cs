using Alpha.DirectX.UI;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX
{
    class MousePointer
    {
        private IContext _context;
        private TexturedRectangle _default;
        private TexturedRectangle _drag;
        public CursorType Type { get; set; }

        public enum CursorType
        {
            Default,
            Drag,
            None
        }
        
        public void Initialize(IContext context)
        {
            _context = context;
            _default = new TexturedRectangle(context, context.TextureManager.Create("default.png", @"Data/MousePointers/"));
            _drag = new TexturedRectangle(context, context.TextureManager.Create("move.png", @"Data/MousePointers/"));
        }
        
        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(_default, _drag);
        }

        public void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            if (Type == CursorType.None) return;
            if (Type == CursorType.Default)
            {
                int drawX = -(_context.ScreenSize.X >> 1) + _context.Input.AbsoluteMousePosition.X;
                int drawY = -(_context.ScreenSize.Y >> 1) + _context.Input.AbsoluteMousePosition.Y;
                Matrix position = Matrix.Translation(drawX, drawY, 0);
                _default.Render(deviceContext, position, viewMatrix, projectionMatrix);
            }
            else if (Type == CursorType.Drag)
            {
                int drawX = -(_context.ScreenSize.X >> 1) + _context.Input.AbsoluteMousePosition.X - _drag.Size.X / 2;
                int drawY = -(_context.ScreenSize.Y >> 1) + _context.Input.AbsoluteMousePosition.Y - _drag.Size.Y / 2;
                Matrix position = Matrix.Translation(drawX, drawY, 0);
                _drag.Render(deviceContext, position, viewMatrix, projectionMatrix);
            }
        }
    }
}
