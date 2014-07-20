using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Graphics;
using Alpha.Graphics.Shaders;
using Alpha.WorldGeneration;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.Direct3D;

namespace Alpha
{
    class FleetMoveOrder : IDailyUpdatable
    {
        public class Step
        {
            public VoronoiSite Destination { get; set; }
            public Int32 Duration { get; set; }

            public Step(VoronoiSite destination, Int32 duration)
            {
                Destination = destination;
                Duration = duration;
            }
        }
        public VoronoiSite Destination { get; set; }
        public List<Step> Steps { get; set; } 
        public Fleet Fleet { get; set; }
        public Int32 Progress { get; set; }
        private PathShader Shader { get; set; }
        public Int32 VertexCount { get; set; }
        public Buffer VertexBuffer { get; set; }
        ShaderResourceView PathTexture { get; set; }

        public FleetMoveOrder(IRenderer renderer, Fleet fleet, VoronoiSite destination, List<Step> steps)
        {
            Progress = 0;
            Fleet = fleet;
            Destination = destination;
            Steps = steps;
            Shader = renderer.PathShader;
            Vector4 color = Color.Green.ToVector4();

            List<Step> allSteps;
            (allSteps = new List<Step> {new Step(fleet.Location, 0)}).AddRange(Steps);

            List<VertexDefinition.Path> vertices = new List<VertexDefinition.Path>();
            int filling = 0;
            Vector3 position, prevPosition = new Vector3((float)fleet.Location.Center[0], 0.5f, (float)fleet.Location.Center[1]);
            float totalDistance = 0.0f, arcLength;
            for (int i = 0; i < allSteps.Count-1; i++)
            {
                Vector3 middlePoint = GetMiddlePoint(allSteps[i].Destination, allSteps[i+1].Destination);
                Vector3 startPoint = new Vector3((float)allSteps[i].Destination.Center[0], 0.5f, (float)allSteps[i].Destination.Center[1]);
                Vector3 endPoint = new Vector3((float)allSteps[i + 1].Destination.Center[0], 0.5f, (float)allSteps[i + 1].Destination.Center[1]);
                Vector3 prevPoint = i == 0 ? startPoint : GetMiddlePoint(allSteps[i].Destination, allSteps[i - 1].Destination);
                Vector3 nextPoint = i == allSteps.Count - 2 ? endPoint : GetMiddlePoint(allSteps[i+1].Destination, allSteps[i+2].Destination);
                int subdivisions = allSteps[i + 1].Duration;
                //First part of the arc
                for (int j = 0; j < subdivisions; j++)
                {
                    position = Vector3.CatmullRom(prevPoint, startPoint, middlePoint, endPoint, (1.0f*j)/subdivisions);
                    arcLength = Vector3.Distance(prevPosition, position);
                    vertices.Add(new VertexDefinition.Path
                    {
                        color = color,
                        fillingIndex = (uint)((++filling)/2),
                        position = position,
                        pathLength = new Vector2(totalDistance,arcLength)
                    });
                    totalDistance += arcLength;
                    prevPosition = position;
                }
                //Second part of the arc
                for (int j = 0; j < subdivisions; j++)
                {
                    position = Vector3.CatmullRom(startPoint, middlePoint, endPoint, nextPoint, (1.0f*j)/subdivisions);
                    arcLength = Vector3.Distance(prevPosition, position);
                    vertices.Add(new VertexDefinition.Path
                    {
                        color = color,
                        fillingIndex = (uint)((++filling) / 2),
                        position = position,
                        pathLength = new Vector2(totalDistance, arcLength)
                    });
                    totalDistance += arcLength;
                    prevPosition = position;
                }
            }
            VertexCount = vertices.Count;
            VertexBuffer = Buffer.Create(renderer.Device, BindFlags.VertexBuffer, vertices.ToArray());
            PathTexture = renderer.TextureManager.Create("Path.png").TextureResource;
        }

        private static Vector3 GetMiddlePoint(VoronoiSite polyOne, VoronoiSite polyTwo)
        {
            var commonPoints = polyOne.Points.Where(p => polyTwo.Points.Contains(p)).ToList();
            var point = new Vector3((float) ((commonPoints[0][0] + commonPoints[1][0])/2), 0.5f,
                (float) ((commonPoints[0][1] + commonPoints[1][1])/2));
            return point;
        }

        public void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<VertexDefinition.Path>(), 0));
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineStripWithAdjacency;
            Shader.Render(deviceContext, VertexCount, viewMatrix, projectionMatrix, Progress, PathTexture, Translation);
        }

        public float Translation { get; set; }

        public void DayUpdate()
        {
            Progress++;
        }

        public void Update(double delta)
        {
            Translation += (float) delta*2;
        }
    }
}
