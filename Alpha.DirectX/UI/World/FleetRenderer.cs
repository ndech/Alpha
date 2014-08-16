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
        public FleetRenderer(IContext context)
        {
            _shader = context.Shaders.LightShader;
            _model = new ObjModel(context.DirectX.Device, "BasicBoat.obj", context.TextureManager.Create("Metal.png"));
        }
        public void Render(IEnumerable<Fleet> fleets, DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix, Light light, ICamera camera)
        {
            foreach (Fleet fleet in fleets)
            {
                _model.Render(deviceContext);
                _shader.Render(deviceContext, 
                    _model.IndexCount, 
                    Matrix.RotationY(-(float)(Math.PI / 2)) * Matrix.Translation((Vector3)fleet.Location.Center), 
                    viewMatrix, 
                    projectionMatrix, 
                    _model.Texture, 
                    light, 
                    camera);
            }
        }
    }
}
