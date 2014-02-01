using System;
using System.Runtime.InteropServices;
using PlaneSimulator.Toolkit;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace PlaneSimulator.Graphics.Shaders
{
    public class WaterShader : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct MatrixBuffer
        {
            public Matrix world;
            public Matrix view;
            public Matrix projection;
            public Matrix reflection;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct TranslationBuffer
        {
            public Vector2 translation;
            public Vector2 padding;
        }

        private const string VertexShaderFileName = @"Data/Shaders/Water.vs";
        private const string PixelShaderFileName = @"Data/Shaders/Water.ps";
        VertexShader VertexShader { get; set; }
        PixelShader PixelShader { get; set; }
        InputLayout Layout { get; set; }
        Buffer ConstantMatrixBuffer { get; set; }
        Buffer ConstantTranslationBuffer { get; set; }
        SamplerState SamplerState { get; set; }

        public WaterShader(Device device)
        {
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile(VertexShaderFileName, "WaterVertexShader", "vs_4_0", ShaderFlags.Debug, EffectFlags.None);
            var pixelShaderByteCode = ShaderBytecode.CompileFromFile(PixelShaderFileName, "WaterPixelShader", "ps_4_0", ShaderFlags.Debug, EffectFlags.None);

            VertexShader = new VertexShader(device, vertexShaderByteCode);
            PixelShader = new PixelShader(device, pixelShaderByteCode);

            Layout = VertexDefinition.PositionTexture.GetInputLayout(device, vertexShaderByteCode);

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
            var translateBufferDesc = new BufferDescription
            {
                Usage = ResourceUsage.Dynamic, // Updated each frame
                SizeInBytes = Utilities.SizeOf<TranslationBuffer>(), // Contains three matrices
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            };
            ConstantTranslationBuffer = new Buffer(device, translateBufferDesc);

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

        public void Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, Matrix reflexionMatrix,
            ShaderResourceView reflectionMap, ShaderResourceView refractionMap, ShaderResourceView bumpMap, Vector2 translation)
        {
            worldMatrix.Transpose();
            viewMatrix.Transpose();
            projectionMatrix.Transpose();
            reflexionMatrix.Transpose();
            // Lock the constant memory buffer so it can be written to.
            DataStream mappedResource;
            deviceContext.MapSubresource(ConstantMatrixBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);

            // Copy the transposed matrices (because they are stored in column-major order on the GPU by default) into the constant buffer.
            var matrixBuffer = new MatrixBuffer()
            {
                world = worldMatrix,
                view = viewMatrix,
                projection = projectionMatrix,
                reflection = reflexionMatrix
            };
            mappedResource.Write(matrixBuffer);

            // Unlock the constant buffer.
            deviceContext.UnmapSubresource(ConstantMatrixBuffer, 0);


            deviceContext.MapSubresource(ConstantTranslationBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);

            // Copy the transposed matrices (because they are stored in column-major order on the GPU by default) into the constant buffer.
            var translationBuffer = new TranslationBuffer()
            {
                translation = translation
            };
            mappedResource.Write(translationBuffer);

            // Unlock the constant buffer.
            deviceContext.UnmapSubresource(ConstantTranslationBuffer, 0);

            // Set the position of the constant buffer in the vertex shader.
            var bufferNumber = 0;

            // Finally set the constant buffer in the vertex shader with the updated values.
            deviceContext.VertexShader.SetConstantBuffer(bufferNumber, ConstantMatrixBuffer);

            // Set the vertex input layout.
            deviceContext.InputAssembler.InputLayout = Layout;

            // Set the vertex and pixel shaders that will be used to render this triangle.
            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.PixelShader.SetConstantBuffer(0, ConstantTranslationBuffer);
            deviceContext.PixelShader.Set(PixelShader);
            deviceContext.PixelShader.SetSampler(0, SamplerState);
            deviceContext.PixelShader.SetShaderResource(0, reflectionMap);
            deviceContext.PixelShader.SetShaderResource(1, refractionMap);
            deviceContext.PixelShader.SetShaderResource(2, bumpMap);

            // Render the triangle.
            deviceContext.DrawIndexed(indexCount, 0, 0);
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(VertexShader, PixelShader, Layout, ConstantMatrixBuffer);
        }
    }
}
