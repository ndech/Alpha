using System.Windows.Input;
using Alpha.DirectX.UI.Controls;
using Alpha.DirectX.UI.Coordinates;

namespace Alpha.DirectX.UI.Screens
{
    class WorldParametersScreen : Screen
    {
        public WorldParametersScreen(IContext context) : base(context, "world_parameters_screen")
        {
            Button validateButton = Register(new Button(context, "ok_button",
                new UniRectangle(new UniScalar(1.0f, -300), new UniScalar(1.0f, -120), 250, 70),"Generate world"));
            validateButton.Shortcut = Key.Back;
            validateButton.Clicked += b =>
            {
                Context.Game.GenerateWorldEvent.Set();
                Context.UiManager.SetScreen(new WorldGeneratingScreen(Context));
            };

            Button newValidateButton = Register(new Button(context, "ok_button",
                new UniRectangle(new UniScalar(1.0f, -600), new UniScalar(1.0f, -120), 250, 70), "Generate spherical world"));
            newValidateButton.Shortcut = Key.Enter;
            newValidateButton.Clicked += b =>
            {
                Context.UiManager.SetScreen(new SphericalWorldScreen(Context));
            };
        }
    }
}
