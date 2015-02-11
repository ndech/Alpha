using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.DirectX.Shaders;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.DXGI;

namespace Alpha.DirectX.UI.World
{
    class Sphere
    {
        private readonly Color _color;
        private readonly int _iterations;
        private Buffer _vertexBuffer;
        private Buffer _indexBuffer;
        private int _indexCount;
        private readonly ColorShader _shader;


        public Sphere(IContext context, Color color, int subdivisionLevel, int iterations)
        {
            _color = color;
            _iterations = iterations;
            _shader = context.Shaders.Get<ColorShader>();
            BuildBuffers(context, subdivisionLevel);
        }
        
        public void Update(double delta)
        { }

        class Vertex
        {
            public Vector3 Position;
            public int Offset;

            public Vertex(int offset, Vector3 position)
            {
                Offset = offset;
                Position = position;
            }
        }

        private void BuildBuffers(IContext context, int subdivisionLevel)
        {
            List<Vector3> vertices = new List<Vector3> 
            {
                new Vector3(-1.0f, -1.0f,  1.0f),
                new Vector3(1.0f, -1.0f,  1.0f),
                new Vector3(1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3(1.0f, -1.0f, -1.0f),
                new Vector3(1.0f,  1.0f, -1.0f),
                new Vector3(-1.0f,  1.0f, -1.0f)
            };
            List<int> indices = new List<int>
            {
                // front
                0, 1, 2,
                2, 3, 0,
                // top
                3, 2, 6,
                6, 7, 3,
                // back
                7, 6, 5,
                5, 4, 7,
                // bottom
                4, 5, 1,
                1, 0, 4,
                // left
                4, 0, 3,
                3, 7, 4,
                // right
                1, 5, 6,
                6, 2, 1
            };

            for (int i = 0; i < subdivisionLevel; i++)
                Subdivide(vertices, indices);

            List<Vertex> verticesDispersion = vertices.Select(v=> new Vertex(0,Vector3.Normalize(v))).ToList();

            for (int i = 0; i < _iterations; i++)
            {
                Vector3 normal = new Vector3((float)RandomGenerator.GetDouble(-1, 1), (float)RandomGenerator.GetDouble(-1, 1), (float)RandomGenerator.GetDouble(-1, 1));
                normal.Normalize();
                double offset = RandomGenerator.GetDouble(-1, 1);
                Func<Vector3, bool> isOutOfPlane = (v) => normal.X*v.X + normal.Y*v.Y + normal.Z*v.Z - offset > 0;
                foreach (Vertex vertex in verticesDispersion)
                {
                    vertex.Offset += (isOutOfPlane(vertex.Position) ? 1 : -1);
                }
            }

            double averageOffset = verticesDispersion.Average(v => v.Offset);
            int maxOffset = verticesDispersion.Max(v => v.Offset);
            Vector3 averagePosition = verticesDispersion.Select(v => v.Position).AverageVector();          
            _indexCount = indices.Count;
            _vertexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.VertexBuffer, verticesDispersion.Select(v => new VertexDefinition.PositionColor
            {
                position = (v.Position-averagePosition)*(1-(float)((v.Offset-averageOffset)/(50*(maxOffset+1)))),
                color = _color.ToVector4()
            }).ToArray());
            _indexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.IndexBuffer, indices.ToArray());
        }

        private void Subdivide(List<Vector3> vertices, List<int> indices)
        {
            var midpointIndices = new Dictionary<string, int>();

            var newIndices = new List<int>(indices.Count * 4);

            for (var i = 0; i < indices.Count - 2; i += 3)
            {
                var i0 = indices[i];
                var i1 = indices[i + 1];
                var i2 = indices[i + 2];

                var m01 = GetMidpointIndex(midpointIndices, vertices, i0, i1);
                var m12 = GetMidpointIndex(midpointIndices, vertices, i1, i2);
                var m02 = GetMidpointIndex(midpointIndices, vertices, i2, i0);

                newIndices.AddRange(
                    new[] {
                    i0,m01,m02
                    ,
                    i1,m12,m01
                    ,
                    i2,m02,m12
                    ,
                    m02,m01,m12
                }
                    );

            }

            indices.Clear();
            indices.AddRange(newIndices);
        }

        private static int GetMidpointIndex(Dictionary<string, int> midpointIndices, List<Vector3> vertices, int i0, int i1)
        {
            var edgeKey = string.Format("{0}_{1}", Math.Min(i0, i1), Math.Max(i0, i1));

            int midpointIndex;

            if (!midpointIndices.TryGetValue(edgeKey, out midpointIndex))
            {
                var v0 = vertices[i0];
                var v1 = vertices[i1];

                var midpoint = (v0 + v1) / 2f;

                if (vertices.Contains(midpoint))
                    midpointIndex = vertices.IndexOf(midpoint);
                else
                {
                    midpointIndex = vertices.Count;
                    vertices.Add(midpoint);
                }
            }
            return midpointIndex;
        }

        public void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, Utilities.SizeOf<VertexDefinition.WaterVertex>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _shader.Render(deviceContext, _indexCount, Matrix.Scaling(200) * worldMatrix, viewMatrix, projectionMatrix);
        }
    }
}
