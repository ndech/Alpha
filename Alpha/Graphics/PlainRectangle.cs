using System;
using Alpha.Graphics.Shaders;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Alpha.Graphics
{
    class PlainRectangle
    {
        private readonly Buffer _vertexBuffer;
        private readonly Buffer _indexBuffer;
        private readonly int _indexCount;

        public Vector2I Position { get; set; }

        private Vector2I _size;
        public Vector2I Size
        {
            get { return _size; }
            set
            {
                if (value != _size)
                {
                    _size = value;
                    Update();
                }
            }
        }
        private Vector2I _screenSize;
        private Vector4 _color;
        public Vector4 Color
        {
            get { return _color; }
            set
            {
                if (value != _color)
                {
                    _color = value;
                    Update();
                }
            }
        }
        private ColorShader _shader;
        private VertexDefinition.PositionColor[] _vertices;
        private DeviceContext _deviceContext;
        public float Depth { get; set; }

        public PlainRectangle(IRenderer renderer, Vector2I position, Vector2I size, Vector4 color, float depth = 0.0f)
        {
            _shader = renderer.ColorShader;
            _deviceContext = renderer.Device.ImmediateContext;
            Position = position;
            _screenSize = renderer.ScreenSize;
            _color = color;
            Depth = depth;

            const int vertexCount = 4;
            _vertices = new VertexDefinition.PositionColor[vertexCount];
            _vertexBuffer = Buffer.Create(renderer.Device, _vertices,
                new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<VertexDefinition.PositionColor>() * vertexCount,
                    BindFlags = BindFlags.VertexBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                });

            _indexCount = 6;
            UInt32[] indices = { 0, 1, 2, 0, 3, 1 };
            _indexBuffer = Buffer.Create(renderer.Device, BindFlags.IndexBuffer, indices);
            Size = size;
        }

        public void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            int stride = Utilities.SizeOf<VertexDefinition.PositionColor>(); //Gets or sets the stride between vertex elements in the buffer (in bytes). 
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, stride, 0));
            deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            int drawX = -(_screenSize.X >> 1) + Position.X;
            int drawY = (_screenSize.Y >> 1) - Position.Y;
            Matrix position = Matrix.Translation(drawX, drawY, 0);
            _shader.Render(deviceContext, _indexCount, worldMatrix*position, viewMatrix, projectionMatrix);
        }

        private void Update()
        {
            const float left = 0;
            float right = left + Size.X;
            const float top = 0;
            float bottom = top - Size.Y;

            _vertices[0] = new VertexDefinition.PositionColor {position = new Vector3(left, top, Depth), color = _color};
            _vertices[1] = new VertexDefinition.PositionColor {position = new Vector3(right, bottom, Depth), color = _color};
            _vertices[2] = new VertexDefinition.PositionColor {position = new Vector3(left, bottom, Depth), color = _color};
            _vertices[3] = new VertexDefinition.PositionColor {position = new Vector3(right, top, Depth), color = _color};

            DataStream mappedResource;
            _deviceContext.MapSubresource(_vertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None,
                out mappedResource);
            mappedResource.WriteRange(_vertices);
            _deviceContext.UnmapSubresource(_vertexBuffer, 0);
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(_vertexBuffer, _indexBuffer);
        }
    }
}
