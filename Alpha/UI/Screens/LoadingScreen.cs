using System;
using Alpha.UI.Controls;
using Alpha.UI.Coordinates;
using SharpDX;

namespace Alpha.UI.Screens
{
    class LoadingScreen : Screen
    {
        private readonly Label _label;
        public String LoadedContent { set { _label.Text = "Loading " + value.ToLower() + "."; } }
        public LoadingScreen(IGame game) : base(game, "load_screen", true)
        {
            Panel overlay = Register(new Panel(game, "loading_overlay", new UniRectangle(0, 0, 1.0f, 1.0f), new Color(0, 0, 0, 0.6f)));
            Panel panel = overlay.Register(new Panel(game, "loading_panel", new UniRectangle(new UniScalar(.5f, -200), new UniScalar(.5f, -50), 400, 100), Color.Yellow));
            _label = panel.Register(new Label(game, "loading_label", new UniRectangle(0, 0, 1.0f, 1.0f), "Loading..."));
        }
    }
}
