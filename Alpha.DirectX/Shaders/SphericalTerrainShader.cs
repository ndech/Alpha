using Alpha.Toolkit;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace Alpha.DirectX.Shaders
{
    class SphericalTerrainShader : Shader
    {
        private const string ShaderFileName = @"Data/Shaders/SphericalTerrain.hlsl";
        VertexShader VertexShader { get; }
        PixelShader PixelShader { get; }
        InputLayout Layout { get; }
        Buffer ConstantMatrixBuffer { get; }
        SamplerState SamplerState { get; }
        SamplerState VertexSamplerState { get; }

        public SphericalTerrainShader(Device device)
        {
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile(ShaderFileName, "SphericalTerrainVertexShader", "vs_4_0", ShaderFlags);
            var pixelShaderByteCode = ShaderBytecode.CompileFromFile(ShaderFileName, "SphericalTerrainPixelShader", "ps_4_0", ShaderFlags);

            VertexShader = new VertexShader(device, vertexShaderByteCode);
            PixelShader = new PixelShader(device, pixelShaderByteCode);

            Layout = VertexDefinition.PositionTexture.GetInputLayout(device, vertexShaderByteCode);

            vertexShaderByteCode.Dispose();
            pixelShaderByteCode.Dispose();

            ConstantMatrixBuffer = new Buffer(device, MatrixBufferDesription);

            // Create a texture sampler state description.
            var samplerDesc = new SamplerStateDescription
            {
                Filter = Filter.Anisotropic,
                AddressU = TextureAddressMode.Mirror,
                AddressV = TextureAddressMode.Mirror,
                AddressW = TextureAddressMode.Mirror,
                MipLodBias = 0,
                MaximumAnisotropy = 16,
                ComparisonFunction = Comparison.Always,
                BorderColor = new Color4(1, 1, 1, 1),
                MinimumLod = 0,
                MaximumLod = 0
            };

            // Create the texture sampler state.
            SamplerState = new SamplerState(device, samplerDesc);

            // Create a texture sampler state description.
            var vertexSamplerDesc = new SamplerStateDescription
            {
                Filter = Filter.MinMagMipPoint,
                AddressU = TextureAddressMode.Border,
                AddressV = TextureAddressMode.Border,
                AddressW = TextureAddressMode.Border,
                BorderColor = Color.White,
                ComparisonFunction = Comparison.Never,
                MaximumAnisotropy = 0,
                MipLodBias = 0,
                MinimumLod = 0,
                MaximumLod = 0
            };

            // Create the texture sampler state.
            VertexSamplerState = new SamplerState(device, vertexSamplerDesc);
        }

        public void Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, ShaderResourceView terrain, ShaderResourceView water)
        {
            UpdateMatrixBuffer(deviceContext, ConstantMatrixBuffer, worldMatrix, viewMatrix, projectionMatrix);
            deviceContext.VertexShader.SetConstantBuffer(0, ConstantMatrixBuffer);
            deviceContext.VertexShader.SetShaderResource(0, terrain);
            deviceContext.VertexShader.SetShaderResource(1, water);
            deviceContext.VertexShader.SetSampler(1,VertexSamplerState);
            deviceContext.PixelShader.SetShaderResource(0, terrain);
            deviceContext.PixelShader.SetShaderResource(1, water);
            deviceContext.InputAssembler.InputLayout = Layout;
            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.PixelShader.Set(PixelShader);
            deviceContext.PixelShader.SetSampler(0, SamplerState);
            deviceContext.DrawIndexed(indexCount, 0, 0);
        }

        public override void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(SamplerState, ConstantMatrixBuffer, Layout, PixelShader, VertexShader, VertexSamplerState);
        }
    }
}
