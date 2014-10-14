using System;
using System.Windows.Forms;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Styles;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;
using HorizontalAlignment = Alpha.DirectX.UI.Styles.HorizontalAlignment;

namespace Alpha.DirectX.UI.Controls
{
    class Tooltip : Control
    {
        private TexturedExtensibleRectangle _rectangle;
        private double _delay;
        private double _cooldown;
        private Text.Text _text;
        private readonly Control _associatedControl;
        private Vector2I _position;
        
        public String Text
        {
            set
            {
                _text.Content = value;
                _rectangle.Size = _text.UsedSize;
            }
        }

        public override Vector2I Size
        {
            get { return new Vector2I(); }
        }

        public override Vector2I Position
        {
            get { return _position; }
        }

        public override Vector2I RelativePosition
        {
            get { return _position; }
        }

        protected override void DisposeItem()
        {
            _text.Dispose();
            _rectangle.Dispose();
        }

        public Tooltip(IContext context, String id, Control associatedControl, double delay, string text = null)
            : base(context, id, new UniRectangle())
        {
            _associatedControl = associatedControl;
            _text = context.TextManager.Create("Courrier", 14, "",
                new Vector2I(400, 500), Color.Wheat, HorizontalAlignment.Left, VerticalAlignment.Top, new Padding(8));
            if(text != null)
                _text.Content = text;
            Texture texture = Context.TextureManager.Create("tooltip.png", @"Data/UI/");
            _rectangle = new TexturedExtensibleRectangle(Context, new Vector2I(), texture, 8);
            _delay = delay;
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix,
            Matrix projectionMatrix)
        {
            if(Visible)
                Context.UiManager.VisibleTooltip = this;
        }
        
        public void RenderTooltip(DeviceContext deviceContext,  Matrix viewMatrix, Matrix projectionMatrix)
        {
            int drawX = -(Context.ScreenSize.X >> 1) + _position.X;
            int drawY = -(Context.ScreenSize.Y >> 1) + _position.Y;
            Matrix position = Matrix.Translation(drawX, drawY, 0);
            _rectangle.Render(deviceContext, position, viewMatrix, projectionMatrix);
            _text.Render(deviceContext, position, viewMatrix, projectionMatrix);
        }

        public override string ComponentType
        {
            get { return "tooltip"; }
        }

        public bool IsValid(Vector2I mousePosition)
        {
            return _associatedControl.InBounds(mousePosition);
        }

        protected override void Update(double delta)
        {
            Vector2I mousePosition = Context.UiManager.MousePosition;
            if (!IsValid(mousePosition))
            {
                Visible = false;
                _cooldown = 0;
                return;
            }
            if (!Visible)
            {
                _cooldown += delta;
                if (_cooldown > _delay)
                    Visible = true;
            }
            if (Visible)
            {
                _position = mousePosition + new Vector2I(20, 20);
            }
        }

        public override bool InBoundsAndActive(Vector2I position)
        {
            return false;
        }

        public override void Initialize()
        {

        }
    }
}