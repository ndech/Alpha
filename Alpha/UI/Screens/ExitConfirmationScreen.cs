using Alpha.UI.Controls;
using Alpha.UI.Coordinates;
using SharpDX;

namespace Alpha.UI.Screens
{
    class ExitConfirmationScreen : Screen
    {
        public ExitConfirmationScreen(IGame game) : base(game, true)
        {
            Panel panel;
            Panel subPanel;
            Button yesButton;
            Button noButton;
            Register(panel =new Panel(game, new UniRectangle(0, 0, 1.0f, 1.0f), new Color(0, 0, 0, 0.8f)));
            panel.Register(subPanel = new Panel(game, new UniRectangle(new UniScalar(0.5f,-200), new UniScalar(0.5f,-70), 400, 140), Color.LightPink));
            subPanel.Register(yesButton = new Button(game, new UniRectangle(20, 70, 160, 50), "Yes"));
            subPanel.Register(noButton = new Button(game, new UniRectangle(220, 70, 160, 50), "No"));
            subPanel.Register(new Label(game, new UniRectangle(20, 20, 360, 50), "Are you sure you want to quit ?"));

            yesButton.Clicked += game.Exit;
            noButton.Clicked += () => UiManager.DeleteScreen(this);
        }
    }
}
