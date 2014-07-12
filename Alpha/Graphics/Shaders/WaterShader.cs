using System;
using System.Runtime.InteropServices;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace Alpha.Graphics.Shaders
{
    class WaterShader : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct MatrixBuffer
        {
            public Matrix world;
            public Matrix view;
            public Matrix projection;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct TranslationBuffer
        {
            public Vector2 translation;
            public Vector2 padding;
        }

        private const string ShaderFileName = @"Data/Shaders/Water.hlsl";
        VertexShader VertexShader { get; set; }
        PixelShader PixelShader { get; set; }
        InputLayout Layout { get; set; }
        Buffer ConstantMatrixBuffer { get; set; }
        Buffer ConstantTranslationBuffer { get; set; }
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
                SizeInBytes = Utilities.SizeOf<TranslationBuffer>(),
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            };
            ConstantTranslationBuffer = new Buffer(device, translateBufferDesc);
            
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
            ShaderResourceView bumpMap, ShaderResourceView borderTexture, Vector2 translation)
        {
            DataStream mappedResource;

            // Copy the transposed matrices (because they are stored in column-major order on the GPU by default) into the constant buffer.
            deviceContext.MapSubresource(ConstantMatrixBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
            mappedResource.Write(
                new MatrixBuffer
                {
                    world = Matrix.Transpose(worldMatrix),
                    view = Matrix.Transpose(viewMatrix),
                    projection = Matrix.Transpose(projectionMatrix)
                });
            deviceContext.UnmapSubresource(ConstantMatrixBuffer, 0);

            //Copy translation buffer to GPU
            deviceContext.MapSubresource(ConstantTranslationBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
            mappedResource.Write(new TranslationBuffer { translation = translation });
            deviceContext.UnmapSubresource(ConstantTranslationBuffer, 0);

            deviceContext.InputAssembler.InputLayout = Layout;

            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.VertexShader.SetConstantBuffer(0, ConstantMatrixBuffer);
            deviceContext.PixelShader.Set(PixelShader);
            deviceContext.PixelShader.SetConstantBuffer(1, ConstantTranslationBuffer);
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
