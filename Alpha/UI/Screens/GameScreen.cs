using System;
using System.Collections.Generic;
using System.Windows.Input;
using Alpha.UI.Controls;
using Alpha.UI.Controls.Custom;
using Alpha.UI.Coordinates;
using SharpDX;
using Button = Alpha.UI.Controls.Button;
using Panel = Alpha.UI.Controls.Panel;

namespace Alpha.UI.Screens
{
    class GameScreen : Screen
    {
        private readonly IList<Territory> _demesne;
        private Realm _playerRealm;
        private Terrain _terrain;
        public GameScreen(IGame game) : base(game, "game_screen")
        {
            _playerRealm = game.Services.GetService<IRealmManager>().PlayerRealm;
            _demesne = _playerRealm.Demesne;
            Register(_terrain = new Terrain(game));
            Register(new CalendarWidget(game));

            Button menuButton = Register(new Button(game, "menu", new UniRectangle(new UniScalar(0.5f, -40), 0, 80, 30), "Menu"));
            menuButton.Clicked += (b) => UiManager.AddScreen(new MenuScreen(game));
            menuButton.Shortcut = Key.Escape;
            Button regenerateButton =
                Register(new Button(game, "regenerate", new UniRectangle(new UniScalar(0.5f, -220), 0, 160, 30), "Regenerate map"));
            regenerateButton.Clicked += (b) => _terrain.Regenerate();
            Panel provincesPanel = Register(new Panel(game, "provinces_panel", new UniRectangle(0, 90, 500, 400), Color.LawnGreen));
            provincesPanel.Visible = false;

            Panel economyPanel = Register(new Panel(game, "economy_panel", new UniRectangle(0, 90, 500, 600), Color.BlueViolet));
            economyPanel.Visible = false;
            
            Button provincesButton = Register(new Button(game, "provinces_button", new UniRectangle(0, 0, 100, 90), "Provinces"));
            provincesButton.Clicked += (b) =>
            {
                provincesPanel.Visible = !provincesPanel.Visible;
                economyPanel.Visible = false;
            };

            Button economyButton = Register(new Button(game, "economy_button", new UniRectangle(100, 0, 100, 90), "Economy"));
            economyButton.Clicked += (b) =>
            {
                economyPanel.Visible = !economyPanel.Visible;
                provincesPanel.Visible = false;
            };
            economyButton.Register(new DynamicLabel(game, "economy_balance", new UniRectangle(0, 30, 1.0f, 30),
                () => _playerRealm.Treasury.ToString("N2")) { Overlay = true });
            economyButton.Register(new DynamicLabel(game, "economy_revenue", new UniRectangle(0, 60, 1.0f, 30),
                () => _playerRealm.Revenue.ToString("N2")) { Overlay = true });


            Label taxLabel = economyPanel.Register(new Label(game, "tax_label", new UniRectangle(20, 50, 460, 50), "Tax rate : 0 %"));
            Slider taxSlider = economyPanel.Register(new Slider(game, "tax_slider", new UniRectangle(20, 100, 460, 41), 0.0f, 100.0f, 100));
            taxSlider.Changed += () =>
            {
                taxLabel.Text = String.Format("Tax rate : {0:0} %", taxSlider.Value);
                _playerRealm.TaxRate = taxSlider.Value/100;
            };

            Label vassalTaxLabel = economyPanel.Register(new Label(game, "vassal_tax_label", new UniRectangle(20, 150, 460, 50), "Vassal tax rate : 0 %"));
            Slider vassalTaxSlider = economyPanel.Register(new Slider(game, "vassal_tax_slider", new UniRectangle(20, 200, 460, 41), 0.0f, 100.0f, 100));
            vassalTaxSlider.Changed += () =>
            {
                vassalTaxLabel.Text = String.Format("Vassal tax rate : {0:0} %", vassalTaxSlider.Value);
                _playerRealm.VassalTaxRate = vassalTaxSlider.Value/100;
            };

            Label spendingLabel = economyPanel.Register(new Label(game, "spending_label", new UniRectangle(20, 250, 460, 50), "Spending rate : 0 %"));
            Slider spendingSlider = economyPanel.Register(new Slider(game, "spending_slider", new UniRectangle(20, 300, 460, 41), 0.0f, 100.0f, 100));
            spendingSlider.Changed += () =>
            {
                spendingLabel.Text = String.Format("Spending rate : {0:0} %", spendingSlider.Value);
                _playerRealm.SpendingRate = spendingSlider.Value/100;
            };

            economyPanel.Register(new AccountLine(game, "tax_income", new UniRectangle(20, 350, 460, 50), "Tax income", () => _playerRealm.TaxIncome));
            economyPanel.Register(new AccountLine(game, "vassal_income", new UniRectangle(20, 390, 460, 50), "Vassals income", () => _playerRealm.VassalsIncome));
            economyPanel.Register(new AccountLine(game, "spending", new UniRectangle(20, 430, 460, 50), "Spendings", () => _playerRealm.Spending, AccountLine.ValueType.Spending));
            economyPanel.Register(new AccountLine(game, "revenue", new UniRectangle(20, 470, 460, 50), "Revenue", () => _playerRealm.Revenue, AccountLine.ValueType.Revenue));
            
            for (int i = 0; i < _demesne.Count; i++)
                provincesPanel.Register(new ProvinceButton(game, i, _demesne[i]));
            Register(new DebugConsoleWidget(game));
        }
    }
}