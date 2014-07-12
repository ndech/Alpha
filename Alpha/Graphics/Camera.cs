using System;
using SharpDX;
using Vector3 = SharpDX.Vector3;

namespace Alpha.Graphics
{
    class Camera : GameComponent, ICamera
    {
        private Matrix _viewMatrix;
        private Matrix _uiMatrix;
        private Matrix _reflectionMatrix;
        private Vector3 _position;
        private Vector3 _orientation;
        public Camera(IGame game)
            :base(game)
        {}
        public Matrix ViewMatrix { get { return _viewMatrix; } }

        public Matrix UiMatrix { get { return _uiMatrix; } }

        public Matrix ReflectionMatrix { get { return _reflectionMatrix; } }
        public void Move(int x, int y)
        {
            _position.X += 15*x;
            _position.Z += 15*y;
            Calculate();
        }

        public void Rotate(int tick)
        {
            _orientation.X += tick*-0.03f;
            Calculate();
        }

        public Vector3 Position { get { return _position; } }

        public override void Initialize(Action<string> feedback)
        {
            _position = new Vector3(1, 200, 0);
            _orientation = new Vector3(0, 0.5f, 0);
            Calculate();
        }

        private void Calculate()
        {
            // Create the rotation matrix from the yaw, pitch, and roll values (in radians).
            Matrix rotationMatrix = Matrix.RotationYawPitchRoll(_orientation.X, _orientation.Y, _orientation.Z);

            // Get the direction that the camera is pointing to and the up direction
            Vector3 lookAt = Vector3.TransformCoordinate(Vector3.UnitZ, rotationMatrix);
            Vector3 up = Vector3.TransformCoordinate(Vector3.UnitY, rotationMatrix);
            
            // Finally create the view matrix from the three updated vectors.
            _viewMatrix = Matrix.LookAtLH(_position, _position + lookAt, up);

            _uiMatrix = Matrix.LookAtLH(new Vector3(0, 0, -50), Vector3.UnitZ, Vector3.UnitY) * Matrix.Scaling(1, -1, 1);

            _reflectionMatrix = Matrix.LookAtLH(new Vector3(_position.X, -_position.Y, _position.Z),
                new Vector3(_position.X + lookAt.X, -_position.Y /*- lookAt.Y*/, _position.Z + lookAt.Z), up);
        }

        public override void Update(double delta)
        {}

        public override void Dispose() { }
        public void RegisterAsService()
        {
            Game.Services.Register<ICamera>(this);
        }
    }
}