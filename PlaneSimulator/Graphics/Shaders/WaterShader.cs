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
        internal struct Vertex
        {
            public static int AppendAlignedElement = 12;
            public Vector3 position;
            public Vector4 color;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MatrixBuffer
        {
            public Matrix world;
            public Matrix view;
            public Matrix projection;
        }

        private const string VertexShaderFileName = @"Data/Shaders/Water.vs";
        private const string PixelShaderFileName = @"Data/Shaders/Water.ps";
        VertexShader VertexShader { get; set; }
        PixelShader PixelShader { get; set; }
        InputLayout Layout { get; set; }
        Buffer ConstantMatrixBuffer { get; set; }

        public WaterShader(Device device)
        {
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile(VertexShaderFileName, "WaterVertexShader", "vs_4_0", ShaderFlags.Debug, EffectFlags.None);
            var pixelShaderByteCode = ShaderBytecode.CompileFromFile(PixelShaderFileName, "WaterPixelShader", "ps_4_0", ShaderFlags.Debug, EffectFlags.None);

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
					SemanticName = "COLOR",
					SemanticIndex = 0,
					Format = Format.R32G32B32A32_Float,
					Slot = 0,
					AlignedByteOffset = ColorShader.Vertex.AppendAlignedElement,
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
        }

        public void Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            worldMatrix.Transpose();
            viewMatrix.Transpose();
            projectionMatrix.Transpose();
            // Lock the constant memory buffer so it can be written to.
            DataStream mappedResource;
            deviceContext.MapSubresource(ConstantMatrixBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);

            // Copy the transposed matrices (because they are stored in column-major order on the GPU by default) into the constant buffer.
            var matrixBuffer = new MatrixBuffer()
            {
                world = worldMatrix,
                view = viewMatrix,
                projection = projectionMatrix
            };
            mappedResource.Write(matrixBuffer);

            // Unlock the constant buffer.
            deviceContext.UnmapSubresource(ConstantMatrixBuffer, 0);

            // Set the position of the constant buffer in the vertex shader.
            var bufferNumber = 0;

            // Finally set the constant buffer in the vertex shader with the updated values.
            deviceContext.VertexShader.SetConstantBuffer(bufferNumber, ConstantMatrixBuffer);

            // Set the vertex input layout.
            deviceContext.InputAssembler.InputLayout = Layout;

            // Set the vertex and pixel shaders that will be used to render this triangle.
            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.PixelShader.Set(PixelShader);

            // Render the triangle.
            deviceContext.DrawIndexed(indexCount, 0, 0);
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(VertexShader, PixelShader, Layout, ConstantMatrixBuffer);
        }
    }
}
