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
    class TexturedRectangle
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
        private TextureShader _shader;
        private VertexDefinition.PositionTexture[] _vertices;
        private DeviceContext _deviceContext;
        private Texture _texture ;
        public float Depth { get; set; }

        public TexturedRectangle(IRenderer renderer, Vector2I position, Vector2I size, Texture texture, float depth = 0.0f)
        {
            _shader = renderer.TextureShader;
            _texture = texture;
            _deviceContext = renderer.Device.ImmediateContext;
            Position = position;
            _screenSize = renderer.ScreenSize;
            Depth = depth;

            const int vertexCount = 4;
            _vertices = new VertexDefinition.PositionTexture[vertexCount];
            _vertexBuffer = Buffer.Create(renderer.Device, _vertices,
                new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<VertexDefinition.PositionTexture>() * vertexCount,
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
            int stride = Utilities.SizeOf<VertexDefinition.PositionTexture>(); //Gets or sets the stride between vertex elements in the buffer (in bytes). 
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, stride, 0));
            deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            int drawX = -(_screenSize.X >> 1) + Position.X;
            int drawY = (_screenSize.Y >> 1) - Position.Y;
            Matrix position = Matrix.Translation(drawX, drawY, 0);
            _shader.Render(deviceContext, _indexCount, worldMatrix * position, viewMatrix, projectionMatrix, _texture.TextureResource);
        }

        private void Update()
        {
            const float left = 0;
            float right = left + Size.X;
            const float top = 0;
            float bottom = top - Size.Y;

            _vertices[0] = new VertexDefinition.PositionTexture { position = new Vector3(left, top, Depth), texture = new Vector2(0,0)};
            _vertices[1] = new VertexDefinition.PositionTexture { position = new Vector3(right, bottom, Depth), texture = new Vector2(1, 1) };
            _vertices[2] = new VertexDefinition.PositionTexture { position = new Vector3(left, bottom, Depth), texture = new Vector2(0, 1) };
            _vertices[3] = new VertexDefinition.PositionTexture { position = new Vector3(right, top, Depth), texture = new Vector2(1, 0) };

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
