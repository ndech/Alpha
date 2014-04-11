using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI
{
    class UiManager : RenderableGameComponent
    {
        private Screen _activeScreen;
        private readonly List<Screen> _screens; 
        public UiManager(IGame game) 
            : base(game, 1000, false, true)
        {
            _screens = new List<Screen>();
        }

        public override void Initialize()
        {
            _activeScreen = new MenuScreen(Game);
            _screens.Add(_activeScreen);
        }

        public override void Update(double delta)
        {
            _activeScreen.Update(delta);
        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _activeScreen.Render(deviceContext, viewMatrix, projectionMatrix);
        }

        public override void Dispose()
        {

        }
    }
}
