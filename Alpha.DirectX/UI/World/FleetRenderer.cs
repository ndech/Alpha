using System;
using System.Collections.Generic;
using Alpha.Core.Fleets;
using Alpha.DirectX.Shaders;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.World
{
    class FleetRenderer
    {
        private readonly ObjModel _model;
        private readonly LightShader _shader;
        private readonly Dictionary<Int32,Matrix> _matrices = new Dictionary<int, Matrix>();
        public FleetRenderer(IContext context)
        {
            _shader = context.Shaders.LightShader;
            _model = new ObjModel(context.DirectX.Device, "BasicBoat.obj", context.TextureManager.Create("Metal.png"));
            foreach (Fleet fleet in context.World.FleetManager.Fleets)
                OnNewFleet(fleet);
            context.NotificationResolver.FleetMoved += OnFleetUpdate;
        }

        private void OnNewFleet(Fleet fleet)
        {
            _matrices[fleet.Id] = Matrix.RotationY(-(float)(Math.PI / 2)) * Matrix.Translation((Vector3)fleet.Location.Center);
        }

        private void OnFleetUpdate(Fleet fleet)
        {
            _matrices[fleet.Id] = Matrix.RotationY(-(float)(Math.PI / 2)) * Matrix.Translation((Vector3)fleet.Location.Center);
        }

        private void OnFleetDelete(Fleet fleet)
        {
            _matrices.Remove(fleet.Id);
        }
        
        public void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix, Light light, ICamera camera)
        {
            foreach (Matrix matrix in _matrices.Values)
            {
                _model.Render(deviceContext);
                _shader.Render(deviceContext, 
                    _model.IndexCount, 
                    matrix, 
                    viewMatrix, 
                    projectionMatrix, 
                    _model.Texture, 
                    light, 
                    camera);
            }
        }
    }
}
