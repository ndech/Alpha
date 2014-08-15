using System.Runtime.InteropServices;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Alpha.DirectX.Shaders
{
    class WaterShader : Shader
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct TranslationBuffer
        {
            public Vector2 translation;
            public Vector2 padding;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct LightBuffer
        {
            public Vector4 ambiantColor;
            public Vector4 diffuseColor;
            public Vector3 direction;
            public float specularPower;
            public Vector4 specularColor;

            public LightBuffer(Light light)
            {
                ambiantColor = light.AmbiantColor;
                diffuseColor = light.Color;
                direction = light.Direction;
                specularColor = light.SpecularColor;
                specularPower = light.SpecularPower;
            }
        }

        private const string ShaderFileName = @"Data/Shaders/Water.hlsl";
        VertexShader VertexShader { get; set; }
        PixelShader PixelShader { get; set; }
        InputLayout Layout { get; set; }
        Buffer ConstantMatrixBuffer { get; set; }
        Buffer ConstantTranslationBuffer { get; set; }
        Buffer ConstantLightBuffer { get; set; }
        SamplerState SamplerStateWrap { get; set; }
        SamplerState SamplerStateBorder { get; set; }

        public WaterShader(Device device)
        {
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile(ShaderFileName, "WaterVertexShader", "vs_4_0", ShaderFlags.Debug);
            var pixelShaderByteCode = ShaderBytecode.CompileFromFile(ShaderFileName, "WaterPixelShader", "ps_4_0", ShaderFlags.Debug);

            VertexShader = new VertexShader(device, vertexShaderByteCode);
            PixelShader = new PixelShader(device, pixelShaderByteCode);

            Layout = VertexDefinition.WaterVertex.GetInputLayout(device, vertexShaderByteCode);

            vertexShaderByteCode.Dispose();
            pixelShaderByteCode.Dispose();

            ConstantMatrixBuffer = new Buffer(device, MatrixBufferDesription);

            // Setup the description of the dynamic matrix constant buffer that is in the vertex shader.
            var translateBufferDesc = new BufferDescription
            {
                Usage = ResourceUsage.Dynamic, // Updated each frame
                SizeInBytes = Utilities.SizeOf<TranslationBuffer>(),
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            };
            ConstantTranslationBuffer = new Buffer(device, translateBufferDesc);


            // Setup the description of the dynamic matrix constant buffer that is in the vertex shader.
            var lightBufferDesc = new BufferDescription
            {
                Usage = ResourceUsage.Dynamic, // Updated each frame
                SizeInBytes = Utilities.SizeOf<LightBuffer>(),
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            };
            ConstantLightBuffer = new Buffer(device, lightBufferDesc);
            
            // Create a texture sampler state description.
            var samplerDescWrap = new SamplerStateDescription
            {
                Filter = Filter.ComparisonMinLinearMagPointMipLinear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                MipLodBias = 0,
                MaximumAnisotropy = 1,
                ComparisonFunction = Comparison.Always,
                BorderColor = new Color4(0, 0, 0, 0),
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            };

            // Create the texture sampler state.
            SamplerStateWrap = new SamplerState(device, samplerDescWrap);

            var samplerDescBorder = new SamplerStateDescription
            {
                Filter = Filter.Anisotropic,
                AddressU = TextureAddressMode.MirrorOnce,
                AddressV = TextureAddressMode.Border,
                AddressW = TextureAddressMode.Border,
                MipLodBias = 0,
                MaximumAnisotropy = 16,
                ComparisonFunction = Comparison.Always,
                BorderColor = Color.Transparent,
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            };

            // Create the texture sampler state.
            SamplerStateBorder = new SamplerState(device, samplerDescBorder);
        }

        public void Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix,
            ShaderResourceView bumpMap, ShaderResourceView borderTexture, Vector2 translation, Light light)
        {
            UpdateMatrixBuffer(deviceContext, ConstantMatrixBuffer, worldMatrix, viewMatrix, projectionMatrix);

            //Copy translation buffer to GPU
            DataStream mappedResource;
            deviceContext.MapSubresource(ConstantTranslationBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
            mappedResource.Write(new TranslationBuffer { translation = translation });
            deviceContext.UnmapSubresource(ConstantTranslationBuffer, 0);

            deviceContext.MapSubresource(ConstantLightBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
            mappedResource.Write(new LightBuffer(light));
            deviceContext.UnmapSubresource(ConstantLightBuffer, 0);

            deviceContext.InputAssembler.InputLayout = Layout;

            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.VertexShader.SetConstantBuffer(0, ConstantMatrixBuffer);
            deviceContext.PixelShader.Set(PixelShader);
            deviceContext.PixelShader.SetConstantBuffer(1, ConstantTranslationBuffer);
            deviceContext.PixelShader.SetConstantBuffer(2, ConstantLightBuffer);
            deviceContext.PixelShader.SetSampler(0, SamplerStateWrap);
            deviceContext.PixelShader.SetSampler(1, SamplerStateBorder);
            deviceContext.PixelShader.SetShaderResource(0, bumpMap);
            deviceContext.PixelShader.SetShaderResource(1, borderTexture);

            deviceContext.DrawIndexed(indexCount, 0, 0);
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(VertexShader, PixelShader, Layout, ConstantMatrixBuffer);
        }
    }
}
