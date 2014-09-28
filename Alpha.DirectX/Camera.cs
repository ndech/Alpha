using Alpha.DirectX.UI;
using Alpha.Toolkit.Math;
using SharpDX;

namespace Alpha.DirectX
{
    class Camera : ICamera
    {
        private IContext _context;
        private Matrix _viewMatrix;
        private Matrix _uiMatrix;
        private Matrix _reflectionMatrix;
        private Vector3 _targetPosition;
        private Vector3 _currentPosition;
        private Vector3 _orientation;

        public Matrix ViewMatrix { get { return _viewMatrix; } }

        public Matrix UiMatrix { get { return _uiMatrix; } }

        public Matrix ReflectionMatrix { get { return _reflectionMatrix; } }

        public void Move(int x, int y)
        {
            _targetPosition += Vector3.TransformCoordinate(new Vector3(5*x, 0, 5*y), Matrix.RotationY(_orientation.X));
        }

        public void Zoom(int tick)
        {
            _targetPosition.Y += 10*tick;
        }

        public void Rotate(int tick)
        {
            _orientation.X += tick*-0.02f;
            Calculate();
        }

        public Vector3 Position
        {
            get { return _targetPosition; }
            set { _targetPosition = value; }
        }

        public Camera()
        {
            _targetPosition = new Vector3(1000, 200, 0);
            _currentPosition = new Vector3(1000, 200, 0);
            _orientation = new Vector3(0, 0.7f, 0);
            Calculate();
        }

        public void Update(double delta)
        {
            _currentPosition += (_targetPosition - _currentPosition)*0.08f;
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
            _viewMatrix = Matrix.LookAtLH(_currentPosition, _currentPosition + lookAt, up);

            _uiMatrix = Matrix.LookAtLH(new Vector3(0, 0, -50), Vector3.UnitZ, Vector3.UnitY) * Matrix.Scaling(1, -1, 1);

            _reflectionMatrix = Matrix.LookAtLH(new Vector3(_currentPosition.X, -_currentPosition.Y, _currentPosition.Z),
                new Vector3(_currentPosition.X + lookAt.X, -_currentPosition.Y /*- lookAt.Y*/, _currentPosition.Z + lookAt.Z), up);
        }

        public void Initialize(IContext context)
        {
            _context = context;
        }
    }
}