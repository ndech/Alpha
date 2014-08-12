using System;
using Alpha.DirectX.Shaders;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Alpha.DirectX.UI
{
    class PlainRectangle : Rectangle
    {
        private Vector4 _color;
        public Vector4 Color
        {
            get { return _color; }
            set
            {
                if (value == _color) return;
                _color = value;
                Update();
            }
        }
        private readonly ColorShader _shader;
        private readonly VertexDefinition.PositionColor[] _vertices;

        public PlainRectangle(IContext context, Vector2I size, Color color)
        {
            _shader = context.Shaders.ColorShader;
            DeviceContext = context.DirectX.Device.ImmediateContext;
            _color = color.ToVector4();
            const int vertexCount = 4;
            _vertices = new VertexDefinition.PositionColor[vertexCount];
            VertexBuffer = Buffer.Create(context.DirectX.Device, _vertices,
                new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<VertexDefinition.PositionColor>() * vertexCount,
                    BindFlags = BindFlags.VertexBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                });

            IndexCount = 6;
            UInt32[] indices = { 0, 1, 2, 0, 3, 1 };
            IndexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.IndexBuffer, indices);
            Size = size;
        }

        public override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            int stride = Utilities.SizeOf<VertexDefinition.PositionColor>(); //Gets or sets the stride between vertex elements in the buffer (in bytes). 
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, stride, 0));
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _shader.Render(deviceContext, IndexCount, worldMatrix, viewMatrix, projectionMatrix);
        }

        public override void Update()
        {
            const float left = 0;
            const float top = 0;
            float right = Size.X;
            float bottom = Size.Y;

            _vertices[0] = new VertexDefinition.PositionColor {position = new Vector3(left, top, 0), color = _color};
            _vertices[1] = new VertexDefinition.PositionColor {position = new Vector3(right, bottom, 0), color = _color};
            _vertices[2] = new VertexDefinition.PositionColor {position = new Vector3(left, bottom, 0), color = _color};
            _vertices[3] = new VertexDefinition.PositionColor {position = new Vector3(right, top, 0), color = _color};

            DataStream mappedResource;
            DeviceContext.MapSubresource(VertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None,
                out mappedResource);
            mappedResource.WriteRange(_vertices);
            DeviceContext.UnmapSubresource(VertexBuffer, 0);
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(VertexBuffer, IndexBuffer);
        }
    }
}
