using System;
using SharpDX;

namespace Alpha.DirectX
{
    class SphericalWorldCamera : ICamera
    {
        private Matrix _viewMatrix;
        private double _distance = 500;
        private double _targetDistance = 500;
        private Vector3 _orientation;

        public Matrix ViewMatrix { get { return _viewMatrix; } }

        public Matrix UiMatrix { get { throw new InvalidOperationException("No Ui matrix for spherical cameras");} }

        public void Move(int x, int y)
        {
            _orientation.X += -(float)(x * 0.01);
            _orientation.Y = (float)Math.Max(-Math.PI/2, Math.Min(_orientation.Y+(y * 0.01), Math.PI/2));
        }

        public void Tilt(int z)
        {
            _orientation.Z += -(float)(z * 0.01);
        }

        public void Zoom(int tick)
        {
            _targetDistance += 10*tick;
        }

        public void Rotate(int tick)
        {
            _orientation.X += tick*-0.02f;
            Calculate();
        }

        public Vector3 Position
        {
            get { return new Vector3((float)_distance); }
            set { //_targetPosition = value; 
            }
        }

        public SphericalWorldCamera()
        {
            _orientation = new Vector3(0, 0, 0);
            Calculate();
        }

        public void Update(double delta)
        {
            _distance += (_targetDistance - _distance)*delta*5;
            Calculate();
        }


        private void Calculate()
        {
            // Create the rotation matrix from the yaw, pitch, and roll values (in radians).
            Matrix rotationMatrix = Matrix.RotationYawPitchRoll(_orientation.X, _orientation.Y, _orientation.Z);

            // Get the direction that the camera is pointing to and the up direction
            Vector3 up = Vector3.TransformCoordinate(Vector3.UnitY, rotationMatrix);

            Vector3 position = Vector3.TransformCoordinate(new Vector3(0,0, -(float)_distance), rotationMatrix);

            // Finally create the view matrix from the three updated vectors.
            _viewMatrix = Matrix.LookAtLH(position, new Vector3(0, 0, 0), up);
        }
    }
}
