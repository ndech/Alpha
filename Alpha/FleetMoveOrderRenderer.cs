using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Graphics;
using Alpha.Graphics.Shaders;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Alpha
{
    class FleetMoveOrderRenderer
    {
        private readonly FleetMoveOrder _fleetMoveOrder;

        private readonly PathShader _shader;
        private readonly ShaderResourceView _pathTexture;
        private readonly Int32 _vertexCount;
        private readonly Buffer _vertexBuffer;
        private float _translation;

        public FleetMoveOrderRenderer(IRenderer renderer, FleetMoveOrder fleetMoveOrder)
        {
            _fleetMoveOrder = fleetMoveOrder;
            _shader = renderer.PathShader;
            _pathTexture = renderer.TextureManager.Create("Path.png").TextureResource;

            List<MoveOrder.Step> allSteps;
            (allSteps = new List<MoveOrder.Step> { new MoveOrder.Step(_fleetMoveOrder.Fleet.Location, 0) }).AddRange(_fleetMoveOrder.Steps);
            _fleetMoveOrder.Fleet.Angle = (float)Math.Atan2(allSteps[1].Destination.Center[1] - allSteps[0].Destination.Center[1], allSteps[1].Destination.Center[0] - allSteps[0].Destination.Center[0]);
            List<VertexDefinition.Path> vertices = new List<VertexDefinition.Path>();
            int filling = 0;
            Vector3 prevPosition = new Vector3((float)allSteps[0].Destination.Center[0], 0.5f, (float)allSteps[0].Destination.Center[1]);
            float totalDistance = 0.0f;
            for (int i = 0; i < allSteps.Count - 1; i++)
            {
                Vector3 middlePoint =
                    allSteps[i].Destination.Adjacencies.Single(a => a.Neighbourg == allSteps[i + 1].Destination)
                        .PassingPoints.First();
                Vector3 startPoint = allSteps[i].Destination.Center;
                Vector3 endPoint = allSteps[i + 1].Destination.Center;
                Vector3 prevPoint = i == 0 ? startPoint :
                    allSteps[i].Destination.Adjacencies.Single(a => a.Neighbourg == allSteps[i - 1].Destination)
                        .PassingPoints.First();
                Vector3 nextPoint = i == allSteps.Count - 2 ? endPoint :
                    allSteps[i + 1].Destination.Adjacencies.Single(a => a.Neighbourg == allSteps[i + 2].Destination)
                        .PassingPoints.First();
                int subdivisions = allSteps[i + 1].Duration;
                //First part of the arc
                for (int j = 0; j < subdivisions; j++)
                {
                    Vector3 position = Vector3.CatmullRom(prevPoint, startPoint, middlePoint, endPoint, (1.0f * j) / subdivisions);
                    float arcLength = Vector3.Distance(prevPosition, position);
                    vertices.Add(new VertexDefinition.Path
                    {
                        fillingIndex = (uint)(++filling),
                        position = position,
                        pathLength = new Vector2(totalDistance, arcLength)
                    });
                    totalDistance += arcLength;
                    prevPosition = position;
                }
                //Second part of the arc
                for (int j = 0; j < subdivisions; j++)
                {
                    Vector3 position = Vector3.CatmullRom(startPoint, middlePoint, endPoint, nextPoint, (1.0f * j) / subdivisions);
                    float arcLength = Vector3.Distance(prevPosition, position);
                    vertices.Add(new VertexDefinition.Path
                    {
                        fillingIndex = (uint)(++filling),
                        position = position,
                        pathLength = new Vector2(totalDistance, arcLength)
                    });
                    totalDistance += arcLength;
                    prevPosition = position;
                }
            }
            _vertexCount = vertices.Count;
            _vertexBuffer = Buffer.Create(renderer.Device, BindFlags.VertexBuffer, vertices.ToArray());
        }

        public void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix, DiplomaticStatus status)
        {
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, Utilities.SizeOf<VertexDefinition.Path>(), 0));
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineStripWithAdjacency;
            _shader.Render(deviceContext, _vertexCount, _fleetMoveOrder.ElapsedProgress * 2, viewMatrix, projectionMatrix, (_fleetMoveOrder.ElapsedProgress + _fleetMoveOrder.CurrentStepProgress) * 2, _pathTexture, _translation, MoveOrder.MainColor(status), MoveOrder.BackgroundColor(status));
        }
        
        public void Update(double delta)
        {
            _translation -= (float)delta * _fleetMoveOrder.Fleet.Speed;
        }
    }
}
