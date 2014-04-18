using Alpha.Toolkit.Math;
using Alpha.UI.Controls;

namespace Alpha.UI
{
    class MenuScreen : Screen
    {
        private readonly Button _debugButton;
        private readonly Button _saveButton;
        private readonly Button _loadButton;
        private readonly Button _exitButton;
        public MenuScreen(IGame game)
            :base(game)
        {
            _components.Add(_saveButton = new Button(game, "Save", 300, 50) { Position = new Vector2I(100, 100) });
            _components.Add(_loadButton = new Button(game, "Load", 300, 50) { Position = new Vector2I(100, 200) });
            _components.Add(_debugButton = new Button(game, "Debug", 300, 50) { Position = new Vector2I(100, 330) });
            _components.Add(_exitButton = new Button(game, "Exit", 300, 50) { Position = new Vector2I(100, 400) });

            _components.Add(new Tooltip(game, _exitButton, 1d, "Return to desktop"));
            _components.Add(new Tooltip(game, _debugButton, 1d, "This button ain't [red]doing anything.[-]\nAt all.\nClicking on it will cost you [yellow]20[-] [gold] anyway."));

            _exitButton.Clicked += game.Exit;
        }
    }
}
