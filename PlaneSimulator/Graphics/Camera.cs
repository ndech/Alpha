using SharpDX;
using Vector3 = SharpDX.Vector3;

namespace PlaneSimulator.Graphics
{
    public class Camera : GameComponent
    {
        private Matrix _viewMatrix;
        private Matrix _uiMatrix;
        private Matrix _reflectionMatrix;
        private readonly Airplane _playerAirplane;
        private Vector3 _position;
        public Camera(Game game, Airplane playerAirplane)
            :base(game, playerAirplane.UpdateOrder + 1)
        {
            _playerAirplane = playerAirplane;
        }
        public Matrix ViewMatrix { get { return _viewMatrix; } }

        public Matrix UiMatrix { get { return _uiMatrix; } }

        public Matrix ReflectionMatrix { get { return _reflectionMatrix; } }

        public Vector3 Position { get { return _position; } }

        public override void Update(double delta)
        {
            _position = new Vector3(
                (float)_playerAirplane.CurrentState.Position.Y, 
                _playerAirplane.Altitude, 
                (float)_playerAirplane.CurrentState.Position.X);
            
            Vector3 orientation = new Vector3((float)_playerAirplane.CurrentState.AngularPosition.X, 
                                              (float)_playerAirplane.CurrentState.AngularPosition.Y, 
                                              (float)_playerAirplane.CurrentState.AngularPosition.Z);
            // Create the rotation matrix from the yaw, pitch, and roll values (in radians).
            Matrix rotationMatrix = Matrix.RotationYawPitchRoll(orientation.X, orientation.Y, orientation.Z);

            // Get the direction that the camera is pointing to and the up direction
            Vector3 lookAt = Vector3.TransformCoordinate(Vector3.UnitZ, rotationMatrix);
            Vector3 up = Vector3.TransformCoordinate(Vector3.UnitY, rotationMatrix);

            Vector3 positionDisplacement = Vector3.TransformCoordinate(new Vector3(0, 10, -60), rotationMatrix);

            // Finally create the view matrix from the three updated vectors.
            _viewMatrix = Matrix.LookAtLH(_position + positionDisplacement, _position + positionDisplacement + lookAt, up);

            _uiMatrix = Matrix.LookAtLH(new Vector3(0, 0, -50), Vector3.UnitZ, Vector3.UnitY);

            _reflectionMatrix = Matrix.LookAtLH(new Vector3(_position.X, -_position.Y, _position.Z),
                new Vector3(_position.X + lookAt.X, -_position.Y /*- lookAt.Y*/, _position.Z + lookAt.Z), up);
        }

        public override void Dispose() { }
    }
}