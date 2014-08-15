using System;
using Alpha.DirectX.UI.Controls;
using Alpha.DirectX.UI.Controls.Custom;
using Alpha.DirectX.UI.Coordinates;
using SharpDX;

namespace Alpha.DirectX.UI.Screens
{
    class WorldGeneratingScreen : Screen
    {
        private readonly Label _label;
        public WorldGeneratingScreen(IContext context) : base(context, "world_generating_screen", false)
        {
            _label = Register(new Panel(context, "loading_panel", new UniRectangle(new UniScalar(.5f, -200), 0.7f, 400, 50), Color.Gray))
                    .Register(new Label(context, "loading_label", new UniRectangle(0, 0, 1.0f, 1.0f), "Loading..."));
            Register(new HintPanel(context,
                new UniRectangle(new UniScalar(.5f, -400), new UniScalar(0.7f, 50), 800, 100)));
        }

        protected override void Update(double delta)
        {
            base.Update(delta);
            if (Context.Game.IsWorldGenerationDone)
            {
                Context.UiManager.AddScreen(new GameScreen(Context));
                Context.UiManager.DeleteScreen(this);
                //Context.UiManager.SetScreen(new GameScreen(Context));
            }
            _label.Text = Context.Game.LoadingMessage;
        }
    }
}
