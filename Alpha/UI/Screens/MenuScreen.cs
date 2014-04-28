using Alpha.UI.Controls;
using Alpha.UI.Coordinates;
using SharpDX;

namespace Alpha.UI.Screens
{
    class MenuScreen : Screen
    {
        private readonly Button _calendarButton;
        private readonly Button _provincesButton;

        public MenuScreen(IGame game)
            :base(game, true)
        {
            Panel panel = Register(new Panel(game, new UniRectangle(new UniScalar(.5f, -150), new UniScalar(.5f, -172), 300, 345), Color.Black));
            panel.Register(new Label(game, new UniRectangle(0.05f, 15, 0.9f, 40), "Menu"));
            panel.Register(_calendarButton = new Button(game, new UniRectangle(0.05f, 70, 0.9f, 40), "Calendar"));
            panel.Register(_provincesButton = new Button(game, new UniRectangle(0.05f, 125, 0.9f, 40), "Provinces"));
            Button saveButton = panel.Register(new Button(game, new UniRectangle(0.05f, 180, 0.9f, 40), "Save"));
            Button exitButton = panel.Register(new Button(game, new UniRectangle(0.05f, 235, 0.9f, 40), "Exit to desktop"));
            Button cancelButton = panel.Register(new Button(game, new UniRectangle(0.25f, 290, 0.5f, 40), "Cancel"));

            exitButton.Clicked += () => UiManager.AddScreen(new ExitConfirmationScreen(game));
            cancelButton.Clicked += () => UiManager.DeleteScreen(this);
            /*Register(new Tooltip(game, _calendarButton, 1d, "Show the calendar"));
            Register(new Tooltip(game, _provincesButton, 1d, "Show the provinces details"));
            Register(new Tooltip(game, _exitButton, 1d, "Return to desktop"));
            Register(new Tooltip(game, _debugButton, 1d, "This button ain't [red]doing anything.[-]\nAt all.\nClicking on it will cost you [yellow] 20[-] [gold] anyway."));
            */
        }
    }
}
