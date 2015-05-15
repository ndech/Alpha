using System;
using System.Linq;
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
    class MapTooltip : Control, IStylable<Label, LabelStyle>
    {
        private readonly GameScreen _gameScreen;
        private Zone _hoveredZone;
        private readonly TexturedExtensibleRectangle _rectangle;
        private readonly Text.Text _text;
        private double _delay;
        private double _cooldown;
        private bool _isVisible;

        private Vector2I _position;

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

        public MapTooltip(IContext context, GameScreen gameScreen) : base(context, "map_tooltip", new UniRectangle())
        {
            _gameScreen = gameScreen;
            _rectangle = new TexturedExtensibleRectangle(context, new Vector2I(), context.TextureManager.Create("tooltip.png", "Data/UI/"), 8);
            LabelStyle style = Context.UiManager.StyleManager.GetStyle(this);
            _text = Context.TextManager.Create(style.Font, style.FontSize, "", new Vector2I(400, 600), style.TextColor,
                style.HorizontalAlignment, style.VerticalAlignment, style.Padding);
            _delay = .5f;
        }

        protected override void Update(double delta)
        {
            Zone currentZone;
            Vector2I mousePosition = Context.UiManager.MousePosition;
            if (mousePosition.X >= 0
                   && mousePosition.X < _gameScreen.Size.X
                   && mousePosition.Y >= 0
                   && mousePosition.Y < 0 + _gameScreen.Size.Y)
                currentZone = _gameScreen.HoveredZone();
            else
                currentZone = null;
            if (currentZone != _hoveredZone) //User has left the previous province
            {
                _hoveredZone = currentZone;
                _cooldown = 0;
                _isVisible = false;
                return;
            }
            if(_hoveredZone == null) return;
            if (!_isVisible)
            {
                _cooldown += delta;
                if (_cooldown > _delay)
                    _isVisible = true;
            }
            if (_isVisible)
            {
                UpdateContent();
                _position = Context.UiManager.MousePosition + new Vector2I(20, 25);
                _position -= new Vector2I(Math.Max(0, _position.X + _rectangle.Size.X + 10 - Context.ScreenSize.X),0);
            }
        }

        private void UpdateContent()
        {
            string text;
            if (_hoveredZone.Province is LandProvince)
            {
                LandProvince province = _hoveredZone.Province as LandProvince;
                text = "Province of " + province.Name;
                text += Environment.NewLine + province.Capital.Population.Value + " pop ";
                text += Environment.NewLine + province.Surface + " km²";
                text += Environment.NewLine + Text.Text.EnphasizeAsPercentage(province.Capital.Population.YearlyGrowth);
                text += " (" + Text.Text.Enphasize(province.Capital.Population.LastDayVariation) + ")";
                text += Environment.NewLine + "Resources : " +string.Join(", ", province.Resources.Select(r=>r.Type.Name).OrderBy(s=>s));
            }
            else
                text = _hoveredZone.Province.Name;
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
            get { return "label"; }
        }

        public UiComponent Component
        {
            get { return this; }
        }

        protected override void DisposeItem()
        {
            DisposeHelper.DisposeAndSetToNull(_text, _rectangle);
        }

        public override void Initialize()
        {

        }
    }
}
