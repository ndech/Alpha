using PlaneSimulator.Toolkit.Math;
using SharpDX;
using Vector3 = SharpDX.Vector3;

namespace PlaneSimulator.Graphics
{
    public class Camera : GameComponent
    {
        private Matrix _viewMatrix;
        private Matrix _uiMatrix;
        private Airplane _playerAirplane;
        private Vector3 _position;
        public Camera(Game game, Airplane playerAirplane)
            :base(game, playerAirplane.UpdateOrder + 1)
        {
            _playerAirplane = playerAirplane;
        }
        public Matrix ViewMatrix { get { return _viewMatrix; } }

        public Matrix UiMatrix { get { return _uiMatrix; } }

        public Vector3 Position { get { return _position; } }

        public override void Update(double delta)
        {
            Vector3 _position = new Vector3(
                (float)_playerAirplane.CurrentState.Position.Y, 
                _playerAirplane.Altitude + 10, 
                (float)_playerAirplane.CurrentState.Position.X-60);
            Vector3 orientation = new Vector3(0);
            // Create the rotation matrix from the yaw, pitch, and roll values (in radians).
            Matrix rotationMatrix = Matrix.RotationYawPitchRoll(
                                     Conversion.AngleToRadian(orientation.X),
                                     Conversion.AngleToRadian(orientation.Y),
                                     Conversion.AngleToRadian(orientation.Z));

            // Get the direction that the camera is pointing to and the up direction
            Vector3 lookAt = Vector3.TransformCoordinate(Vector3.UnitZ, rotationMatrix);
            Vector3 up = Vector3.TransformCoordinate(Vector3.UnitY, rotationMatrix);

            // Finally create the view matrix from the three updated vectors.
            _viewMatrix = Matrix.LookAtLH(_position, _position + lookAt, up);

            _uiMatrix = Matrix.LookAtLH(new Vector3(0, 0, -10), lookAt, up);
        }

        public override void Dispose() { }
    }
}