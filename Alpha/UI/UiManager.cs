using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Xml;
using Alpha.Graphics;
using Alpha.Toolkit.Math;
using Alpha.UI.Screens;
using Alpha.UI.Styles;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI
{
    interface IUiManager : IService
    {
        void AddScreen(Screen screen);
        void DeleteScreen(Screen screen);
        Vector2I ScreenSize { get; }
        StyleManager StyleManager { get; }
        Vector2I MousePosition { get; set; }
        bool IsKeyPressed(Key key);
        void RecalculateActiveComponents();

        bool IsAnyKeyPressed(params Key[] keys);
    }
    class UiManager : RenderableGameComponent, IUiManager, ISavable
    {
        private readonly List<Screen> _activeScreens;
        public StyleManager StyleManager { get; private set; }
        public Vector2I MousePosition { get; set; }
        private IInput _input;
        public bool IsKeyPressed(Key key)
        {
            return _input.IsKeyPressed(key);
        }

        public Vector2I ScreenSize { get; private set; }
        public UiManager(IGame game) 
            : base(game, 1000, false, true)
        {
            _activeScreens = new List<Screen>();
            StyleManager = new StyleManager();
        }

        private void OnMouseMoved(Vector2I position)
        {
            MousePosition = position;
            if (_activeScreens.Count > 0)
                _activeScreens[0].MouseMoved(position);
        }

        public override void Initialize(Action<string> feedback)
        {
            _input = Game.Services.Get<IInput>();
            _input.MouseMoved += OnMouseMoved;
            _input.MouseClicked += OnMouseClicked;
            _input.MouseReleased += OnMouseReleased;
            _input.KeyPressed += OnKeyPressed;
            _input.KeyReleased += OnKeyReleased;
            _input.MouseScrolled += OnMouseScrolled;
            ScreenSize = Game.Services.Get<IRenderer>().ScreenSize;
        }

        private void OnMouseScrolled(int delta)
        {
            if (_activeScreens.Count > 0)
                _activeScreens[0].MouseScrolled(delta);
        }

        private void OnKeyReleased(Key key)
        {
            if (_activeScreens.Count > 0)
                _activeScreens[0].KeyReleased(key);
        }

        private void OnKeyPressed(Key key, char? character, bool repeat)
        {
            if (_activeScreens.Count > 0)
                _activeScreens[0].KeyPressed(key, character, repeat);
        }

        private void OnMouseClicked(Vector2I position, int button)
        {
            if (_activeScreens.Count > 0)
                _activeScreens[0].MouseClicked(position, button);
        }

        private void OnMouseReleased(Vector2I position, int button)
        {
            if (_activeScreens.Count > 0)
                _activeScreens[0].MouseReleased(position, button);
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
            foreach (Screen screen in _activeScreens)
                screen.Dispose();
        }

        public void RegisterAsService()
        {
            Game.Services.Register<IUiManager>(this);
        }

        public void AddScreen(Screen screen)
        {
            if(_activeScreens.Count > 0)
                _activeScreens[0].Unregister();
            _activeScreens.Insert(0, screen);
        }

        public void DeleteScreen(Screen screen)
        {

            Screen first = _activeScreens.Count > 0 ? _activeScreens[0] : null;
            screen.Dispose();
            _activeScreens.Remove(screen);
            if (_activeScreens.Count > 0 && first != _activeScreens[0])
                _activeScreens[0].ActivateTree();
        }

        public void RecalculateActiveComponents()
        {
            _activeScreens[0].RecalculateActiveComponents();
        }

        public bool IsAnyKeyPressed(params Key[] keys)
        {
            return keys.Any(IsKeyPressed);
        }
        public bool AreAllKeysPressed(params Key[] keys)
        {
            return keys.All(IsKeyPressed);
        }

        public int SaveOrder { get { return Int32.MaxValue; } }
        public string SaveName { get { return "UI"; } }
        public void Save(XmlWriter writer)
        { }

        public void PreLoading()
        {
        }

        public void Load(SaveGame save)
        {
            _activeScreens.Clear();
            AddScreen(new GameScreen(Game));
        }

        public void PostLoading()
        {
        }
    }
}
