using System;
using System.Runtime.InteropServices;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Alpha.Graphics.Shaders
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
        }
        public PathShader(IRenderer renderer)
        {
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile(FileName, "VS", "vs_4_0", ShaderFlags.None);
            var pixelShaderByteCode = ShaderBytecode.CompileFromFile(FileName, "PS", "ps_4_0", ShaderFlags.None);
            var geometryShaderByteCode = ShaderBytecode.CompileFromFile(FileName, "GS", "gs_4_0", ShaderFlags.Debug);
            
            VertexShader = new VertexShader(renderer.Device, vertexShaderByteCode);
            PixelShader = new PixelShader(renderer.Device, pixelShaderByteCode);
            GeometryShader = new GeometryShader(renderer.Device, geometryShaderByteCode);
            Layout = VertexDefinition.Path.GetInputLayout(renderer.Device, vertexShaderByteCode);

            vertexShaderByteCode.Dispose();
            pixelShaderByteCode.Dispose();
            geometryShaderByteCode.Dispose();
            
            ConstantMatrixBuffer = new Buffer(renderer.Device, MatrixBufferDesription);
            ConstantPathDataBuffer = new Buffer(renderer.Device,
                new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<PathData>(),
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                });
            SamplerState = new SamplerState(renderer.Device, WrapSamplerStateDescription);
        }

        public void Render(DeviceContext deviceContext, int vertexCount, Matrix viewMatrix, Matrix projectionMatrix, int positionIndex, ShaderResourceView pathTexture, float translation)
        {
            DataStream mappedResource;
            deviceContext.MapSubresource(ConstantMatrixBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
            mappedResource.Write(new MatrixBuffer
            {
                world = Matrix.Identity,
                view = Matrix.Transpose(viewMatrix),
                projection = Matrix.Transpose(projectionMatrix)
            });
            deviceContext.UnmapSubresource(ConstantMatrixBuffer, 0);

            deviceContext.MapSubresource(ConstantPathDataBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
            mappedResource.Write(new PathData
            {
                Translation = translation,
                PositionIndex = positionIndex
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
            deviceContext.Draw(vertexCount,0);
            deviceContext.GeometryShader.Set(null);
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(VertexShader, PixelShader, Layout, ConstantMatrixBuffer);
        }
    }
}
