using System;
using SharpDX;
using SharpDX.DirectInput;

namespace PlaneSimulator
{
    public class Input : GameComponent
    {
        private DirectInput _directInput;
        private Keyboard _keyboard;
        private Mouse _mouse;
        private KeyboardState _keyboardState;
        private MouseState _mouseState;

        public Input(Game game, IntPtr handle) : base(game, -10000)
        {
            _directInput = new DirectInput();
            _keyboard = new Keyboard(_directInput);
            _keyboard.Properties.BufferSize = 256;
            _keyboard.SetCooperativeLevel(handle, CooperativeLevel.Background | CooperativeLevel.NonExclusive);
            //_keyboard.Acquire();
            _mouse = new Mouse(_directInput);
            _mouse.Properties.AxisMode = DeviceAxisMode.Relative;
            _mouse.SetCooperativeLevel(handle, CooperativeLevel.Background | CooperativeLevel.NonExclusive);
            //_mouse.Acquire();
        }

        public override void Update(double delta)
        {
            ReadMouse();
            ReadKeyboard();
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

        public bool IsKeyPressed(Key key)
        {
            if (_keyboardState == null)
                return false;
            return _keyboardState.IsPressed(key);
        }
    }
}
