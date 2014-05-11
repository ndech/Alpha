using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Alpha.Graphics;
using Alpha.Toolkit.Math;
using SharpDX.Windows;
using Cursor = System.Windows.Forms.Cursor;
using Point = System.Drawing.Point;

namespace Alpha
{
    public interface IInput : IService
    {
        Vector2I AbsoluteMousePosition { get; }
        event CustomEventHandler<Vector2I> MouseMoved;
        event CustomEventHandler<Vector2I, Int32> MouseClicked;
        event CustomEventHandler<Vector2I, Int32> MouseReleased;
        event CustomEventHandler<Key, bool> KeyPressed;
        event CustomEventHandler<Key> KeyReleased;
        bool IsKeyPressed(Key key);
    }
    class Input : GameComponent, IInput
    {
        private Vector2I _mousePosition;
        private readonly bool[] _previousMouseButtons;
        private readonly Dictionary<Key, double> _pressedKeys;
        private RenderForm _form;
        private Array _keys;

        public event CustomEventHandler<Vector2I> MouseMoved;
        public event CustomEventHandler<Vector2I, Int32> MouseClicked;
        public event CustomEventHandler<Vector2I, Int32> MouseReleased;
        public event CustomEventHandler<Key, bool> KeyPressed;
        public event CustomEventHandler<Key> KeyReleased;

        public Input(IGame game) : base(game, -10000)
        {
            _previousMouseButtons = new bool[2];
            _pressedKeys = new Dictionary<Key, double>();
        }
        
        public override void Initialize()
        {
            IRenderer renderer = Game.Services.GetService<IRenderer>();
            _mousePosition = renderer.ScreenSize/2;
            _form = renderer.Form;

            _keys = Enum.GetValues(typeof(Key));
            Cursor.Hide();
        }

        public override void Update(double delta)
        {
            // Truncate mouse position to screen dimensions
            Point position = _form.PointToClient(Cursor.Position);
            _mousePosition = new Vector2I(position.X, position.Y);
            MouseMoved.Raise(_mousePosition);
            
            MouseButtons buttons = Control.MouseButtons;
            bool leftMouseButtonPressed = (buttons == MouseButtons.Left);
            if(leftMouseButtonPressed && !_previousMouseButtons[0])
                MouseClicked.Raise(_mousePosition, 0);
            else if (_previousMouseButtons[0] && ! leftMouseButtonPressed)
                MouseReleased.Raise(_mousePosition, 0);
            _previousMouseButtons[0] = leftMouseButtonPressed;

            bool rightMouseButtonPressed = (buttons == MouseButtons.Right);
            if (rightMouseButtonPressed && !_previousMouseButtons[1])
                MouseClicked.Raise(_mousePosition, 1);
            else if (_previousMouseButtons[1] && !rightMouseButtonPressed)
                MouseReleased.Raise(_mousePosition, 1);
            _previousMouseButtons[1] = rightMouseButtonPressed;


            foreach (Key key in _keys.Cast<Key>().Where(key => key != Key.None))
            {
                if (Keyboard.IsKeyDown(key))
                {
                    if (_pressedKeys.ContainsKey(key))
                    {
                        _pressedKeys[key] += delta;
                        if (_pressedKeys[key] > 0.2)
                        {
                            _pressedKeys[key] = 0.0;
                            KeyPressed.Raise(key, true);
                        }
                    }
                    else
                    {
                        _pressedKeys.Add(key, -0.8);
                        KeyPressed.Raise(key, false);
                    }
                }
                else
                {
                    if (_pressedKeys.ContainsKey(key))
                    {
                        _pressedKeys.Remove(key);
                        KeyReleased.Raise(key);
                    }
                }
            }
        }

        public override void Dispose()
        { }


        public Vector2I AbsoluteMousePosition { get { return _mousePosition; } }
        
        public bool IsKeyPressed(Key key)
        {
            //return _keyboardState.IsPressed(key);
            return false;
        }

        public void RegisterAsService()
        {
            Game.Services.AddService<IInput>(this);
        }
    }
}
