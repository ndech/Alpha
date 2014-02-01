using System;
using PlaneSimulator.Graphics;
using PlaneSimulator.Graphics.Shaders;
using SharpDX;
using SharpDX.Direct3D11;

namespace PlaneSimulator
{
    class AirplaneOverlay : RenderableGameComponent
    {
        private Airplane _airplane;
        private Airplane _playerAirplane;
        private Text _nameText;
        private Text _modelNameText;
        private Text _distanceText;
        private Vector3 _position;
        public Bitmap _overlay;
        public FontShader _shader;
        public Vector4 _color;

        public AirplaneOverlay(Game game, Renderer renderer, Airplane airplane, Airplane playerAirplane)
            : base(game, renderer, airplane.UpdateOrder+1, false, true)
        {
            _airplane = airplane;
            _playerAirplane = playerAirplane;
            _shader = Renderer.FontShader;
            _overlay = new Bitmap(Renderer.DirectX.Device, Renderer.TextureManager.Create("Circle.png").TextureResource, (int) Renderer.ScreenSize.X,
                (int)Renderer.ScreenSize.Y, 100, 100);
            _overlay.Position = new Vector2(0);
            _overlay.Size = new Vector2(40, 40);
            _color = new Vector4(0.2f, 0, 0, 0.4f);
            _distanceText = Renderer.TextManager.Create("Courrier", 14, 8, _color);
            _distanceText.Position = new Vector2(0,0);
            _nameText = Renderer.TextManager.Create("Courrier", 14, 20, _color);
            _nameText.Position = new Vector2(0,0);
            _nameText.Content = airplane.Name;
            _modelNameText = Renderer.TextManager.Create("Courrier", 14, 20, _color);
            _modelNameText.Position = new Vector2(0, 0);
            _modelNameText.Content = airplane.ModelName;
        }

        public override void Update(double delta)
        {
            _position = Vector3.Project(_airplane.PositionInDxCoordinates(), 0, 0, 
                Renderer.ScreenSize.X, Renderer.ScreenSize.Y, 0.0f, 1.0f,
                Renderer.Camera.ViewMatrix * Renderer.DirectX.ProjectionMatrix);
            int distance = (int)(_playerAirplane.CurrentState.Position - _airplane.CurrentState.Position).Magnitude;
            _distanceText.Content = distance > 1000 ? String.Format("{0:0.00} km", (float) distance/1000) : String.Format("{0} m", distance);
        }

        public override void Dispose()
        {
            _overlay.Dispose();
        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _overlay.Render(deviceContext);
            _shader.Render(deviceContext, _overlay.IndexCount, Matrix.Translation(new Vector3(_position.X - 20, -_position.Y + 20, 0.0f)), 
                viewMatrix, projectionMatrix, _overlay.Texture, _color);

            _distanceText.Update(deviceContext);
            _distanceText.Render(deviceContext, Matrix.Translation(new Vector3(_position.X - _distanceText.Width / 2, -_position.Y - 20, 0.0f)), viewMatrix, projectionMatrix);
            _nameText.Render(deviceContext, Matrix.Translation(new Vector3(_position.X +22, -_position.Y + 15, 0.0f)), viewMatrix, projectionMatrix);
            _modelNameText.Render(deviceContext, Matrix.Translation(new Vector3(_position.X + 22, -_position.Y, 0.0f)), viewMatrix, projectionMatrix);
        }
    }
}