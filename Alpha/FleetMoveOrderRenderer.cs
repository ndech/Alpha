using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Graphics;
using Alpha.Graphics.Shaders;
using Alpha.WorldGeneration;
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

            List<FleetMoveOrder.Step> allSteps;
            (allSteps = new List<FleetMoveOrder.Step> { new FleetMoveOrder.Step(_fleetMoveOrder.Fleet.Location, 0) }).AddRange(_fleetMoveOrder.Steps);
            _fleetMoveOrder.Fleet.Angle = (float)Math.Atan2(allSteps[1].Destination.Center[1] - allSteps[0].Destination.Center[1], allSteps[1].Destination.Center[0] - allSteps[0].Destination.Center[0]);
            List<VertexDefinition.Path> vertices = new List<VertexDefinition.Path>();
            int filling = 0;
            Vector3 prevPosition = new Vector3((float)allSteps[0].Destination.Center[0], 0.5f, (float)allSteps[0].Destination.Center[1]);
            float totalDistance = 0.0f;
            for (int i = 0; i < allSteps.Count - 1; i++)
            {
                Vector3 middlePoint = GetMiddlePoint(allSteps[i].Destination, allSteps[i + 1].Destination);
                Vector3 startPoint = new Vector3((float)allSteps[i].Destination.Center[0], 0.5f, (float)allSteps[i].Destination.Center[1]);
                Vector3 endPoint = new Vector3((float)allSteps[i + 1].Destination.Center[0], 0.5f, (float)allSteps[i + 1].Destination.Center[1]);
                Vector3 prevPoint = i == 0 ? startPoint : GetMiddlePoint(allSteps[i].Destination, allSteps[i - 1].Destination);
                Vector3 nextPoint = i == allSteps.Count - 2 ? endPoint : GetMiddlePoint(allSteps[i + 1].Destination, allSteps[i + 2].Destination);
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
        public void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, Utilities.SizeOf<VertexDefinition.Path>(), 0));
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineStripWithAdjacency;
            _shader.Render(deviceContext, _vertexCount, _fleetMoveOrder.ElapsedProgress * 2, viewMatrix, projectionMatrix, (_fleetMoveOrder.ElapsedProgress + _fleetMoveOrder.CurrentStepProgress) * 2, _pathTexture, _translation, Color.Green.ToVector4(), Color.LimeGreen.ToVector4());
        }

        private static Vector3 GetMiddlePoint(VoronoiSite polyOne, VoronoiSite polyTwo)
        {
            var commonPoints = polyOne.Points.Where(p => polyTwo.Points.Contains(p)).ToList();
            var point = new Vector3((float)((commonPoints[0][0] + commonPoints[1][0]) / 2), 0.5f,
                (float)((commonPoints[0][1] + commonPoints[1][1]) / 2));
            return point;
        }

        public void Update(double delta)
        {
            _translation -= (float)delta * _fleetMoveOrder.Fleet.Speed;
        }
    }
}
