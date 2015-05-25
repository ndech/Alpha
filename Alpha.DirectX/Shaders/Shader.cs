using System;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Alpha.DirectX.Shaders
{
    public abstract class Shader : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct MatrixBuffer
        {
            public Matrix world;
            public Matrix view;
            public Matrix projection;
        }

        protected BufferDescription MatrixBufferDesription
        {
            get
            {
                return new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<MatrixBuffer>(),
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };
            }
        }

        public static ShaderFlags ShaderFlags
        {
            get
            {
#if GPU_DEBUG
                return ShaderFlags.Debug;
#elif DEBUG
                return ShaderFlags.None;
#else
                return ShaderFlags.OptimizationLevel3;
#endif
            }
        }

        public void UpdateMatrixBuffer(DeviceContext deviceContext, Buffer constantMatrixBuffer, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            worldMatrix.Transpose();
            viewMatrix.Transpose();
            projectionMatrix.Transpose();
            var matrixBuffer = new MatrixBuffer
            {
                world = worldMatrix,
                view = viewMatrix,
                projection = projectionMatrix
            };

            UpdateBuffer(deviceContext, constantMatrixBuffer, matrixBuffer);
        }

        public void UpdateBuffer<T>(DeviceContext deviceContext, Buffer buffer, T value) where T : struct
        {
            DataStream mappedResource;
            deviceContext.MapSubresource(buffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
            mappedResource.Write(value);
            deviceContext.UnmapSubresource(buffer, 0);
        }

        public SamplerStateDescription WrapSamplerStateDescription
        {
            get
            {
                return new SamplerStateDescription
                {
                    Filter = Filter.MinLinearMagMipPoint,
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
            }
        }

        public abstract void Dispose();
    }
}
