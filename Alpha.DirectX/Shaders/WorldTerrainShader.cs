using Alpha.Toolkit;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.Shaders
{
    class WorldTerrainShader : Shader
    {
        private const string ShaderFileName = @"Data/Shaders/WorldTerrain.hlsl";
        VertexShader VertexShader { get; }
        PixelShader PixelShader { get; }
        InputLayout Layout { get; }
        Buffer ConstantMatrixBuffer { get; }
        SamplerState SamplerStateMap { get; }

        public WorldTerrainShader(Device device)
        {
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile(ShaderFileName, "TerrainVertexShader", "vs_4_0", ShaderFlags);
            var pixelShaderByteCode = ShaderBytecode.CompileFromFile(ShaderFileName, "TerrainPixelShader", "ps_4_0", ShaderFlags);

            VertexShader = new VertexShader(device, vertexShaderByteCode);
            PixelShader = new PixelShader(device, pixelShaderByteCode);

            Layout = VertexDefinition.PositionTexture.GetInputLayout(device, vertexShaderByteCode);

            vertexShaderByteCode.Dispose();
            pixelShaderByteCode.Dispose();

            ConstantMatrixBuffer = new Buffer(device, MatrixBufferDesription);
            
            var samplerDescMap = new SamplerStateDescription
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
            SamplerStateMap = new SamplerState(device, samplerDescMap);
        }

        public void Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, ShaderResourceView heightMap)
        {
            UpdateMatrixBuffer(deviceContext, ConstantMatrixBuffer, worldMatrix, viewMatrix, projectionMatrix);

            deviceContext.InputAssembler.InputLayout = Layout;

            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.VertexShader.SetConstantBuffer(0, ConstantMatrixBuffer);
            deviceContext.PixelShader.Set(PixelShader);
            deviceContext.VertexShader.SetSampler(0, SamplerStateMap);
            deviceContext.VertexShader.SetShaderResource(0, heightMap);

            deviceContext.DrawIndexed(indexCount, 0, 0);
        }
        public override void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(VertexShader, PixelShader, SamplerStateMap, Layout, ConstantMatrixBuffer);
        }
    }
}
