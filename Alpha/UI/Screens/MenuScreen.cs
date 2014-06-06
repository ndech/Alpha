using System.Windows.Input;
using Alpha.UI.Controls;
using Alpha.UI.Coordinates;
using SharpDX;

namespace Alpha.UI.Screens
{
    class MenuScreen : Screen
    {
        private readonly Button _calendarButton;
        private readonly Button _loadButton;

        public MenuScreen(IGame game)
            : base(game, "menu_screen", true)
        {
            Panel overlay = Register(new Panel(game, "menu_overlay", new UniRectangle(0, 0, 1.0f, 1.0f), new Color(0, 0, 0, 0.6f)));
            Panel panel = overlay.Register(new Panel(game, "menu_panel", new UniRectangle(new UniScalar(.5f, -150), new UniScalar(.5f, -172), 300, 345), Color.Black));
            panel.Register(new Label(game, "menu_label", new UniRectangle(0.05f, 15, 0.9f, 40), "Menu"));
            panel.Register(_calendarButton = new Button(game, "menu_save", new UniRectangle(0.05f, 70, 0.9f, 40), "Calendar"));
            panel.Register(_loadButton = new Button(game, "menu_load", new UniRectangle(0.05f, 125, 0.9f, 40), "Load"));
            Button saveButton = panel.Register(new Button(game, "menu_options", new UniRectangle(0.05f, 180, 0.9f, 40), "Save"));
            Button exitButton = panel.Register(new Button(game, "menu_exit", new UniRectangle(0.05f, 235, 0.9f, 40), "Exit to desktop"));
            Button cancelButton = panel.Register(new Button(game, "menu_cancel", new UniRectangle(0.25f, 290, 0.5f, 40), "Cancel"));
            cancelButton.Shortcut = Key.Escape;

            _loadButton.Clicked += (b) => UiManager.AddScreen(new LoadScreen(game));
            exitButton.Clicked += (b) => UiManager.AddScreen(new ExitConfirmationScreen(game));
            saveButton.Clicked += (b) => UiManager.AddScreen(new SaveScreen(game));
            cancelButton.Clicked += (b) => UiManager.DeleteScreen(this);
            Register(new Tooltip(game, "calendar_tooltip", _calendarButton, 1d, "Show the calendar"));
            Register(new Tooltip(game, "provinces_tooltip", _loadButton, 1d, "Load a previously saved game"));
            Register(new Tooltip(game, "exit_tooltip", exitButton, 1d, "Return to desktop"));
            Register(new Tooltip(game, "cancel_tooltip", cancelButton, 1d, "This button ain't [Red]doing anything.[-]\nAt all.\nClicking on it will cost you [Yellow] 20[-] [gold] anyway."));
        }
    }
}
