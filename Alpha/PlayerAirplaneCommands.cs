using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace Alpha
{
    class PlayerAirplaneCommands : IAirplaneCommands
    {
        private readonly Input _input;
        public PlayerAirplaneCommands(Input input)
        {
            _input = input;
        }
        public void Update(double time)
        {
            if (_input.JoystickEnabled)
                LeftAileron = -((float)(_input.JoystickState.X-32767))/65536;
            if (_input.IsKeyPressed(Key.Left))
                LeftAileron = 1;
            else if (_input.IsKeyPressed(Key.Right))
                LeftAileron = -1;
            RightAileron = -LeftAileron;

            if (_input.JoystickEnabled)
                Elevator = -((float)(_input.JoystickState.Y - 32767)) / 65536;
            if (_input.IsKeyPressed(Key.Up))
                Elevator = 1;
            else if (_input.IsKeyPressed(Key.Down))
                Elevator = -1;
        }

        public float Thrust { get; private set; }
        public float LeftAileron { get; private set; }
        public float RightAileron { get; private set; }
        public float Elevator { get; private set; }
    }
}
