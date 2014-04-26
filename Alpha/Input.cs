using System;
using System.Windows.Forms;
using Alpha.Graphics;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.DirectInput;

namespace Alpha
{
    public interface IInput : IService
    {
        Vector2I AbsoluteMousePosition { get; }
        event CustomEventHandler<Vector2I> MouseMoved;
        event CustomEventHandler<Int32> MouseButtonClicked;
        event CustomEventHandler<Int32> MouseButtonReleased;
    }
    class Input : GameComponent, IInput
    {
        private readonly DirectInput _directInput;
        private readonly Keyboard _keyboard;
        private readonly Mouse _mouse;
        private KeyboardState _keyboardState;
        private MouseState _mouseState;
        private Vector2I _mousePosition;
        private Vector2I _screenSize;
        private readonly bool[] _previousMouseButtons;
        
        public event CustomEventHandler<Vector2I> MouseMoved;
        public event CustomEventHandler<Int32> MouseButtonClicked;
        public event CustomEventHandler<Int32> MouseButtonReleased;

        public Input(IGame game) : base(game, updateOrder: -10000)
        {
            _directInput = new DirectInput();
            _keyboard = new Keyboard(_directInput);
            _mouse = new Mouse(_directInput);
            _previousMouseButtons = new bool[8];
        }
        
        public override void Initialize()
        {
            IRenderer renderer = Game.Services.GetService<IRenderer>();
            _mousePosition = renderer.ScreenSize/2;
            IntPtr handle = renderer.Form.Handle;
            _keyboard.Properties.BufferSize = 256;
            _keyboard.SetCooperativeLevel(handle, CooperativeLevel.Background | CooperativeLevel.NonExclusive);
            _mouse.Properties.AxisMode = DeviceAxisMode.Relative;
            _mouse.SetCooperativeLevel(handle, CooperativeLevel.Foreground | CooperativeLevel.NonExclusive);
            _screenSize = Game.Services.GetService<IRenderer>().ScreenSize;
            Cursor.Hide();
        }

        public override void Update(double delta)
        {
            ReadMouse();
            ReadKeyboard();

            if (_mouseState == null) return;

            //Truncate mouse position to screen dimensions
            _mousePosition.X = Math.Max(0, Math.Min(_screenSize.X, _mousePosition.X + RelativeMousePosition.X));
            _mousePosition.Y = Math.Max(0, Math.Min(_screenSize.Y, _mousePosition.Y + RelativeMousePosition.Y));

            Console.WriteLine(_mousePosition);
            //Send mouse position signal
            if (RelativeMousePosition != Vector2I.Zero)
                MouseMoved.Raise(_mousePosition);

            //Send mouse clicks signals
            for (int i = 0; i < 8; i ++)
            {
                if (_mouseState.Buttons[i] ==_previousMouseButtons[i]) continue;
                if (_mouseState.Buttons[i] && ! _previousMouseButtons[i])
                    MouseButtonClicked.Raise(i);
                else if (_previousMouseButtons[i] && ! _mouseState.Buttons[i])
                    MouseButtonReleased.Raise(i);
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

        public Vector2I RelativeMousePosition
        {
            get
            {
                if (_mouseState == null)
                    return new Vector2I(0, 0);
                return new Vector2I(_mouseState.X, _mouseState.Y);
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
