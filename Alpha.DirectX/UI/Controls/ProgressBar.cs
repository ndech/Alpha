using Alpha.DirectX.UI.Coordinates;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Controls
{
    class ProgressBar : Control
    {
        private readonly Color _color;
        private PlainRectangle _rectangle;
        
        public ProgressBar(IContext context, string id, UniRectangle coordinates, Color color) : base(context, id, coordinates)
        {
            _color = color;
        }

        public override string ComponentType
        {
            get { return "progressbar"; }
        }

        protected override void DisposeItem()
        {
            DisposeHelper.DisposeAndSetToNull(_rectangle);
        }

        public override void Initialize()
        {
            _rectangle = new PlainRectangle(Context, this.Size, _color);
        }

        public void SetValues(int min, int max, int current)
        {
            double factor = ((double)(current - min)/(max - min));
            _rectangle.Size = new Vector2I((int)(Size.X*factor), Size.Y);
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _rectangle.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
        }
    }
}
