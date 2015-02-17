using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Alpha.DirectX.Shaders;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Alpha.DirectX.UI.World
{
    class Sphere : IDisposable
    {

        private class Face
        {
            public Face(Matrix transform)
            {
                Transform = transform;
            }

            public Matrix Transform { get; set; }
            public ShaderResourceView ShaderResourceView { get; set; }
            public UnorderedAccessView UnorderedAccessView { get; set; }
        }
        private readonly Color _color;
        private readonly int _iterations;
        private Buffer _vertexBuffer;
        private Buffer _indexBuffer;
        private int _indexCount;
        private readonly SphericalTerrainShader _shader;
        private List<ShaderResourceView> _heightMaps;
        private List<Face> _faces;
        private IContext _context;


        public Sphere(IContext context, Color color, int faceSubdivisions, int iterations)
        {
            _color = color;
            _iterations = iterations;
            _context = context;
            _shader = context.Shaders.Get<SphericalTerrainShader>();
            _faces = new List<Face>
            {
                new Face(Matrix.Identity),
                new Face(Matrix.RotationX(-MathUtil.Pi)),
                new Face(Matrix.RotationX(-MathUtil.PiOverTwo)),
                new Face(Matrix.RotationX(MathUtil.PiOverTwo)),
                new Face(Matrix.RotationY(-MathUtil.PiOverTwo)),
                new Face(Matrix.RotationY(MathUtil.PiOverTwo))
            };
            _heightMaps = new List<ShaderResourceView>(context.TextureManager.Create("Sky.png").TextureResource.Yield().Times(6));
            BuildBuffers(context, faceSubdivisions);
            GenerateHeightMaps(faceSubdivisions);
        }
        [StructLayout(LayoutKind.Sequential)]
        struct ComputeData
        {
            public readonly Vector4 Plane;
            public readonly Matrix Rotation;

            public ComputeData(Vector4 plane, Matrix rotation)
            {
                Plane = plane;
                Rotation = rotation;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct TextureSizeData
        {
            public readonly float Size;
            public readonly Vector3 Padding;

            public TextureSizeData(float size)
            {
                Size = size;
                Padding = new Vector3();
            }
        }

        private void GenerateHeightMaps(int faceSubdivisions)
        {
            ShaderBytecode shaderByteCode = ShaderBytecode.CompileFromFile(@"Data/Shaders/TerrainCompute.hlsl", "init", "cs_5_0", Shader.ShaderFlags);
            ComputeShader initTerrain = new ComputeShader(_context.DirectX.Device, shaderByteCode);
            shaderByteCode.Dispose();
            shaderByteCode = ShaderBytecode.CompileFromFile(@"Data/Shaders/TerrainCompute.hlsl", "applyRandomDisplacement", "cs_5_0", Shader.ShaderFlags);
            ComputeShader baseTerrainGeneration = new ComputeShader(_context.DirectX.Device, shaderByteCode);
            shaderByteCode.Dispose();
            const int textureSize = 1024;
            Texture2DDescription textureDescription = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R32_Float,
                Height = textureSize,
                Width = textureSize,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };

            Buffer planeBuffer = new Buffer(_context.DirectX.Device,
                new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<ComputeData>(),
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                });
            Buffer textureSizeBuffer = new Buffer(_context.DirectX.Device,
                new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<TextureSizeData>(),
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                });
            DataStream mappedResource;
            _context.DirectX.DeviceContext.MapSubresource(textureSizeBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);
            mappedResource.Write(new TextureSizeData(textureSize));
            _context.DirectX.DeviceContext.UnmapSubresource(textureSizeBuffer, 0);
            _context.DirectX.DeviceContext.ComputeShader.SetConstantBuffer(1, textureSizeBuffer);

            foreach (Face face in _faces)
            {

                Texture2D texture = new Texture2D(_context.DirectX.Device, textureDescription);
                face.ShaderResourceView = new ShaderResourceView(_context.DirectX.Device, texture);
                face.UnorderedAccessView = new UnorderedAccessView(_context.DirectX.Device, texture);

                _context.DirectX.DeviceContext.ComputeShader.Set(initTerrain);
                _context.DirectX.DeviceContext.ComputeShader.SetUnorderedAccessView(0, face.UnorderedAccessView);
                _context.DirectX.DeviceContext.Dispatch(textureSize / 32, textureSize/32, 1);
            }



            for (int i = 0; i < _iterations; i++)
            {
                Vector3 normal = new Vector3((float)RandomGenerator.GetDouble(-1, 1), (float)RandomGenerator.GetDouble(-1, 1), (float)RandomGenerator.GetDouble(-1, 1));
                Vector4 data = new Vector4(Vector3.Normalize(normal), (float)RandomGenerator.GetDouble(-1, 1));
                foreach (Face face in _faces)
                {
                    _context.DirectX.DeviceContext.MapSubresource(planeBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);
                    mappedResource.Write(new ComputeData(data, Matrix.Transpose(face.Transform)));
                    _context.DirectX.DeviceContext.UnmapSubresource(planeBuffer, 0);
                    _context.DirectX.DeviceContext.ComputeShader.Set(baseTerrainGeneration);
                    _context.DirectX.DeviceContext.ComputeShader.SetUnorderedAccessView(0, face.UnorderedAccessView);
                    _context.DirectX.DeviceContext.ComputeShader.SetConstantBuffer(0, planeBuffer);
                    _context.DirectX.DeviceContext.Dispatch(textureSize / 32, textureSize / 32, 1);
                }
            }
            _context.DirectX.DeviceContext.ComputeShader.SetUnorderedAccessView(0, null);
            initTerrain.Dispose();
            baseTerrainGeneration.Dispose();
            planeBuffer.Dispose();
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

        private void BuildBuffers(IContext context, int faceSubdivisions)
        {
            int vertexCount = (faceSubdivisions + 1) * (faceSubdivisions + 1);
            VertexDefinition.SphericalTerrainVertex[] vertices = new VertexDefinition.SphericalTerrainVertex[vertexCount];
            for (int i = 0; i < (faceSubdivisions + 1); i++)
                for (int j = 0; j < (faceSubdivisions + 1); j++)
                    vertices[j * (faceSubdivisions + 1) + i] = new VertexDefinition.SphericalTerrainVertex
                    {
                        position = Vector3.Normalize(new Vector3((float)(-(1.0) + (double)2*i / faceSubdivisions), (float)(-(1.0) + (double)2*j / faceSubdivisions), -1.0f)),
                        texture = new Vector2((float)i / faceSubdivisions, (float)j / faceSubdivisions)
                    };

            _indexCount = faceSubdivisions * faceSubdivisions * 6;
            UInt32[] indices = new UInt32[_indexCount];
            for (int i = 0; i < (faceSubdivisions); i++)
                for (int j = 0; j < (faceSubdivisions); j++)
                {
                    indices[(i * faceSubdivisions + j) * 6] = (uint)(i * (faceSubdivisions + 1) + j + 1); //Left top
                    indices[(i * faceSubdivisions + j) * 6 + 1] = (uint)(i * (faceSubdivisions + 1) + j); //Left bottom
                    indices[(i * faceSubdivisions + j) * 6 + 2] = (uint)((i + 1) * (faceSubdivisions + 1) + j); //Right bottom
                    indices[(i * faceSubdivisions + j) * 6 + 3] = (uint)(i * (faceSubdivisions + 1) + j + 1); //Left top
                    indices[(i * faceSubdivisions + j) * 6 + 4] = (uint)((i + 1) * (faceSubdivisions + 1) + j); //Right bottom
                    indices[(i * faceSubdivisions + j) * 6 + 5] = (uint)((i + 1) * (faceSubdivisions + 1) + j + 1); //Right top
                }
            _vertexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.VertexBuffer, vertices);
            _indexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.IndexBuffer, indices);








            //List<Vector3> vertices = new List<Vector3> 
            //{
            //    new Vector3(-1.0f, -1.0f,  1.0f),
            //    new Vector3(1.0f, -1.0f,  1.0f),
            //    new Vector3(1.0f,  1.0f,  1.0f),
            //    new Vector3(-1.0f,  1.0f,  1.0f),
            //    new Vector3(-1.0f, -1.0f, -1.0f),
            //    new Vector3(1.0f, -1.0f, -1.0f),
            //    new Vector3(1.0f,  1.0f, -1.0f),
            //    new Vector3(-1.0f,  1.0f, -1.0f)
            //};
            //List<int> indices = new List<int>
            //{
            //    // front
            //    0, 1, 2,
            //    2, 3, 0,
            //    // top
            //    3, 2, 6,
            //    6, 7, 3,
            //    // back
            //    7, 6, 5,
            //    5, 4, 7,
            //    // bottom
            //    4, 5, 1,
            //    1, 0, 4,
            //    // left
            //    4, 0, 3,
            //    3, 7, 4,
            //    // right
            //    1, 5, 6,
            //    6, 2, 1
            //};

            //for (int i = 0; i < subdivisionLevel; i++)
            //    Subdivide(vertices, indices);

            //List<Vertex> verticesDispersion = vertices.Select(v=> new Vertex(0,Vector3.Normalize(v))).ToList();

            //for (int i = 0; i < _iterations; i++)
            //{
            //    Vector3 normal = new Vector3((float)RandomGenerator.GetDouble(-1, 1), (float)RandomGenerator.GetDouble(-1, 1), (float)RandomGenerator.GetDouble(-1, 1));
            //    normal.Normalize();
            //    double offset = RandomGenerator.GetDouble(-1, 1);
            //    Func<Vector3, bool> isOutOfPlane = (v) => normal.X*v.X + normal.Y*v.Y + normal.Z*v.Z - offset > 0;
            //    foreach (Vertex vertex in verticesDispersion)
            //    {
            //        vertex.Offset += (isOutOfPlane(vertex.Position) ? 1 : -1);
            //    }
            //}

            //double averageOffset = verticesDispersion.Average(v => v.Offset);
            //int maxOffset = verticesDispersion.Max(v => v.Offset);
            //Vector3 averagePosition = verticesDispersion.Select(v => v.Position).AverageVector();          
            //_indexCount = indices.Count;
            //_vertexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.VertexBuffer, verticesDispersion.Select(v => new VertexDefinition.PositionColor
            //{
            //    position = (v.Position-averagePosition)*(1-(float)((v.Offset-averageOffset)/(15*(maxOffset+1)))),
            //    color = _color.ToVector4()
            //}).ToArray());
            //_indexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.IndexBuffer, indices.ToArray());
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
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, Utilities.SizeOf<VertexDefinition.SphericalTerrainVertex>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            foreach (Face face in _faces)
                _shader.Render(deviceContext, _indexCount, face.Transform * worldMatrix, viewMatrix, projectionMatrix, face.ShaderResourceView);
        }
        
        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(_vertexBuffer, _indexBuffer);
        }
    }
}
