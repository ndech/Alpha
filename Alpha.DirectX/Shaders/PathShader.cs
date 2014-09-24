using System;
using System.Runtime.InteropServices;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Alpha.DirectX.Shaders
{
    class PathShader : Shader, IDisposable
    {
        private const string FileName = @"Data/Shaders/Path.hlsl";
        VertexShader VertexShader { get; set; }
        PixelShader PixelShader { get; set; }
        GeometryShader GeometryShader { get; set; }
        Buffer ConstantMatrixBuffer { get; set; }
        Buffer ConstantPathDataBuffer { get; set; }
        SamplerState SamplerState { get; set; }
        InputLayout Layout { get; set; }

        [StructLayout(LayoutKind.Sequential)]
        private struct PathData
        {
            public float Translation;
            public int PositionIndex;
            private readonly Vector2 padding;
            public Vector4 MainColor;
            public Vector4 BackgroundColor;
        }

        public PathShader(Device device)
        {
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile(FileName, "VS", "vs_4_0", ShaderFlags);
            var pixelShaderByteCode = ShaderBytecode.CompileFromFile(FileName, "PS", "ps_4_0", ShaderFlags);
            var geometryShaderByteCode = ShaderBytecode.CompileFromFile(FileName, "GS", "gs_4_0", ShaderFlags);
            
            VertexShader = new VertexShader(device, vertexShaderByteCode);
            PixelShader = new PixelShader(device, pixelShaderByteCode);
            GeometryShader = new GeometryShader(device, geometryShaderByteCode);
            Layout = VertexDefinition.Path.GetInputLayout(device, vertexShaderByteCode);

            vertexShaderByteCode.Dispose();
            pixelShaderByteCode.Dispose();
            geometryShaderByteCode.Dispose();
            
            ConstantMatrixBuffer = new Buffer(device, MatrixBufferDesription);
            ConstantPathDataBuffer = new Buffer(device,
                new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<PathData>(),
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                });
            SamplerState = new SamplerState(device, WrapSamplerStateDescription);
        }

        public void Render(DeviceContext deviceContext, int vertexCount, int vertexOffset, Matrix viewMatrix, Matrix projectionMatrix, int positionIndex, ShaderResourceView pathTexture, double translation, Vector4 mainColor, Vector4 backgroundColor)
        {
            DataStream mappedResource;
            UpdateMatrixBuffer(deviceContext, ConstantMatrixBuffer, Matrix.Identity, viewMatrix, projectionMatrix);

            deviceContext.MapSubresource(ConstantPathDataBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
            mappedResource.Write(new PathData
            {
                Translation = (float)translation,
                PositionIndex = positionIndex,
                MainColor = mainColor,
                BackgroundColor = backgroundColor
            });
            deviceContext.UnmapSubresource(ConstantPathDataBuffer, 0);

            deviceContext.InputAssembler.InputLayout = Layout;
            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.PixelShader.Set(PixelShader);
            deviceContext.PixelShader.SetSampler(0, SamplerState);
            deviceContext.PixelShader.SetShaderResource(0, pathTexture);
            deviceContext.PixelShader.SetConstantBuffer(1, ConstantPathDataBuffer);
            deviceContext.GeometryShader.Set(GeometryShader);
            deviceContext.GeometryShader.SetConstantBuffer(0, ConstantMatrixBuffer);
            deviceContext.GeometryShader.SetConstantBuffer(1, ConstantPathDataBuffer);
            deviceContext.Draw(vertexCount-vertexOffset ,vertexOffset);
            deviceContext.GeometryShader.Set(null);
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(VertexShader, PixelShader, Layout, ConstantMatrixBuffer);
        }
    }
}
