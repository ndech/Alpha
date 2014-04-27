using System.Collections.Generic;
using System.Drawing;
using Alpha.Graphics;
using Alpha.Toolkit.Math;
using Alpha.UI.Screens;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI
{
    interface IUiManager : IService
    {
        void AddScreen(Screen screen);
        void DeleteScreen(Screen screen);
        Vector2I ScreenSize { get; }
    }
    class UiManager : RenderableGameComponent, IUiManager
    {
        private readonly List<Screen> _activeScreens;
        public Vector2I ScreenSize { get; private set; }
        public UiManager(IGame game) 
            : base(game, 1000, false, true)
        {
            _activeScreens = new List<Screen>();
        }

        private void OnMouseMoved(Vector2I position)
        {
            if (_activeScreens.Count > 0)
                _activeScreens[0].OnMouseMoved(position);
        }

        public override void Initialize()
        {
            IInput input = Game.Services.GetService<IInput>();
            input.MouseMoved += OnMouseMoved;
            input.MouseClicked += OnMouseClicked;
            input.MouseReleased += OnMouseReleased;
            ScreenSize = Game.Services.GetService<IRenderer>().ScreenSize;
            AddScreen(new GameScreen(Game));
            AddScreen(new MenuScreen(Game));
        }

        private void OnMouseClicked(Vector2I position, int button)
        {
            if (_activeScreens.Count > 0)
                _activeScreens[0].OnMouseClicked(position, button);
        }

        private void OnMouseReleased(Vector2I position, int button)
        {
            if (_activeScreens.Count > 0)
                _activeScreens[0].OnMouseReleased(position, button);
        }

        public override void Update(double delta)
        {
            foreach (Screen screen in _activeScreens)
                screen.UpdateTree(delta);
        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            int i;
            for(i = 0; i< _activeScreens.Count; i++)
                if (!_activeScreens[i].Transparent)
                    break;
            for (int j = i; j >= 0; j--)
                _activeScreens[j].RenderTree(deviceContext, Matrix.Identity, viewMatrix, projectionMatrix);
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
            if(_activeScreens.Count > 0)
                _activeScreens[0].Unregister();
            _activeScreens.Insert(0, screen);
        }

        public void DeleteScreen(Screen screen)
        {
            _activeScreens.Remove(screen);
        }
    }
}
