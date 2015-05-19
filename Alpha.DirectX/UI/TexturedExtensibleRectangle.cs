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
    class TexturedExtensibleRectangle : Rectangle, IDisposable
    {
        private TextureShader _shader;
        private Texture _texture;
        private VertexDefinition.PositionTexture[] _vertices;
        private int _fixedBorderRadius;

        public TexturedExtensibleRectangle(IContext context, Vector2I size, Texture texture, int fixedBorderRadius)
        {
            DeviceContext = context.DirectX.DeviceContext;
            _shader = context.Shaders.Get<TextureShader>();
            _texture = texture;
            _fixedBorderRadius = fixedBorderRadius;

            const int vertexCount = 16;
            _vertices = new VertexDefinition.PositionTexture[vertexCount];
            VertexBuffer = Buffer.Create(context.DirectX.Device, _vertices,
                new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<VertexDefinition.PositionTexture>() * vertexCount,
                    BindFlags = BindFlags.VertexBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                });

            IndexCount = 54;
            uint[] indices = new uint[IndexCount];
            for(uint i=0; i< 3; i++)
                for (uint j = 0; j < 3; j++)
                {
                    indices[(i * 3 + j) * 6] = (i + 1) * 4 + j + 1;
                    indices[(i * 3 + j) * 6 + 1] = i * 4 + j + 1;
                    indices[(i * 3 + j) * 6 + 2] = i * 4 + j;
                    indices[(i * 3 + j) * 6 + 3] = (i + 1) * 4 + j;
                    indices[(i * 3 + j) * 6 + 4] = (i + 1) * 4 + j + 1;
                    indices[(i * 3 + j) * 6 + 5] = i * 4 + j;
                }
            IndexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.IndexBuffer, indices);
            Size = size;
        }

        public override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            int stride = Utilities.SizeOf<VertexDefinition.PositionTexture>(); //Gets or sets the stride between vertex elements in the buffer (in bytes). 
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, stride, 0));
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            _shader.Render(deviceContext, IndexCount, worldMatrix, viewMatrix, projectionMatrix, _texture.TextureResource);
        }

        public override void Update()
        {
            float[] xVertexCoordinates = { 0f, _fixedBorderRadius, Size.X - _fixedBorderRadius, Size.X };
            float[] yVertexCoordinates = { 0f, _fixedBorderRadius, Size.Y - _fixedBorderRadius, Size.Y };
            float[] xTextureCoordinates = { 0f, (float)_fixedBorderRadius / _texture.Width, 1f - (float)_fixedBorderRadius / _texture.Width, 1f };
            float[] yTextureCoordinates = { 0f, (float)_fixedBorderRadius / _texture.Height, 1f - (float)_fixedBorderRadius / _texture.Height, 1f };
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                    _vertices[x*4+y]  = new VertexDefinition.PositionTexture
                    {
                        position = new Vector3(xVertexCoordinates[x], yVertexCoordinates[y], 0), 
                        texture = new Vector2(xTextureCoordinates[x], yTextureCoordinates[y])
                    };

            DataStream mappedResource;
            DeviceContext.MapSubresource(VertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None,
                out mappedResource);
            mappedResource.WriteRange(_vertices);
            DeviceContext.UnmapSubresource(VertexBuffer, 0);
        }

        public override void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(VertexBuffer, IndexBuffer);
        }
    }
}
