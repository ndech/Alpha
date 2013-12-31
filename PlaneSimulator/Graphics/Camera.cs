using PlaneSimulator.Toolkit.Math;
using SharpDX;
using Vector3 = SharpDX.Vector3;

namespace PlaneSimulator.Graphics
{
    class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Orientation { get; set; }
        public Matrix ViewMatrix
        {
            get
            {
                // Create the rotation matrix from the yaw, pitch, and roll values (in radians).
                Matrix rotationMatrix = Matrix.RotationYawPitchRoll(
                                         Conversion.AngleToRadian(Orientation.X),
                                         Conversion.AngleToRadian(Orientation.Y),
                                         Conversion.AngleToRadian(Orientation.Z));

                // Get the direction that the camera is pointing to and the up direction
                Vector3 lookAt = Vector3.TransformCoordinate(Vector3.UnitZ, rotationMatrix);
                Vector3 up = Vector3.TransformCoordinate(Vector3.UnitY, rotationMatrix);

                // Finally create the view matrix from the three updated vectors.
                return Matrix.LookAtLH(Position, Position + lookAt, up);
            }
        }

        public Camera(Vector3 position, Vector3 orientation)
        {
            Orientation = orientation;
            Position = position;
        }
    }
}