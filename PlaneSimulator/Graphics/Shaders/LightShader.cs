using System;
using System.Runtime.InteropServices;
using PlaneSimulator.Toolkit;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace PlaneSimulator.Graphics.Shaders
{
    class LightShader : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct Vertex
        {
            public static int AppendAlignedElement1 = 12;
            public static int AppendAlignedElement2 = 20;
            public Vector3 position;
            public Vector2 texture;
            public Vector3 normal;
        }

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
            public Vector4 diffuseColor;
            public Vector3 direction;
            public float padding; // Structure size must be a multiple of 16 bytes
        }

        private const string VertexShaderFileName = @"Data/Shaders/Light.vs";
        private const string PixelShaderFileName = @"Data/Shaders/Light.ps";
        VertexShader VertexShader { get; set; }
        PixelShader PixelShader { get; set; }
        InputLayout Layout { get; set; }
        Buffer ConstantMatrixBuffer { get; set; }
        Buffer ConstantLightBuffer { get; set; }
        SamplerState SamplerState { get; set; }

        public LightShader(Device device)
        {
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile(VertexShaderFileName, "LightVertexShader", "vs_4_0", ShaderFlags.None, EffectFlags.None);
            var pixelShaderByteCode = ShaderBytecode.CompileFromFile(PixelShaderFileName, "LightPixelShader", "ps_4_0", ShaderFlags.None, EffectFlags.None);

            VertexShader = new VertexShader(device, vertexShaderByteCode);
            PixelShader = new PixelShader(device, pixelShaderByteCode);

            var inputElements = new InputElement[]
			{
				new InputElement
				{
					SemanticName = "POSITION",
					SemanticIndex = 0,
					Format = Format.R32G32B32_Float,
					Slot = 0,
					AlignedByteOffset = 0,
					Classification = InputClassification.PerVertexData,
					InstanceDataStepRate = 0
				},
				new InputElement
				{
					SemanticName = "TEXCOORD",
					SemanticIndex = 0,
					Format = Format.R32G32_Float,
					Slot = 0,
					AlignedByteOffset = LightShader.Vertex.AppendAlignedElement1,
					Classification = InputClassification.PerVertexData,
					InstanceDataStepRate = 0
				},
				new InputElement
				{
					SemanticName = "NORMAL",
					SemanticIndex = 0,
					Format = Format.R32G32B32_Float,
					Slot = 0,
					AlignedByteOffset = LightShader.Vertex.AppendAlignedElement2,
					Classification = InputClassification.PerVertexData,
					InstanceDataStepRate = 0
				}
			};
            Layout = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);

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
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                MipLodBias = 0,
                MaximumAnisotropy = 1,
                ComparisonFunction = Comparison.Always,
                BorderColor = new Color4(0, 0, 0, 0),
                MinimumLod = 0,
                MaximumLod = 0
            };

            // Create the texture sampler state.
            SamplerState = new SamplerState(device, samplerDesc);
        }

        public void Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, Texture texture, Vector4 lightColor, Vector3 lightDirection)
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

            // Finally set the constant buffer in the vertex shader with the updated values.
            deviceContext.VertexShader.SetConstantBuffer(0, ConstantMatrixBuffer);

            // Unlock the constant buffer.
            deviceContext.UnmapSubresource(ConstantMatrixBuffer, 0);

            deviceContext.MapSubresource(ConstantLightBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);

            mappedResource.Write(new LightBuffer{ diffuseColor = lightColor, direction = lightDirection});

            deviceContext.UnmapSubresource(ConstantLightBuffer, 0);

            deviceContext.PixelShader.SetConstantBuffer(0, ConstantLightBuffer);

            // Set shader resource in the pixel shader.
            deviceContext.PixelShader.SetShaderResource(0, texture.TextureResource);

            // Set the vertex input layout.
            deviceContext.InputAssembler.InputLayout = Layout;

            // Set the vertex and pixel shaders that will be used to render this triangle.
            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.PixelShader.Set(PixelShader);

            // Set the sampler state in the pixel shader.
            deviceContext.PixelShader.SetSampler(0, SamplerState);

            // Render the triangle.
            deviceContext.DrawIndexed(indexCount, 0, 0);
        }
        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(SamplerState, ConstantMatrixBuffer, ConstantLightBuffer, Layout, PixelShader, VertexShader);
        }
    }
}
