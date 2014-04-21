using Alpha.Toolkit.Math;
using Alpha.UI.Controls;
using SharpDX;

namespace Alpha.UI.Screens
{
    class ExitConfirmationScreen : Screen
    {
        public ExitConfirmationScreen(IGame game) : base(game)
        {
            Button yesButton = new Button(game, new Vector2I(200, 50), new Vector2I(350, 500), "Yes");
            Button noButton = new Button(game, new Vector2I(200, 50), new Vector2I(650, 500), "No");

            Register(
                new Panel(game, new Vector2I(600, 300), new Vector2I(300, 300), new Color(0f, 0f, 0f, 0.5f)).Register(
                    new Label(game, new Vector2I(200, 50), new Vector2I(250, 400), "Are you sure you want to quit ?"),
                    yesButton,
                    noButton));

            yesButton.Clicked += game.Exit;
            noButton.Clicked += () => UiManager.DeleteScreen(this);
        }
    }
}
