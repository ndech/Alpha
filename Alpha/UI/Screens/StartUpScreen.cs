using System;
using Alpha.UI.Controls;
using Alpha.UI.Coordinates;
using SharpDX;

namespace Alpha.UI.Screens
{
    class StartUpScreen : Screen
    {
        private Label _label;
        public StartUpScreen(IGame game) : base(game, "initializing_screen", false)
        {
            Panel panel = Register(new Panel(game, "loading_panel", new UniRectangle(new UniScalar(.5f, -200), 0.8f, 400, 50), Color.Gray));
            _label = panel.Register(new Label(game, "loading_label", new UniRectangle(0, 0, 1.0f, 1.0f), "Loading..."));
        }

        public String Text { set { _label.Text = value; } }
    }
}
