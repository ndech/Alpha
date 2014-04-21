using Alpha.Toolkit.Math;
using Alpha.UI.Controls;

namespace Alpha.UI.Screens
{
    class MenuScreen : Screen
    {
        private readonly Button _debugButton;
        private readonly Button _calendarButton;
        private readonly Button _provincesButton;
        private readonly Button _exitButton;
        public MenuScreen(IGame game)
            :base(game)
        {
            Register(_calendarButton = new Button(game, new Vector2I(300, 50), new Vector2I(100, 100), "Calendar"));
            Register(_provincesButton = new Button(game, new Vector2I(300, 50), new Vector2I(100, 200), "Provinces"));
            Register(_debugButton = new Button(game, new Vector2I(300, 50),  new Vector2I(100, 300), "Debug"));
            Register(_exitButton = new Button(game,new Vector2I(300, 50),  new Vector2I(100, 400), "Exit"));

            Register(new Tooltip(game, _calendarButton, 1d, "Show the calendar"));
            Register(new Tooltip(game, _provincesButton, 1d, "Show the provinces details"));
            Register(new Tooltip(game, _exitButton, 1d, "Return to desktop"));
            Register(new Tooltip(game, _debugButton, 1d, "This button ain't [red]doing anything.[-]\nAt all.\nClicking on it will cost you [yellow] 20[-] [gold] anyway."));

            _provincesButton.Clicked +=
                () => UiManager.AddScreen(new ProvincesScreen(game));
            _exitButton.Clicked +=
                () => UiManager.AddScreen(new ExitConfirmationScreen(game));
            _calendarButton.Clicked +=
                () => UiManager.AddScreen(new CalendarScreen(game));
        }
    }
}
