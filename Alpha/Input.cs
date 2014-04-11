using System;
using Alpha.Graphics;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.DirectInput;

namespace Alpha
{
    public interface IInput : IService
    {
        Vector2I AbsoluteMousePosition();

         event Input.MouseMovedEventHandler MouseMoved;
    }
    public class Input : GameComponent, IInput
    {
        private readonly DirectInput _directInput;
        private readonly Keyboard _keyboard;
        private readonly Mouse _mouse;
        private KeyboardState _keyboardState;
        private MouseState _mouseState;
        private Vector2I _mousePosition;

        public delegate void MouseMovedEventHandler(Vector2I coordinates);
        public event MouseMovedEventHandler MouseMoved = delegate {};
        public Input(IGame game) : base(game, updateOrder: -10000)
        {
            _directInput = new DirectInput();
            _keyboard = new Keyboard(_directInput);
            _mouse = new Mouse(_directInput);
        }
        
        public override void Initialize()
        {
            IRenderer renderer = Game.Services.GetService<IRenderer>();
            _mousePosition = renderer.ScreenSize/2;
            System.IntPtr handle = renderer.Form.Handle;
            _keyboard.Properties.BufferSize = 256;
            _keyboard.SetCooperativeLevel(handle, CooperativeLevel.Background | CooperativeLevel.NonExclusive);
            _mouse.Properties.AxisMode = DeviceAxisMode.Relative;
            _mouse.SetCooperativeLevel(handle, CooperativeLevel.Foreground | CooperativeLevel.NonExclusive);
        }

        public override void Update(double delta)
        {
            ReadMouse();
            ReadKeyboard();

            Vector2I relativeMousePoistion = RelativeMousePosition();
            _mousePosition += RelativeMousePosition();
            if(relativeMousePoistion != Vector2I.Zero)
                MouseMoved.Invoke(_mousePosition);
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

        public Vector2I RelativeMousePosition()
        {
            if(_mouseState == null)
                return new Vector2I(0,0);
            return new Vector2I(_mouseState.X, _mouseState.Y);
        }
        public Vector2I AbsoluteMousePosition()
        {
            return _mousePosition;
        }
        
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
