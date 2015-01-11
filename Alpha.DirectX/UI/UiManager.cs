using Alpha.DirectX.Input;
using Alpha.DirectX.UI.Controls;
using Alpha.DirectX.UI.Styles;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Screen = Alpha.DirectX.UI.Screens.Screen;

namespace Alpha.DirectX.UI
{
    class UiManager : IUiManager, IDisposable
    {
        private readonly List<Screen> _activeScreens;
        private readonly MousePointer _mousePointer;
        private IInput _input;
        public Tooltip VisibleTooltip { get; set; }
        public StyleManager StyleManager { get; private set; }
        public Vector2I MousePosition { get; private set; }
        public Vector2I RelativeMousePosition(Vector2I origin)
        {
            return MousePosition - origin;
        }

        public Vector2I PreviousMousePosition { get; private set; }
        public Vector2I RelativePreviousMousePosition(Vector2I origin)
        {
            return PreviousMousePosition - origin;
        }

        public bool IsKeyPressed(Key key)
        {
            return _input.IsKeyPressed(key);
        }

        public void SetMousePointer(MousePointer.CursorType type)
        {
            _mousePointer.Type = type;
        }
        
        public Vector2I ScreenSize { get; private set; }
        public UiManager()
        {
            _activeScreens = new List<Screen>();
            _mousePointer = new MousePointer();
            StyleManager = new StyleManager();
        }

        private void OnMouseMoved(Vector2I position)
        {
            PreviousMousePosition = MousePosition;
            MousePosition = position;
            if (_activeScreens.Count > 0)
                _activeScreens[0].MouseMoved(position);
        }

        public void Initialize(IContext context)
        {
            _mousePointer.Initialize(context);
            _input = context.Input;
            _input.MouseMoved += OnMouseMoved;
            _input.MouseClicked += OnMouseClicked;
            _input.MouseReleased += OnMouseReleased;
            _input.KeyPressed += OnKeyPressed;
            _input.KeyReleased += OnKeyReleased;
            _input.MouseScrolled += OnMouseScrolled;
            ScreenSize = context.ScreenSize;
            MousePosition = ScreenSize/2;
            PreviousMousePosition = ScreenSize/2;
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

        public void Update(double delta)
        {
            foreach (Screen screen in _activeScreens.ToList())
                screen.UpdateTree(delta);
        }

        public void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            if(_activeScreens.Count ==0) return;
            int i;
            for(i = 0; i< _activeScreens.Count; i++)
                if (!_activeScreens[i].Transparent)
                    break;
            for (int j = i; j >= 0; j--)
                _activeScreens[j].RenderTree(deviceContext, Matrix.Identity, viewMatrix, projectionMatrix);
            _mousePointer.Render(deviceContext, viewMatrix, projectionMatrix);
            if(VisibleTooltip!=null)
                VisibleTooltip.RenderTooltip(deviceContext, viewMatrix, projectionMatrix);
            VisibleTooltip = null;
        }

        public void Dispose()
        {
            foreach (Screen screen in _activeScreens)
                screen.Dispose();
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

        public void SetScreen(Screen screen)
        {
            _activeScreens.Clear();
            _activeScreens.Add(screen);
        }

        public bool AreAllKeysPressed(params Key[] keys)
        {
            return keys.All(IsKeyPressed);
        }
    }
}
