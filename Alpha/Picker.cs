﻿using Alpha.Graphics;
using Alpha.Toolkit.Math;
using SharpDX;

namespace Alpha
{
    class Picker
    {
        public Vector3 Origin { get; private set; }
        public Vector3 GroundIntersection { get; private set; }
        public Vector3 RayDirection { get; private set; }
        public Picker(IGame game, Vector2I mousePosition)
        {
            ICamera camera = game.Services.Get<ICamera>();
            IRenderer renderer = game.Services.Get<IRenderer>();
            //Calculate the intersection between the map and a ray coming from the camera and passing by the clicked point
            Origin = camera.Position;

            Vector2 point;
            // Move the mouse cursor coordinates into the -1 to +1 range.
            point.X = ((2.0f * mousePosition.X) / renderer.ScreenSize.X) - 1.0f;
            point.Y = (((2.0f * mousePosition.Y) / renderer.ScreenSize.Y) - 1.0f) * -1.0f;

            // Adjust the points using the projection matrix to account for the aspect ratio of the viewport.
            point.X = point.X / renderer.ProjectionMatrix.M11;
            point.Y = point.Y / renderer.ProjectionMatrix.M22;

            Matrix inverseViewMatrix = Matrix.Invert(camera.ViewMatrix);

            // Calculate the direction of the picking ray in view space.
            RayDirection = new Vector3(
                (point.X * inverseViewMatrix.M11) + (point.Y * inverseViewMatrix.M21) + inverseViewMatrix.M31,
                (point.X * inverseViewMatrix.M12) + (point.Y * inverseViewMatrix.M22) + inverseViewMatrix.M32,
                (point.X * inverseViewMatrix.M13) + (point.Y * inverseViewMatrix.M23) + inverseViewMatrix.M33);
            GroundIntersection = Origin - RayDirection * (Origin.Y / RayDirection.Y);
        }
    }
}
