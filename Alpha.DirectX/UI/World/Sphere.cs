﻿using System;
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
            const int textureSize = 256;
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
            mappedResource.Write(new TextureSizeData(textureSize-1));
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
        
        private void BuildBuffers(IContext context, int faceSubdivisions)
        {
            int vertexCount = (faceSubdivisions + 1) * (faceSubdivisions + 1);
            VertexDefinition.SphericalTerrainVertex[] vertices = new VertexDefinition.SphericalTerrainVertex[vertexCount];
            for (int i = 0; i < (faceSubdivisions + 1); i++)
                for (int j = 0; j < (faceSubdivisions + 1); j++)
                    vertices[j * (faceSubdivisions + 1) + i] = new VertexDefinition.SphericalTerrainVertex
                    {
                        position = Vector3.Normalize(new Vector3((float)(-(1.0) + (double)2*i / faceSubdivisions), (float)(-(1.0) + (double)2*j / faceSubdivisions), -1.0f)),
                        texture = new Vector2((0.5f + i) / (faceSubdivisions + 1), (0.5f + j) / (faceSubdivisions + 1)),
                        //texture = new Vector2( (float)i / faceSubdivisions , (float)j / (faceSubdivisions))
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
