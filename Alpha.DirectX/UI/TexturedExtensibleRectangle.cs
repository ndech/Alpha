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
    class TexturedExtensibleRectangle
    {
        private readonly Buffer _vertexBuffer;
        private readonly Buffer _indexBuffer;
        private readonly int _indexCount;
        
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

        private TextureShader _shader;
        private Texture _texture;
        private VertexDefinition.PositionTexture[] _vertices;
        private DeviceContext _deviceContext;
        private int _fixedBorderRadius;
        public float Depth { get; set; }

        public TexturedExtensibleRectangle(IContext context, Vector2I size, Texture texture, int fixedBorderRadius, float depth = 0.0f)
        {
            _shader = context.Shaders.Get<TextureShader>();
            _deviceContext = context.DirectX.DeviceContext;
            _texture = texture;
            _fixedBorderRadius = fixedBorderRadius;
            Depth = depth;

            const int vertexCount = 16;
            _vertices = new VertexDefinition.PositionTexture[vertexCount];
            _vertexBuffer = Buffer.Create(context.DirectX.Device, _vertices,
                new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<VertexDefinition.PositionTexture>() * vertexCount,
                    BindFlags = BindFlags.VertexBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                });

            _indexCount = 54;
            UInt32[] indices = new UInt32[_indexCount];
            for(UInt32 i=0; i< 3; i++)
                for (UInt32 j = 0; j < 3; j++)
                {
                    indices[(i * 3 + j) * 6] = (i + 1) * 4 + j + 1;
                    indices[(i * 3 + j) * 6 + 1] = i * 4 + j + 1;
                    indices[(i * 3 + j) * 6 + 2] = i * 4 + j;
                    indices[(i * 3 + j) * 6 + 3] = (i + 1) * 4 + j;
                    indices[(i * 3 + j) * 6 + 4] = (i + 1) * 4 + j + 1;
                    indices[(i * 3 + j) * 6 + 5] = i * 4 + j;
                }
            _indexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.IndexBuffer, indices);
            Size = size;
        }

        public void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            int stride = Utilities.SizeOf<VertexDefinition.PositionTexture>(); //Gets or sets the stride between vertex elements in the buffer (in bytes). 
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, stride, 0));
            deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            _shader.Render(deviceContext, _indexCount, worldMatrix, viewMatrix, projectionMatrix, _texture.TextureResource);
        }

        private void Update()
        {
            float[] xVertexCoordinates = { 0f, _fixedBorderRadius, Size.X - _fixedBorderRadius, Size.X };
            //float[] yVertexCoordinates = { -Size.Y, _fixedBorderRadius - Size.Y, - _fixedBorderRadius, 0f };
            float[] yVertexCoordinates = { 0f, _fixedBorderRadius, Size.Y - _fixedBorderRadius, Size.Y };
            float[] xTextureCoordinates = { 0f, (float)_fixedBorderRadius / _texture.Width, 1f - (float)_fixedBorderRadius / _texture.Width, 1f };
            float[] yTextureCoordinates = { 0f, (float)_fixedBorderRadius / _texture.Height, 1f - (float)_fixedBorderRadius / _texture.Height, 1f };
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                    _vertices[x*4+y]  = new VertexDefinition.PositionTexture
                    {
                        position = new Vector3(xVertexCoordinates[x], yVertexCoordinates[y], Depth), 
                        texture = new Vector2(xTextureCoordinates[x], yTextureCoordinates[y])
                    };

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
