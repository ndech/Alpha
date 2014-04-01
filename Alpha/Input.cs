using Alpha.Graphics;
using SharpDX;
using SharpDX.DirectInput;

namespace Alpha
{
    public interface IInput : IService
    {
        
    }
    public class Input : GameComponent, IInput
    {
        private readonly DirectInput _directInput;
        private readonly Keyboard _keyboard;
        private readonly Mouse _mouse;
        private KeyboardState _keyboardState;
        private MouseState _mouseState;
        public Input(IGame game) : base(game, updateOrder: -10000)
        {
            _directInput = new DirectInput();
            _keyboard = new Keyboard(_directInput);
            _mouse = new Mouse(_directInput);
        }
        
        public override void Initialize()
        {
            System.IntPtr handle = Game.Services.GetService<IRenderer>().Form.Handle;
            _keyboard.Properties.BufferSize = 256;
            _keyboard.SetCooperativeLevel(handle, CooperativeLevel.Background | CooperativeLevel.NonExclusive);
            _mouse.Properties.AxisMode = DeviceAxisMode.Relative;
            _mouse.SetCooperativeLevel(handle, CooperativeLevel.Background | CooperativeLevel.NonExclusive);
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

        public void RegisterAsService()
        {
            Game.Services.AddService<IInput>(this);
        }
    }
}
