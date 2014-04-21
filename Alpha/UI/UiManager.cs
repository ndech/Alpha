using System.Collections.Generic;
using Alpha.UI.Screens;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI
{
    interface IUiManager : IService
    {
        void AddScreen(Screen screen);
        void DeleteScreen(Screen screen);
    }
    class UiManager : RenderableGameComponent, IUiManager
    {
        private readonly List<Screen> _activeScreens; 
        public UiManager(IGame game) 
            : base(game, 1000, false, true)
        {
            _activeScreens = new List<Screen>();
        }

        public override void Initialize()
        {
            _activeScreens.Add(new MenuScreen(Game));
        }

        public override void Update(double delta)
        {
            foreach (Screen screen in _activeScreens)
                screen.Update(delta);
        }


        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            for(int i = _activeScreens.Count - 1 ; i>=0; i--)
                _activeScreens[i].Render(deviceContext, viewMatrix, projectionMatrix);
        }

        public override void Dispose()
        {

        }

        public void RegisterAsService()
        {
            Game.Services.AddService<IUiManager>(this);
        }

        public void AddScreen(Screen screen)
        {
            _activeScreens.Insert(0, screen);
        }

        public void DeleteScreen(Screen screen)
        {
            _activeScreens.Remove(screen);
        }
    }
}
