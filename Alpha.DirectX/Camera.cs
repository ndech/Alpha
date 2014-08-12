using SharpDX;

namespace Alpha.DirectX
{
    class Camera : ICamera
    {
        private Matrix _viewMatrix;
        private Matrix _uiMatrix;
        private Matrix _reflectionMatrix;
        private Vector3 _position;
        private Vector3 _orientation;

        public Matrix ViewMatrix { get { return _viewMatrix; } }

        public Matrix UiMatrix { get { return _uiMatrix; } }

        public Matrix ReflectionMatrix { get { return _reflectionMatrix; } }
        public void Move(int x, int y)
        {
            Vector3 move = new Vector3(5*x, 0, 5*y);
            _position += Vector3.TransformCoordinate(move,Matrix.RotationY(_orientation.X));
            Calculate();
        }

        public void Zoom(int tick)
        {
            _position.Y += 10*tick;
            Calculate();
        }
        public void Rotate(int tick)
        {
            _orientation.X += tick*-0.02f;
            Calculate();
        }

        public Vector3 Position { get { return _position; } }

        public Camera()
        {
            _position = new Vector3(1000, 200, 0);
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
    }
}