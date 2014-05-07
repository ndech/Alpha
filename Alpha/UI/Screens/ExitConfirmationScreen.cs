using Alpha.UI.Controls;
using Alpha.UI.Coordinates;
using SharpDX;

namespace Alpha.UI.Screens
{
    class ExitConfirmationScreen : Screen
    {
        public ExitConfirmationScreen(IGame game) : base(game, "exit_confirmation_screen", true)
        {
            Panel overlay = Register(new Panel(game, "exit_confirmation_overlay", new UniRectangle(0, 0, 1.0f, 1.0f), new Color(0, 0, 0, 0.6f)));
            Panel panel = overlay.Register(new Panel(game, "exit_confirmation_panel", new UniRectangle(new UniScalar(0.5f,-200), new UniScalar(0.5f,-70), 400, 140), Color.LightPink));
            Button yesButton = panel.Register(new Button(game, "exit_confirmation_yes", new UniRectangle(20, 70, 160, 50), "Yes"));
            Button noButton = panel.Register(new Button(game,"exit_confirmation_no", new UniRectangle(220, 70, 160, 50), "No"));
            panel.Register(new Label(game, "exit_confirmation_text", new UniRectangle(20, 20, 360, 50), "Are you sure you want to quit ?"));

            yesButton.Clicked += (b) => game.Exit();
            noButton.Clicked += (b) => UiManager.DeleteScreen(this);
        }
    }
}
