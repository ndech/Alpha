using System;
using System.Windows.Input;
using Alpha.UI.Controls;
using Alpha.UI.Coordinates;
using SharpDX;
using Button = Alpha.UI.Controls.Button;
using Panel = Alpha.UI.Controls.Panel;

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
            menuButton.Shortcut = Key.Escape;

            Panel provincesPanel = Register(new Panel(game, "provinces_panel", new UniRectangle(0, 30, 500, 400), Color.LawnGreen));
            provincesPanel.Visible = false;

            Panel economyPanel = Register(new Panel(game, "economy_panel", new UniRectangle(0, 30, 500, 400), Color.BlueViolet));
            economyPanel.Visible = false;


            Button provincesButton = Register(new Button(game, "provinces_button", new UniRectangle(0, 0, 100, 30), "Provinces"));
            provincesButton.Clicked += (b) =>
            {
                provincesPanel.Visible = !provincesPanel.Visible;
                economyPanel.Visible = false;
            };

            Button economyButton = Register(new Button(game, "economy_button", new UniRectangle(100, 0, 100, 30), "Economy"));
            economyButton.Clicked += (b) =>
            {
                economyPanel.Visible = !economyPanel.Visible;
                provincesPanel.Visible = false;
            };

            Label taxLabel = economyPanel.Register(new Label(game, "tax_label", new UniRectangle(20, 50, 460, 50), "Tax rate : 0 %"));
            Slider taxSlider = economyPanel.Register(new Slider(game, "tax_slider", new UniRectangle(20, 100, 460, 41), 0.0f, 100.0f, 100));
            taxSlider.Changed += () => { taxLabel.Text = String.Format("Tax rate : {0:0} %", taxSlider.Value); };

            Label spendingLabel = economyPanel.Register(new Label(game, "spending_label", new UniRectangle(20, 150, 460, 50), "Spending rate : 0 %"));
            Slider spendingSlider = economyPanel.Register(new Slider(game, "spending_slider", new UniRectangle(20, 200, 460, 41), 0.0f, 100.0f, 100));
            spendingSlider.Changed += () => { spendingLabel.Text = String.Format("Spending rate : {0:0} %", spendingSlider.Value); };

            for (int i = 0; i < _provinceList.Provinces.Count; i++)
                provincesPanel.Register(new ProvinceButton(game, i, _provinceList.Provinces[i]));
        }
    }
}
