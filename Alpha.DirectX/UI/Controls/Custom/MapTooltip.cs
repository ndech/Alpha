using System;
using System.Globalization;
using Alpha.Core.Provinces;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Screens;
using Alpha.DirectX.UI.Styles;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class MapTooltip : Control
    {
        private readonly GameScreen _gameScreen;
        private Province _hoveredProvince;
        private PlainRectangle _rectangle;
        private Text.Text _text;
        private double _delay;
        private double _cooldown;
        private bool _isVisible;

        private Vector2I _size;
        private Vector2I _position;

        public override Vector2I Size
        {
            get { return _size; }
        }

        public override Vector2I Position
        {
            get { return _position; }
        }

        public override Vector2I RelativePosition
        {
            get { return _position; }
        }

        public MapTooltip(IContext context, GameScreen gameScreen) : base(context, "map_tooltip", new UniRectangle())
        {
            _gameScreen = gameScreen;
            _size = new Vector2I(40, 40);
            _rectangle = new PlainRectangle(context, _size, Color.Tomato);
            _text = context.TextManager.Create("Courrier", 14, "",
                new Vector2I(300, 500), Color.Wheat, HorizontalAlignment.Left, VerticalAlignment.Top, new Padding(5));
            _delay = .5f;
        }

        protected override void Update(double delta)
        {
            Province currentProvince = _gameScreen.HoveredProvince();
            if (currentProvince != _hoveredProvince) //User has left the previous province
            {
                _hoveredProvince = currentProvince;
                _cooldown = 0;
                _isVisible = false;
                return;
            }
            if(_hoveredProvince == null) return;
            if (!_isVisible)
            {
                _cooldown += delta;
                if (_cooldown > _delay)
                    _isVisible = true;
            }
            if (_isVisible)
            {
                _position = Context.UiManager.MousePosition + new Vector2I(20, 20);
                UpdateContent();
            }
        }

        private void UpdateContent()
        {
            string text = _hoveredProvince.Id;
            if(_hoveredProvince is LandProvince)
                text += Environment.NewLine + (_hoveredProvince as LandProvince).BaseTax + "g/year";
            _text.Content = text;
            _rectangle.Size = _text.UsedSize;
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _rectangle.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
            _text.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
        }

        public override bool IsVisible()
        {
            return _isVisible;
        }

        public override string ComponentType
        {
            get { return "tooltip"; }
        }

        protected override void DisposeItem()
        {

        }

        public override void Initialize()
        {

        }
    }
}
