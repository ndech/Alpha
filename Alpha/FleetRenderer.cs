using System;
using System.Collections.Generic;
using Alpha.Graphics;
using Alpha.Graphics.Shaders;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha
{
    class FleetRenderer
    {
        private readonly ObjModel _model;
        private readonly LightShader _shader;
        private readonly Light _light;
        private readonly ICamera _camera;
        private readonly TexturedRectangle _selectedOverlay;
        public FleetRenderer(IGame game)
        {
            IRenderer renderer = game.Services.Get<IRenderer>();
            _shader = renderer.LightShader;
            _model = new ObjModel(renderer.Device, "BasicBoat.obj", renderer.TextureManager.Create("Metal.png"));
            _camera = game.Services.Get<ICamera>();
            _selectedOverlay = new TexturedRectangle(renderer, new Vector2I(25, 25), renderer.TextureManager.Create("SelectedCircle.png"));
            _light = game.Services.Get<IWorld>().Sun;
        }
        public void Render(IList<Fleet> fleets, DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _model.Render(deviceContext);
            foreach (Fleet fleet in fleets)
            {
                _shader.Render(deviceContext, _model.IndexCount, Matrix.RotationY(-(float)(Math.PI / 2 + fleet.Angle)) * Matrix.Translation(fleet.Location.Center), viewMatrix, projectionMatrix, _model.Texture, _light, _camera);
                if(fleet.MoveOrder!=null)
                    fleet.MoveOrder.Render(deviceContext, viewMatrix, projectionMatrix);
            }
            _selectedOverlay.Render(deviceContext, Matrix.RotationX(-(float)Math.PI / 2) * Matrix.Translation(fleets[0].Location.Center.X - 12.5f, 0.2f, fleets[0].Location.Center.Z + 12.5f), viewMatrix, projectionMatrix);
        }
    }
}
