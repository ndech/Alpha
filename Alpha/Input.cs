using System;
using System.Windows.Forms;
using Alpha.Graphics;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.DirectInput;
using Point = System.Drawing.Point;

namespace Alpha
{
    public interface IInput : IService
    {
        Vector2I AbsoluteMousePosition { get; }
        event CustomEventHandler<Vector2I> MouseMoved;
        event CustomEventHandler<Vector2I, Int32> MouseClicked;
        event CustomEventHandler<Vector2I, Int32> MouseReleased;
    }
    class Input : GameComponent, IInput
    {
        private readonly DirectInput _directInput;
        private readonly Keyboard _keyboard;
        private readonly Mouse _mouse;
        private KeyboardState _keyboardState;
        private MouseState _mouseState;
        private Vector2I _mousePosition;
        private IRenderer _renderer;
        private readonly bool[] _previousMouseButtons;
        
        public event CustomEventHandler<Vector2I> MouseMoved;
        public event CustomEventHandler<Vector2I, Int32> MouseClicked;
        public event CustomEventHandler<Vector2I, Int32> MouseReleased;

        public Input(IGame game) : base(game, -10000)
        {
            _directInput = new DirectInput();
            _keyboard = new Keyboard(_directInput);
            _mouse = new Mouse(_directInput);
            _previousMouseButtons = new bool[8];
        }
        
        public override void Initialize()
        {
            _renderer = Game.Services.GetService<IRenderer>();
            IntPtr handle = _renderer.Form.Handle;
            _keyboard.Properties.BufferSize = 256;
            _keyboard.SetCooperativeLevel(handle, CooperativeLevel.Background | CooperativeLevel.NonExclusive);
            _mouse.Properties.AxisMode = DeviceAxisMode.Relative;
            _mouse.SetCooperativeLevel(handle, CooperativeLevel.Foreground | CooperativeLevel.NonExclusive);
            _mousePosition = _renderer.ScreenSize/2;
            Cursor.Hide();
        }

        public override void Update(double delta)
        { 
            ReadMouse();
            ReadKeyboard();

            if (_mouseState == null) return;

            //Truncate mouse position to screen dimensions

            Point position = _renderer.Form.PointToClient(Cursor.Position);
            _mousePosition = new Vector2I(position.X, position.Y);
            //_mousePosition.X = Math.Max(0, Math.Min(_screenSize.X, _mousePosition.X + RelativeMousePosition.X));
            //_mousePosition.Y = Math.Max(0, Math.Min(_screenSize.Y, _mousePosition.Y + RelativeMousePosition.Y));
            MouseMoved.Raise(_mousePosition);

            //Send mouse clicks signals
            for (int i = 0; i < 8; i ++)
            {
                if (_mouseState.Buttons[i] ==_previousMouseButtons[i]) continue;
                if (_mouseState.Buttons[i] && ! _previousMouseButtons[i])
                    MouseClicked.Raise(_mousePosition, i);
                else if (_previousMouseButtons[i] && ! _mouseState.Buttons[i])
                    MouseReleased.Raise(_mousePosition, i);
                _previousMouseButtons[i] = _mouseState.Buttons[i];
            }
        }

        public override void Dispose()
        {
            _keyboard.Unacquire();
            _keyboard.Dispose();

            _mouse.Unacquire();
            _mouse.Dispose();

            _directInput.Dispose();
        }

        private void ReadMouse()
        {
            try
            {
                _mouseState = _mouse.GetCurrentState();
            }
            catch (SharpDXException e)
            {
                if (e.Descriptor == ResultCode.InputLost || e.Descriptor == ResultCode.NotAcquired)
                {
                    try
                    {
                        _mouse.Acquire();
                        _mouseState = _mouse.GetCurrentState();
                    }
                    catch (SharpDXException)
                    { }
                }
                else
                    throw;
            }
        }

        private void ReadKeyboard()
        {
            try
            {
                _keyboardState = _keyboard.GetCurrentState();
            }
            catch (SharpDXException e)
            {
                if (e.Descriptor == ResultCode.InputLost || e.Descriptor == ResultCode.NotAcquired)
                {
                    try
                    {
                        _keyboard.Acquire();
                        _keyboardState = _keyboard.GetCurrentState();
                    }
                    catch(SharpDXException)
                    { }
                }
                else
                    throw;
            }
        }


        public Vector2I AbsoluteMousePosition { get { return _mousePosition; } }
        
        public bool IsKeyPressed(Key key)
        {
            if (_keyboardState == null)
                return false;
            return _keyboardState.IsPressed(key);
        }

        public void RegisterAsService()
        {
            Game.Services.AddService<IInput>(this);
        }
    }
}
