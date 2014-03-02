using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    class Rectangle
    {
        private bool _changed;
        private readonly Buffer _vertexBuffer;
        private readonly Buffer _indexBuffer;
        private readonly int _indexCount;
        public Vector2I Size
        {
            get { return _size; }
            set
            {
                if (value != _size)
                    _changed = true;
                _size = value;
            }
        }

        private Vector2I _size;
        public Vector2I Position
        {
            get { return _position; }
            set
            {
                if (value != _position)
                    _changed = true;
                _position = value;
            }
        }
        private Vector2I _position;

        public Vector2I ScreenSize { get; set; }

        private Vector4 _color;
        private ColorShader _shader;
        private VertexDefinition.PositionColor[] _vertices;
        public float Depth { get; set; }

        public Rectangle(Renderer renderer, Vector2I screenSize, Vector2I position, Vector2I size, Vector4 color, float depth = 0.0f)
        {
            _shader = renderer.ColorShader;
            Position = position;
            ScreenSize = screenSize;
            Size = size;
            _color = color;
            _changed = true;
            Depth = depth;

            int vertexCount = 4;
            _indexCount = 6;

            _vertices = new VertexDefinition.PositionColor[vertexCount];
            UInt32[] indices = { 0, 1, 2, 0, 3, 1 };

            _vertexBuffer = Buffer.Create(renderer.DirectX.Device, _vertices,
                new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<VertexDefinition.PositionColor>() * vertexCount,
                    BindFlags = BindFlags.VertexBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                });

            _indexBuffer = Buffer.Create(renderer.DirectX.Device, BindFlags.IndexBuffer, indices);
        }
        public void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            if (_changed)
            {
                float left = (float)((ScreenSize.X / 2) * -1) + (float)Position.X;
                float right = left + (float)Size.X;
                float top = (float)(ScreenSize.Y / 2) - (float)Position.Y;
                float bottom = top - (float)Size.Y;

                _vertices[0] = new VertexDefinition.PositionColor { position = new Vector3(left, top, Depth), color = _color};
                _vertices[1] = new VertexDefinition.PositionColor { position = new Vector3(right, bottom, Depth), color = _color };
                _vertices[2] = new VertexDefinition.PositionColor { position = new Vector3(left, bottom, Depth), color = _color };
                _vertices[3] = new VertexDefinition.PositionColor { position = new Vector3(right, top, Depth), color = _color };

                DataStream mappedResource;
                deviceContext.MapSubresource(_vertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);
                mappedResource.WriteRange(_vertices);
                deviceContext.UnmapSubresource(_vertexBuffer, 0);
                _changed = false;
            }

            // Set vertex buffer stride and offset.
            int stride = Utilities.SizeOf<VertexDefinition.PositionColor>(); //Gets or sets the stride between vertex elements in the buffer (in bytes). 
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, stride, 0));
            deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            _shader.Render(deviceContext, _indexCount, worldMatrix, viewMatrix, projectionMatrix);
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(_vertexBuffer, _indexBuffer);
        }
    }
}
