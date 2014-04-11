using Alpha.Toolkit.Math;
using Alpha.UI.Controls;

namespace Alpha.UI
{
    class MenuScreen : Screen
    {
        private readonly Button _debugButton;
        private readonly Button _saveButton;
        private readonly Button _loadButton;
        public MenuScreen(IGame game)
            :base(game)
        {
            _saveButton = new Button(game, "Save", 300, 50) {Position = new Vector2I(100, 100)};
            _loadButton = new Button(game, "Load", 300, 50) { Position = new Vector2I(100, 200) };
            _debugButton = new Button(game, "Debug", 300, 50) { Position = new Vector2I(100, 300) };

            _components.Add(_saveButton);
            _components.Add(_loadButton);
            _components.Add(_debugButton);
        }

        public override void Update(double delta)
        {
            Vector2I mousePosition = _game.Services.GetService<IInput>().AbsoluteMousePosition();
            _debugButton.Text = mousePosition.ToString();
            _debugButton.OnMouseMoved(mousePosition);
            _saveButton.OnMouseMoved(mousePosition);
            _loadButton.OnMouseMoved(mousePosition);
        }
    }
}
