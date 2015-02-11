using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Fleets;
using Alpha.Core.Movement;
using Alpha.DirectX.Shaders;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.Direct3D;

namespace Alpha.DirectX.UI.World
{
    class FleetMoveOrderRenderer : IDisposable
    {
        private class MoveOrderRenderingItem : IDisposable
        {
            public readonly Int32 VertexCount;
            public readonly Buffer VertexBuffer;
            public Double Speed;
            public Int32 VisiblePointsOffset;
            public Int32 FilledPointsOffset;

            public MoveOrderRenderingItem(Int32 vertexCount, Buffer vertexBuffer, Double speed)
            {
                VertexBuffer = vertexBuffer;
                VertexCount = vertexCount;
                Speed = speed;
                VisiblePointsOffset = 0;
                FilledPointsOffset = 0;
            }

            public void Dispose()
            {
                VertexBuffer.Dispose();
            }
        }

        private float _translation;
        const int ArcSubdivisions = 20;
        private readonly PathShader _shader;
        private readonly ShaderResourceView _pathTexture;
        private readonly Dictionary<Fleet, MoveOrderRenderingItem> _items = new Dictionary<Fleet, MoveOrderRenderingItem>(); 

        public FleetMoveOrderRenderer(IContext context)
        {
            _shader = context.Shaders.Get<PathShader>();
            _pathTexture = context.TextureManager.Create("Path.png").TextureResource;
            foreach (Fleet fleet in context.World.FleetManager.Fleets.Where(f=>f.HasMoveOrder))
                NewMoveOrder(context, fleet);
            context.NotificationResolver.NewFleetMoveOrder += f => NewMoveOrder(context, f);
        }

        private void NewMoveOrder(IContext context, Fleet fleet)
        {
            if (_items.ContainsKey(fleet))
            {
                _items[fleet].Dispose();
            }
            if (fleet.MoveOrder == null)
            {
                _items.Remove(fleet);
                return;
            }
            List<Step> steps = fleet.MoveOrder.Steps.ToList();
            List<VertexDefinition.Path> vertices = new List<VertexDefinition.Path>();
            int index = 0;
            Vector3 prevPosition = (Vector3) steps[0].Source.Center;
            float totalDistance = 0.0f;
            for(int i = 0; i<steps.Count ; i++)
            {
                Vector3 middlePoint = (Vector3)steps[i].Destination.Adjacencies.Single(a => a.Neighbourg == steps[i].Source).PassingPoints.First();
                Vector3 startPoint = (Vector3)steps[i].Source.Center;
                Vector3 endPoint = (Vector3)steps[i].Destination.Center;
                Vector3 prevPoint = i == 0 ? startPoint :
                    (Vector3)steps[i].Source.Adjacencies.Single(a => a.Neighbourg == steps[i - 1].Source).PassingPoints.First();
                Vector3 nextPoint = i == steps.Count -1 ? endPoint :
                    (Vector3)steps[i].Destination.Adjacencies.Single(a => a.Neighbourg == steps[i + 1].Destination).PassingPoints.First();
                //First part of the arc
                for (int j = 0; j < ArcSubdivisions; j++)
                {
                    Vector3 position = Vector3.CatmullRom(prevPoint, startPoint, middlePoint, endPoint, (1.0f * j) / ArcSubdivisions);
                    float arcLength = Vector3.Distance(prevPosition, position);
                    vertices.Add(new VertexDefinition.Path
                    {
                        fillingIndex = (uint)(++index),
                        position = position,
                        pathLength = new Vector2(totalDistance, arcLength)
                    });
                    totalDistance += arcLength;
                    prevPosition = position;
                }
                //Second part of the arc
                for (int j = 0; j < ArcSubdivisions; j++)
                {
                    Vector3 position = Vector3.CatmullRom(startPoint, middlePoint, endPoint, nextPoint, (1.0f * j) / ArcSubdivisions);
                    float arcLength = Vector3.Distance(prevPosition, position);
                    vertices.Add(new VertexDefinition.Path
                    {
                        fillingIndex = (uint)(++index),
                        position = position,
                        pathLength = new Vector2(totalDistance, arcLength)
                    });
                    totalDistance += arcLength;
                    prevPosition = position;
                }
            }
            _items[fleet] = new MoveOrderRenderingItem(
                vertices.Count, 
                Buffer.Create(context.DirectX.Device, BindFlags.VertexBuffer, vertices.ToArray()), 
                fleet.Speed);
        }

        public void Update(double delta)
        {
            _translation -= (float)delta/2;
            foreach (KeyValuePair<Fleet, MoveOrderRenderingItem> item in _items)
            {
                item.Value.Speed = item.Key.Speed;
                item.Value.VisiblePointsOffset = item.Key.MoveOrder.CurrentStepIndex * 2 * ArcSubdivisions;
                item.Value.FilledPointsOffset = item.Value.VisiblePointsOffset + (int)(item.Key.MoveOrder.ProgressToNextStep * 2 * ArcSubdivisions);
            }
        }

        public void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            foreach (MoveOrderRenderingItem item in _items.Values)
            {
                deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(item.VertexBuffer, Utilities.SizeOf<VertexDefinition.Path>(), 0));
                deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineStripWithAdjacency;
                _shader.Render(deviceContext,
                               item.VertexCount,
                               item.VisiblePointsOffset,
                               viewMatrix,
                               projectionMatrix,
                               item.FilledPointsOffset,
                               _pathTexture,
                               _translation * item.Speed,
                               Color.Red.ToVector4(),
                               Color.MistyRose.ToVector4());
            }
        }

        public void Dispose()
        {
            foreach (MoveOrderRenderingItem item in _items.Values)
                item.Dispose();
        }
    }
}