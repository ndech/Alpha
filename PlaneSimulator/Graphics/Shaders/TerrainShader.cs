using System;
using System.Runtime.InteropServices;
using PlaneSimulator.Toolkit;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace PlaneSimulator.Graphics.Shaders
{
    public class TerrainShader : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct MatrixBuffer
        {
            public Matrix world;
            public Matrix view;
            public Matrix projection;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LightBuffer
        {
            public Vector4 ambiantColor;
            public Vector4 diffuseColor;
            public Vector3 direction;
            public float padding;
        }

        private const string VertexShaderFileName = @"Data/Shaders/Terrain.vs";
        private const string PixelShaderFileName = @"Data/Shaders/Terrain.ps";
        VertexShader VertexShader { get; set; }
        PixelShader PixelShader { get; set; }
        InputLayout Layout { get; set; }
        Buffer ConstantMatrixBuffer { get; set; }
        Buffer ConstantLightBuffer { get; set; }
        SamplerState SamplerState { get; set; }

        public TerrainShader(Device device)
        {
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile(VertexShaderFileName, "TerrainVertexShader", "vs_4_0", ShaderFlags.None, EffectFlags.None);
            var pixelShaderByteCode = ShaderBytecode.CompileFromFile(PixelShaderFileName, "TerrainPixelShader", "ps_4_0", ShaderFlags.None, EffectFlags.None);

            VertexShader = new VertexShader(device, vertexShaderByteCode);
            PixelShader = new PixelShader(device, pixelShaderByteCode);

            Layout = VertexDefinition.PositionTextureNormal.GetInputLayout(device, vertexShaderByteCode);

            vertexShaderByteCode.Dispose();
            pixelShaderByteCode.Dispose();

            // Setup the description of the dynamic matrix constant buffer that is in the vertex shader.
            var matrixBufferDesc = new BufferDescription
            {
                Usage = ResourceUsage.Dynamic, // Updated each frame
                SizeInBytes = Utilities.SizeOf<MatrixBuffer>(), // Contains three matrices
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            };
            ConstantMatrixBuffer = new Buffer(device, matrixBufferDesc);

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

            // Create a texture sampler state description.
            var samplerDesc = new SamplerStateDescription
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
            SamplerState = new SamplerState(device, samplerDesc);
        }

        public void Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, Light light, Texture texture)
        {
            worldMatrix.Transpose();
            viewMatrix.Transpose();
            projectionMatrix.Transpose();
            // Lock the constant memory buffer so it can be written to.
            DataStream mappedResource;
            deviceContext.MapSubresource(ConstantMatrixBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);

            // Copy the transposed matrices (because they are stored in column-major order on the GPU by default) into the constant buffer.
            var matrixBuffer = new MatrixBuffer
            {
                world = worldMatrix,
                view = viewMatrix,
                projection = projectionMatrix
            };
            mappedResource.Write(matrixBuffer);

            // Unlock the constant buffer.
            deviceContext.UnmapSubresource(ConstantMatrixBuffer, 0);

            deviceContext.MapSubresource(ConstantLightBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);

            mappedResource.Write(
                new LightBuffer
                {
                    ambiantColor = light.AmbiantColor,
                    diffuseColor = light.Color,
                    direction = light.Direction
                });

            deviceContext.UnmapSubresource(ConstantLightBuffer, 0);

            
            // Finally set the constant buffers in the vertex shader with the updated values.
            deviceContext.VertexShader.SetConstantBuffer(0, ConstantMatrixBuffer);

            deviceContext.PixelShader.SetConstantBuffer(0, ConstantLightBuffer);
            deviceContext.PixelShader.SetShaderResource(0, texture.TextureResource);
            
            // Set the vertex input layout.
            deviceContext.InputAssembler.InputLayout = Layout;

            // Set the vertex and pixel shaders that will be used to render this triangle.
            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.PixelShader.Set(PixelShader);
            deviceContext.PixelShader.SetSampler(0, SamplerState);
            
            // Render the triangles.
            deviceContext.DrawIndexed(indexCount, 0, 0);
        }
        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(ConstantMatrixBuffer, ConstantLightBuffer, Layout, PixelShader, VertexShader);
        }
    }
}
