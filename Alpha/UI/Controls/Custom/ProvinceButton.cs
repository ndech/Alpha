using Alpha.Graphics;
using Alpha.UI.Coordinates;

namespace Alpha.UI.Controls.Custom
{
    class ProvinceButton : Button
    {
        private readonly Territory _territory;
        public ProvinceButton(IGame game, int position, Territory territory)
            : base(game, "province_button",new UniRectangle(0.05f, 40 * position + 10, 0.9f, 30), territory.Name)
        {
            _territory = territory;
        }

        public override void Initialize()
        {
            base.Initialize();
            IRenderer renderer = Game.Services.Get<IRenderer>();
            Register(new Icon(Game, "growth_icon", new UniRectangle(new UniScalar(1.0f, -30), 5, 20, 20),
                renderer.TextureManager.Create("PauseIcon.png", "Data/UI/Icons/")));
        }

        protected override void Update(double delta)
        {
            Text = _territory.Name;
        }
    }
}