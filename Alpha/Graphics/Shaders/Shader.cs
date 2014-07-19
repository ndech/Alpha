using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.Graphics.Shaders
{
    public abstract class Shader
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct MatrixBuffer
        {
            public Matrix world;
            public Matrix view;
            public Matrix projection;
        }

        public BufferDescription MatrixBufferDesription
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
    }
}
