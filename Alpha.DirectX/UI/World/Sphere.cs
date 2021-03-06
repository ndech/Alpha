﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Alpha.DirectX.Shaders;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Timer = Alpha.Toolkit.Timer;

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

            public Matrix Transform { get; }
            public ShaderResourceView TerrainSrv { get; set; }
            public UnorderedAccessView TerrainUav { get; set; }
            public ShaderResourceView WaterSrv { get; set; }
            public UnorderedAccessView WaterUav { get; set; }
            public UnorderedAccessView FlowsLeftUav { get; set; }
            public UnorderedAccessView FlowsTopUav { get; set; }
            public UnorderedAccessView FlowsRightUav { get; set; }
            public UnorderedAccessView FlowsBottomUav { get; set; }
        }

        private int _iterations;
        private Buffer _vertexBuffer;
        private Buffer _indexBuffer;
        private int _indexCount;
        private readonly SphericalTerrainShader _shader;
        private readonly List<Face> _faces;
        private readonly IContext _context;

        const int TextureSize = 2048;
        const int BatchSize = 32;


        public Sphere(IContext context,int faceSubdivisions, int iterations)
        {
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
            BuildBuffers(context, faceSubdivisions);
            GenerateHeightMaps();
        }
        [StructLayout(LayoutKind.Sequential)]
        struct PlaneData
        {
            public readonly Vector4 Plane;
            public readonly Matrix Rotation;

            public PlaneData(Vector4 plane, Matrix rotation)
            {
                Plane = plane;
                Rotation = rotation;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct ComputeData
        {
            public readonly uint Size;
            public readonly uint OffsetX;
            public readonly uint OffsetY;
            public readonly float Level;

            public ComputeData(uint size, uint offsetX, uint offsetY, float level)
            {
                Size = size;
                OffsetX = offsetX;
                OffsetY = offsetY;
                Level = level;
            }
        }

        private void GenerateHeightMaps()
        {
            ShaderBytecode shaderByteCode = ShaderBytecode.CompileFromFile(@"Data/Shaders/TerrainCompute.hlsl", "initTerrain", "cs_5_0", Shader.ShaderFlags);
            ComputeShader initTerrain = new ComputeShader(_context.DirectX.Device, shaderByteCode);
            shaderByteCode.Dispose();
            shaderByteCode = ShaderBytecode.CompileFromFile(@"Data/Shaders/TerrainCompute.hlsl", "initWater", "cs_5_0", Shader.ShaderFlags);
            ComputeShader initWater = new ComputeShader(_context.DirectX.Device, shaderByteCode);
            shaderByteCode.Dispose();
            shaderByteCode = ShaderBytecode.CompileFromFile(@"Data/Shaders/TerrainCompute.hlsl", "applyRandomDisplacement", "cs_5_0", Shader.ShaderFlags);
            _baseTerrainGeneration = new ComputeShader(_context.DirectX.Device, shaderByteCode);
            shaderByteCode.Dispose();
            shaderByteCode = ShaderBytecode.CompileFromFile(@"Data/Shaders/TerrainCompute.hlsl", "flowsCalculation", "cs_5_0", Shader.ShaderFlags);
            _flowsCalculation = new ComputeShader(_context.DirectX.Device, shaderByteCode);
            shaderByteCode.Dispose();
            shaderByteCode = ShaderBytecode.CompileFromFile(@"Data/Shaders/TerrainCompute.hlsl", "updateWaterLevel", "cs_5_0", Shader.ShaderFlags);
            _updateWaterLevel = new ComputeShader(_context.DirectX.Device, shaderByteCode);
            shaderByteCode.Dispose();
            
            Texture2DDescription textureDescription = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R32_Float,
                Height = TextureSize,
                Width = TextureSize,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };

            ConstantBuffer<ComputeData> computeBuffer = new ConstantBuffer<ComputeData>(_context);
            _context.DirectX.DeviceContext.ComputeShader.SetConstantBuffer(1, computeBuffer.Buffer);

            foreach (Face face in _faces)
            {
                Texture2D terrainTexture = new Texture2D(_context.DirectX.Device, textureDescription);
                face.TerrainSrv = new ShaderResourceView(_context.DirectX.Device, terrainTexture);
                face.TerrainUav = new UnorderedAccessView(_context.DirectX.Device, terrainTexture);
                terrainTexture.Dispose();

                Texture2D waterTexture = new Texture2D(_context.DirectX.Device, textureDescription);
                face.WaterSrv = new ShaderResourceView(_context.DirectX.Device, waterTexture);
                face.WaterUav = new UnorderedAccessView(_context.DirectX.Device, waterTexture);
                waterTexture.Dispose();

                Texture2D flowsTexture = new Texture2D(_context.DirectX.Device, textureDescription);
                face.FlowsLeftUav = new UnorderedAccessView(_context.DirectX.Device, flowsTexture);
                flowsTexture.Dispose();

                flowsTexture = new Texture2D(_context.DirectX.Device, textureDescription);
                face.FlowsTopUav = new UnorderedAccessView(_context.DirectX.Device, flowsTexture);
                flowsTexture.Dispose();

                flowsTexture = new Texture2D(_context.DirectX.Device, textureDescription);
                face.FlowsRightUav = new UnorderedAccessView(_context.DirectX.Device, flowsTexture);
                flowsTexture.Dispose();

                flowsTexture = new Texture2D(_context.DirectX.Device, textureDescription);
                face.FlowsBottomUav = new UnorderedAccessView(_context.DirectX.Device, flowsTexture);
                flowsTexture.Dispose();

                _context.DirectX.DeviceContext.ComputeShader.SetUnorderedAccessView(0, face.TerrainUav);
                _context.DirectX.DeviceContext.ComputeShader.SetUnorderedAccessView(1, face.WaterUav);

                _context.DirectX.DeviceContext.ComputeShader.Set(initTerrain);
                computeBuffer.Update(new ComputeData(TextureSize - 1 - BatchSize, 0, 0, 0.0f));
                _context.DirectX.DeviceContext.Dispatch(TextureSize / BatchSize, TextureSize / BatchSize, 1);

                _context.DirectX.DeviceContext.ComputeShader.Set(initWater);
                computeBuffer.Update(new ComputeData(TextureSize - 1 - BatchSize, 0, 0, 0.05f));
                _context.DirectX.DeviceContext.Dispatch(TextureSize / BatchSize, TextureSize / BatchSize, 1);

                _context.DirectX.DeviceContext.ComputeShader.Set(initTerrain);
                computeBuffer.Update(new ComputeData(TextureSize - 1 - BatchSize, BatchSize / 2, BatchSize / 2, 0.5f));
                _context.DirectX.DeviceContext.Dispatch(TextureSize / BatchSize - 1, TextureSize / BatchSize - 1, 1);
            }

             _planeBuffer = new ConstantBuffer<PlaneData>(_context);

            initTerrain.Dispose();
            computeBuffer.Dispose();
        }

        private ConstantBuffer<PlaneData> _planeBuffer;
        private ComputeShader _baseTerrainGeneration;
        private ComputeShader _flowsCalculation;
        private ComputeShader _updateWaterLevel;


        private Timer _timer = new Timer();
        public void Update(double delta)
        {
            if (_iterations > 0)
            {
                _context.DirectX.DeviceContext.PixelShader.SetShaderResource(0, null);
                _context.DirectX.DeviceContext.VertexShader.SetShaderResource(0, null);
                _timer.Tick();
                for (int i = 0; i < 14 && _iterations > 0; i++)
                {
                    Vector3 normal = new Vector3((float)RandomGenerator.GetDouble(-1, 1), (float)RandomGenerator.GetDouble(-1, 1), (float)RandomGenerator.GetDouble(-1, 1));
                    Vector4 data = new Vector4(Vector3.Normalize(normal), (float)RandomGenerator.GetDouble(-1, 1));
                    foreach (Face face in _faces)
                    {
                        _planeBuffer.Update(new PlaneData(data, Matrix.Transpose(face.Transform)));
                        _context.DirectX.DeviceContext.ComputeShader.Set(_baseTerrainGeneration);
                        _context.DirectX.DeviceContext.ComputeShader.SetUnorderedAccessView(0, face.TerrainUav);
                        _context.DirectX.DeviceContext.ComputeShader.SetConstantBuffer(0, _planeBuffer.Buffer);
                        _context.DirectX.DeviceContext.Dispatch(TextureSize / BatchSize - 1, TextureSize / BatchSize - 1, 1);
                    }
                    _iterations--;
                }

                _context.DirectX.DeviceContext.ComputeShader.SetUnorderedAccessView(0, null);
                _context.DirectX.DeviceContext.ComputeShader.SetUnorderedAccessView(1, null);
            }
            else if (_iterations > -5000)
            {
                _context.DirectX.DeviceContext.PixelShader.SetShaderResource(0, null);
                _context.DirectX.DeviceContext.VertexShader.SetShaderResource(0, null);
                _context.DirectX.DeviceContext.PixelShader.SetShaderResource(1, null);
                _context.DirectX.DeviceContext.VertexShader.SetShaderResource(1, null);
                _timer.Tick();
                for (int i = 0; i < 1 && _iterations > -5000; i++)
                {
                    foreach (Face face in _faces)
                    {
                        _context.DirectX.DeviceContext.ComputeShader.SetUnorderedAccessView(0, face.TerrainUav);
                        _context.DirectX.DeviceContext.ComputeShader.SetUnorderedAccessView(1, face.WaterUav);
                        _context.DirectX.DeviceContext.ComputeShader.SetUnorderedAccessView(2, face.FlowsLeftUav);
                        _context.DirectX.DeviceContext.ComputeShader.SetUnorderedAccessView(3, face.FlowsTopUav);
                        _context.DirectX.DeviceContext.ComputeShader.SetUnorderedAccessView(4, face.FlowsRightUav);
                        _context.DirectX.DeviceContext.ComputeShader.SetUnorderedAccessView(5, face.FlowsBottomUav);
                        _context.DirectX.DeviceContext.ComputeShader.Set(_flowsCalculation);
                        _context.DirectX.DeviceContext.Dispatch(TextureSize / BatchSize - 1, TextureSize / BatchSize - 1, 1);
                        _context.DirectX.DeviceContext.ComputeShader.Set(_updateWaterLevel);
                        _context.DirectX.DeviceContext.Dispatch(TextureSize / BatchSize - 1, TextureSize / BatchSize - 1, 1);
                    }
                    _iterations--;
                }

                _context.DirectX.DeviceContext.ComputeShader.SetUnorderedAccessView(0, null);
                _context.DirectX.DeviceContext.ComputeShader.SetUnorderedAccessView(1, null);
                _context.DirectX.DeviceContext.ComputeShader.SetUnorderedAccessView(2, null);
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
                        texture = new Vector2(
                            ((1.0f + BatchSize) / (2 * TextureSize)) + ((i * (TextureSize - BatchSize - 1.0f)) / ((faceSubdivisions ) * TextureSize)),
                            ((1.0f + BatchSize) / (2 * TextureSize)) + ((j * (TextureSize - BatchSize - 1.0f)) / ((faceSubdivisions ) * TextureSize)))
                    };

            _indexCount = faceSubdivisions * faceSubdivisions * 6;
            uint[] indices = new uint[_indexCount];
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
                _shader.Render(deviceContext, _indexCount, face.Transform * worldMatrix, viewMatrix, projectionMatrix, face.TerrainSrv, face.WaterSrv);
        }
        
        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(_vertexBuffer, _indexBuffer, _baseTerrainGeneration, _planeBuffer);
            foreach (Face face in _faces)
                DisposeHelper.DisposeAndSetToNull(face.TerrainSrv, face.TerrainUav);
        }
    }
}
