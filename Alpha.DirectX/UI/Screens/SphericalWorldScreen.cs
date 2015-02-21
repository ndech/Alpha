using System;
using System.Windows.Input;
using Alpha.DirectX.UI.World;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Screens
{
    class SphericalWorldScreen : Screen
    {
        private readonly Sphere _sphere;
        private readonly Sky _sky;
        private readonly Sun _sun;
        private readonly SphericalWorldCamera _camera;
        public SphericalWorldScreen(IContext context) : base(context, "spherical_world")
        {
            _sphere = new Sphere(Context, 255, 10000);
            _sky = new Sky(Context);
            _sun = new Sun();
            _camera = new SphericalWorldCamera();
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            Context.DirectX.EnableZBuffer();
            _sky.Render(deviceContext, _camera.ViewMatrix, Context.DirectX.ProjectionMatrix, _sun ,_camera);
            _sphere.Render(deviceContext, Matrix.Scaling(100), _camera.ViewMatrix, Context.DirectX.ProjectionMatrix);
            Context.DirectX.DisableZBuffer();
        }

        protected override void Update(double delta)
        {
            UpdateCameraFromInput();
            _camera.Update(delta);
        }

        protected override void DisposeItem()
        {
            DisposeHelper.DisposeAndSetToNull(_sphere);
        }
        public override bool OnMouseScrolled(int delta)
        {
            _camera.Zoom(-delta);
            return true;
        }

        private bool keyPressed = false;
        private void UpdateCameraFromInput()
        {
            if (Context.UiManager.IsKeyPressed(Key.Left))
                _camera.Move(-1, 0);
            if (Context.UiManager.IsKeyPressed(Key.Right))
                _camera.Move(1, 0);
            if (Context.UiManager.IsKeyPressed(Key.Up))
                _camera.Move(0, 1);
            if (Context.UiManager.IsKeyPressed(Key.Down))
                _camera.Move(0, -1);

            if (Context.UiManager.IsKeyPressed(Key.X))
            {
                if (!keyPressed)
                {
                    Context.DirectX.SwitchWireFrameMode();
                    keyPressed = true;
                }
            }
            else
                keyPressed = false;
        }
    }
}
