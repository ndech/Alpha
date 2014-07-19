using System;
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
        InputLayout Layout { get; set; }

        public PathShader(Device device)
        {
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile(FileName, "VS", "vs_4_0", ShaderFlags.None);
            var pixelShaderByteCode = ShaderBytecode.CompileFromFile(FileName, "PS", "ps_4_0", ShaderFlags.None);
            var geometryShaderByteCode = ShaderBytecode.CompileFromFile(FileName, "GS", "gs_4_0", ShaderFlags.Debug);
            
            VertexShader = new VertexShader(device, vertexShaderByteCode);
            PixelShader = new PixelShader(device, pixelShaderByteCode);
            GeometryShader = new GeometryShader(device, geometryShaderByteCode);
            Layout = VertexDefinition.PositionColor.GetInputLayout(device, vertexShaderByteCode);

            vertexShaderByteCode.Dispose();
            pixelShaderByteCode.Dispose();
            geometryShaderByteCode.Dispose();
            
            ConstantMatrixBuffer = new Buffer(device, MatrixBufferDesription);
        }

        public void Render(DeviceContext deviceContext, Int32 vertexCount, Matrix viewMatrix, Matrix projectionMatrix)
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

            deviceContext.VertexShader.SetConstantBuffer(0, ConstantMatrixBuffer);
            deviceContext.InputAssembler.InputLayout = Layout;
            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.PixelShader.Set(PixelShader);
            deviceContext.GeometryShader.Set(GeometryShader);
            deviceContext.GeometryShader.SetConstantBuffer(0, ConstantMatrixBuffer);
            deviceContext.Draw(vertexCount,0);
            deviceContext.GeometryShader.Set(null);
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(VertexShader, PixelShader, Layout, ConstantMatrixBuffer);
        }
    }
}
