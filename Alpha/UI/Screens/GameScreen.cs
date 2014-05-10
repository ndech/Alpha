using Alpha.Graphics;
using Alpha.UI.Controls;
using Alpha.UI.Coordinates;
using SharpDX;
using SharpDX.DirectInput;

namespace Alpha.UI.Screens
{
    class GameScreen : Screen
    {
        private readonly IProvinceList _provinceList;
        public GameScreen(IGame game) : base(game, "game_screen")
        {
            _provinceList = game.Services.GetService<IProvinceList>();
            Register(new CalendarWidget(game));
            Button menuButton = Register(new Button(game, "menu", new UniRectangle(new UniScalar(0.5f, -40), 0, 80, 30), "Menu"));
            menuButton.Clicked += (b) => UiManager.AddScreen(new MenuScreen(game));
            Panel provincesPanel = Register(new Panel(game, "provinces_panel", new UniRectangle(0, 30, 400, 400), Color.LawnGreen));
            provincesPanel.Visible = false;
            Button provincesButton = Register(new Button(game, "provinces_button", new UniRectangle(0, 0, 100, 30), "Provinces"));
            provincesButton.Clicked += (b) => { provincesPanel.Visible = !provincesPanel.Visible; };

            for (int i = 0; i < _provinceList.Provinces.Count; i++)
            {
                provincesPanel.Register(new ProvinceButton(game, i, _provinceList.Provinces[i]));

            }
        }
    }
}
