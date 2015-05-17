using System;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Alpha.DirectX
{
    class ConstantBuffer<T> : IDisposable where T : struct
    {
        public Buffer Buffer { get; }
        private readonly DeviceContext _deviceContext;

        public ConstantBuffer(IContext context)
        {
            _deviceContext = context.DirectX.DeviceContext;

            Buffer = new Buffer(context.DirectX.Device,
                new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<T>(),
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                });
        }

        public ConstantBuffer(IContext context, T item) : this(context)
        {
            Update(item);
        }

        internal void Update(T value)
        {
            DataStream mappedResource;
            _deviceContext.MapSubresource(Buffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);
            mappedResource.Write(value);
            _deviceContext.UnmapSubresource(Buffer, 0);
        }

        public void Dispose()
        {
            Buffer.Dispose();
        }
    }
}
