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
    class FleetMoveOrder
    {
        public class Step
        {
            public VoronoiSite Destination { get; set; }
            private Int32 Duration { get; set; }

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

        public FleetMoveOrder(IRenderer renderer, Fleet fleet, VoronoiSite destination, List<Step> steps)
        {
            Progress = 0;
            Fleet = fleet;
            Destination = destination;
            Steps = steps;
            Shader = renderer.PathShader;

            List<Vector3> positions = new List<Vector3>
            { new Vector3((float) fleet.Location.Center[0], 1.0f, (float) fleet.Location.Center[1]) };

            for (int i = 0; i < Steps.Count; i++)
            {
                VoronoiSite polyOne = Steps[i].Destination;
                VoronoiSite polyTwo = i == 0 ? fleet.Location : Steps[i - 1].Destination;
                var commonPoints = polyOne.Points.Where(p => polyTwo.Points.Contains(p)).ToList();

                positions.Add(new Vector3((float)((commonPoints[0][0] + commonPoints[1][0]) / 2), 0.05f, (float)((commonPoints[0][1] + commonPoints[1][1]) / 2)));
                positions.Add(new Vector3((float)Steps[i].Destination.Center[0], 1.0f, (float)Steps[i].Destination.Center[1]));
            }
            
            const int subdivisions = 10;
            var newPositions = positions.SelectMany((p, i) =>
            {
                List<Vector3> newPoints = new List<Vector3>();
                if (i == positions.Count - 1)
                    return newPoints;
                for (int j = 0; j < subdivisions; j++)
                    newPoints.Add(Vector3.CatmullRom(positions[Math.Max(0, i - 1)], positions[i],
                        positions[Math.Min(positions.Count-1, i + 1)]
                        , positions[Math.Min(positions.Count-1, i + 2)], (1.0f*j)/subdivisions));
                return newPoints;
            }).ToList();
            positions = newPositions;
            VertexDefinition.PositionColor[] vertices = positions.Select((p) => new VertexDefinition.PositionColor
            { position = p, color = Color.Red.ToVector4() }).ToArray();
            VertexCount = vertices.Count();
            VertexBuffer = Buffer.Create(renderer.Device, BindFlags.VertexBuffer, vertices);
        }

        public void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<VertexDefinition.PositionColor>(), 0));
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineStripWithAdjacency;
            Shader.Render(deviceContext, VertexCount, viewMatrix, projectionMatrix);
        }
    }
}
