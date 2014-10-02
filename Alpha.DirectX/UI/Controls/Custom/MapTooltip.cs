using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Screens;
using Alpha.Toolkit.Math;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class MapTooltip : Control
    {
        private readonly Screen _gameScreen;
        private double _delay;
        private double _cooldown;
        private bool _isVisible;

        public MapTooltip(IContext context, Screen gameScreen) : base(context, "map_tooltip", new UniRectangle())
        {
            _gameScreen = gameScreen;
        }

        protected override void Update(double delta)
        {
            //if (!_associatedControl.Hovered)
            //{
            //    _isVisible = false;
            //    _cooldown = 0;
            //    return;
            //}
            //if (!_isVisible)
            //{
            //    _cooldown += delta;
            //    if (_cooldown > _delay)
            //        _isVisible = true;
            //}
            //if (_isVisible)
            //{
            //    _position = Context.UiManager.MousePosition + new Vector2I(20, 20);
            //}
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
