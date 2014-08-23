using System.Collections.Generic;
using Alpha.Core.Fleets;
using Alpha.DirectX.Shaders;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.World
{
    class FleetMoveOrderRenderer
    {
        class MoveOrderRenderingItem
        {
            
        }
        private float _translation;
        private PathShader _shader;
        private ShaderResourceView _pathTexture;
        private Dictionary<Fleet, MoveOrderRenderingItem> _items = new Dictionary<Fleet, MoveOrderRenderingItem>(); 

        public FleetMoveOrderRenderer(IContext context)
        {
            _shader = context.Shaders.PathShader;
            _pathTexture = context.TextureManager.Create("Path.png").TextureResource;
        }

        public void Update(double delta)
        {
            //_translation -= (float)delta * _fleetMoveOrder.Fleet.Speed;
        }
    }
}