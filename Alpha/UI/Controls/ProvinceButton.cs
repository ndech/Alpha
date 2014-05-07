using Alpha.Graphics;
using Alpha.UI.Coordinates;

namespace Alpha.UI.Controls
{
    class ProvinceButton : Button
    {
        private readonly Province _province;
        public ProvinceButton(IGame game, int position, Province province)
            : base(game, "province_button",new UniRectangle(0.05f, 40 * position + 10, 0.9f, 30), province.ToString())
        {
            _province = province;
        }

        public override void Initialize()
        {
            base.Initialize();
            IRenderer renderer = Game.Services.GetService<IRenderer>();
            Register(new Icon(Game, "growth_icon", new UniRectangle(new UniScalar(1.0f, -30), 5, 20, 20),
                renderer.TextureManager.Create("PauseIcon.png", "Data/UI/Icons/")));
        }

        protected override void Update(double delta)
        {
            Text = _province.ToString();
        }
    }
}