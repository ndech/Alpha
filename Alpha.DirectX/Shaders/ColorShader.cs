using Alpha.Toolkit;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace Alpha.DirectX.Shaders
{
    class ColorShader : Shader
    {
        private const string ShaderFileName = @"Data/Shaders/Color.hlsl";
        VertexShader VertexShader { get; }
        PixelShader PixelShader { get; }
        InputLayout Layout { get; }
        Buffer ConstantMatrixBuffer { get; }

        public ColorShader(Device device)
        {
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile(ShaderFileName, "ColorVertexShader", "vs_4_0", ShaderFlags);
            var pixelShaderByteCode = ShaderBytecode.CompileFromFile(ShaderFileName, "ColorPixelShader", "ps_4_0", ShaderFlags);

            VertexShader = new VertexShader(device, vertexShaderByteCode);
            PixelShader = new PixelShader(device, pixelShaderByteCode);

            Layout = VertexDefinition.PositionColor.GetInputLayout(device, vertexShaderByteCode);

            vertexShaderByteCode.Dispose();
            pixelShaderByteCode.Dispose();

            ConstantMatrixBuffer = new Buffer(device, MatrixBufferDesription);
        }

        public void Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            UpdateMatrixBuffer(deviceContext, ConstantMatrixBuffer, worldMatrix, viewMatrix, projectionMatrix);
            deviceContext.VertexShader.SetConstantBuffer(0, ConstantMatrixBuffer);
            deviceContext.InputAssembler.InputLayout = Layout;
            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.PixelShader.Set(PixelShader);
            deviceContext.DrawIndexed(indexCount, 0, 0);
        }
        public void RenderNotIndexed(DeviceContext deviceContext, int vertexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            UpdateMatrixBuffer(deviceContext, ConstantMatrixBuffer, worldMatrix, viewMatrix, projectionMatrix);
            deviceContext.VertexShader.SetConstantBuffer(0, ConstantMatrixBuffer);
            deviceContext.InputAssembler.InputLayout = Layout;
            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.PixelShader.Set(PixelShader);
            deviceContext.Draw(vertexCount,0);
        }

        public override void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(VertexShader, PixelShader, Layout, ConstantMatrixBuffer);
        }
    }
}
