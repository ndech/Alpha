using System.Runtime.InteropServices;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace Alpha.DirectX.Shaders
{
    class LightShader : Shader
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct LightBuffer
        {
            public Vector4 ambiantColor;
            public Vector4 diffuseColor;
            public Vector3 direction;
            public float specularPower;
            public Vector4 specularColor;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CameraBuffer
        {
            public Vector3 cameraPosition;
            public float padding; // Structure size must be a multiple of 16 bytes
        }
        private const string ShaderFileName = @"Data/Shaders/Light.hlsl";
        VertexShader VertexShader { get; set; }
        PixelShader PixelShader { get; set; }
        InputLayout Layout { get; set; }
        Buffer ConstantMatrixBuffer { get; set; }
        Buffer ConstantLightBuffer { get; set; }
        Buffer ConstantCameraBuffer { get; set; }
        SamplerState SamplerState { get; set; }

        public LightShader(Device device)
        {
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile(ShaderFileName, "LightVertexShader", "vs_4_0", ShaderFlags);
            var pixelShaderByteCode = ShaderBytecode.CompileFromFile(ShaderFileName, "LightPixelShader", "ps_4_0", ShaderFlags);

            VertexShader = new VertexShader(device, vertexShaderByteCode);
            PixelShader = new PixelShader(device, pixelShaderByteCode);

            Layout = VertexDefinition.PositionTextureNormal.GetInputLayout(device, vertexShaderByteCode);

            vertexShaderByteCode.Dispose();
            pixelShaderByteCode.Dispose();

            ConstantMatrixBuffer = new Buffer(device, MatrixBufferDesription);

            // Setup the description of the dynamic matrix constant buffer that is in the vertex shader.
            var lightBufferDesc = new BufferDescription
            {
                Usage = ResourceUsage.Dynamic, // Updated each frame
                SizeInBytes = Utilities.SizeOf<LightBuffer>(), // Contains three matrices
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            };
            ConstantLightBuffer = new Buffer(device, lightBufferDesc);

            // Setup the description of the dynamic matrix constant buffer that is in the vertex shader.
            var cameraBufferDesc = new BufferDescription
            {
                Usage = ResourceUsage.Dynamic, // Updated each frame
                SizeInBytes = Utilities.SizeOf<CameraBuffer>(), // Contains three matrices
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            };
            ConstantCameraBuffer = new Buffer(device, cameraBufferDesc);

            // Create a texture sampler state description.
            var samplerDesc = new SamplerStateDescription
            {
                Filter = Filter.ComparisonAnisotropic,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                MipLodBias = 0,
                MaximumAnisotropy = 16,
                ComparisonFunction = Comparison.Always,
                BorderColor = new Color4(0, 0, 0, 0),
                MinimumLod = 0,
                MaximumLod = 0
            };

            // Create the texture sampler state.
            SamplerState = new SamplerState(device, samplerDesc);
        }

        public void Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, Texture texture, Light light, ICamera camera)
        {
            DataStream mappedResource;
            UpdateMatrixBuffer(deviceContext, ConstantMatrixBuffer, worldMatrix, viewMatrix, projectionMatrix);

            deviceContext.MapSubresource(ConstantLightBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);

            mappedResource.Write(
                new LightBuffer
                {
                    ambiantColor = light.AmbiantColor, 
                    diffuseColor = light.Color, 
                    direction = light.Direction,
                    specularPower = light.SpecularPower,
                    specularColor = light.SpecularColor
                });

            deviceContext.UnmapSubresource(ConstantLightBuffer, 0);


            deviceContext.MapSubresource(ConstantCameraBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);

            mappedResource.Write(
                new CameraBuffer
                {
                    cameraPosition = camera.Position
                });

            deviceContext.UnmapSubresource(ConstantCameraBuffer, 0);

            
            deviceContext.VertexShader.SetConstantBuffer(0, ConstantMatrixBuffer);
            deviceContext.VertexShader.SetConstantBuffer(1, ConstantCameraBuffer);
            deviceContext.PixelShader.SetConstantBuffer(0, ConstantLightBuffer);
            deviceContext.PixelShader.SetShaderResource(0, texture.TextureResource);
            deviceContext.InputAssembler.InputLayout = Layout;
            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.PixelShader.Set(PixelShader);
            deviceContext.PixelShader.SetSampler(0, SamplerState);
            deviceContext.DrawIndexed(indexCount, 0, 0);
        }
        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(SamplerState, ConstantMatrixBuffer, ConstantLightBuffer, ConstantCameraBuffer, Layout, PixelShader, VertexShader);
        }
    }
}
