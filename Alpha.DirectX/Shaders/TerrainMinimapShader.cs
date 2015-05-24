using Alpha.Toolkit;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.Shaders
{
    class TerrainMinimapShader : Shader
    {
        private const string ShaderFileName = @"Data/Shaders/MinimapTerrain.hlsl";
        VertexShader VertexShader { get; }
        PixelShader PixelShader { get; }
        InputLayout Layout { get; }
        Buffer ConstantMatrixBuffer { get; }
        SamplerState SamplerStateBorder { get; }
        SamplerState SamplerStateColor { get; }

        public TerrainMinimapShader(Device device)
        {
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile(ShaderFileName, "MinimapTerrainVertexShader", "vs_4_0", ShaderFlags);
            var pixelShaderByteCode = ShaderBytecode.CompileFromFile(ShaderFileName, "MinimapTerrainPixelShader", "ps_4_0", ShaderFlags);

            VertexShader = new VertexShader(device, vertexShaderByteCode);
            PixelShader = new PixelShader(device, pixelShaderByteCode);

            Layout = VertexDefinition.TerrainVertex.GetInputLayout(device, vertexShaderByteCode);

            vertexShaderByteCode.Dispose();
            pixelShaderByteCode.Dispose();

            ConstantMatrixBuffer = new Buffer(device, MatrixBufferDesription);

            var samplerDescBorder = new SamplerStateDescription
            {
                Filter = Filter.Anisotropic,
                AddressU = TextureAddressMode.MirrorOnce,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                MipLodBias = 0,
                MaximumAnisotropy = 16,
                ComparisonFunction = Comparison.Always,
                BorderColor = Color.Transparent,
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            };

            // Create the texture sampler state.
            SamplerStateBorder = new SamplerState(device, samplerDescBorder);

            var samplerDescColor = new SamplerStateDescription
            {
                Filter = Filter.MinMagMipPoint,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                MipLodBias = 0,
                MaximumAnisotropy = 1,
                ComparisonFunction = Comparison.Always,
                BorderColor = Color.Transparent,
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            };

            // Create the texture sampler state.
            SamplerStateColor = new SamplerState(device, samplerDescColor);
        }

        public void Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, ShaderResourceView borderTexture, ShaderResourceView paperTexture)
        {
            UpdateMatrixBuffer(deviceContext, ConstantMatrixBuffer, worldMatrix, viewMatrix, projectionMatrix);

            deviceContext.InputAssembler.InputLayout = Layout;

            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.VertexShader.SetConstantBuffer(0, ConstantMatrixBuffer);
            deviceContext.PixelShader.Set(PixelShader);
            deviceContext.PixelShader.SetSampler(0, SamplerStateBorder);
            deviceContext.PixelShader.SetSampler(1, SamplerStateColor);
            deviceContext.PixelShader.SetShaderResource(0, borderTexture);
            deviceContext.PixelShader.SetShaderResource(1, paperTexture);

            deviceContext.DrawIndexed(indexCount, 0, 0);
        }
        public override void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(VertexShader, PixelShader, Layout, SamplerStateBorder, SamplerStateColor, ConstantMatrixBuffer);
        }
    }
}
