using Alpha.Toolkit.Math;

namespace Alpha.UI
{
    abstract class Screen : UiComponent
    {
        protected readonly IUiManager UiManager;
        protected Screen(IGame game) : base(game, new Vector2I(0,0), new Vector2I(0,0))
        {
            UiManager = game.Services.GetService<IUiManager>();
        }
    }
}
