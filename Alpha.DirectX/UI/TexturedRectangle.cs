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
    class TexturedRectangle : Rectangle, IDisposable
    {
        private readonly TextureShader _shader;
        private readonly VertexDefinition.PositionTexture[] _vertices;
        private readonly ShaderResourceView _texture;
        private readonly DeviceContext _deviceContext;

        public TexturedRectangle(IContext context, Texture texture) : this(context, texture.TextureResource, texture.Size)
        { }
        public TexturedRectangle(IContext context, Texture texture, Vector2I size)
            : this(context, texture.TextureResource, size)
        { }

        public TexturedRectangle(IContext context, ShaderResourceView texture, Vector2I size)
        {
            _shader = context.Shaders.Get<TextureShader>();
            _texture = texture;
            _deviceContext = context.DirectX.Device.ImmediateContext;

            const int vertexCount = 4;
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

            IndexCount = 6;
            uint[] indices = { 0, 1, 2, 0, 3, 1 };
            IndexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.IndexBuffer, indices);
            Size = size;
        }

        public override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix, null);
        }

        public void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, ShaderResourceView texture)
        {

            int stride = Utilities.SizeOf<VertexDefinition.PositionTexture>(); //Gets or sets the stride between vertex elements in the buffer (in bytes). 
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, stride, 0));
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _shader.Render(deviceContext, IndexCount, worldMatrix, viewMatrix, projectionMatrix, texture ?? _texture);
        }

        public override void Update()
        {
            const float left = 0;
            float right = Size.X;
            const float top = 0;
            float bottom = Size.Y;

            _vertices[0] = new VertexDefinition.PositionTexture { position = new Vector3(left, top, 0.0f), texture = new Vector2(0,0)};
            _vertices[1] = new VertexDefinition.PositionTexture { position = new Vector3(right, bottom, 0.0f), texture = new Vector2(1, 1) };
            _vertices[2] = new VertexDefinition.PositionTexture { position = new Vector3(left, bottom, 0.0f), texture = new Vector2(0, 1) };
            _vertices[3] = new VertexDefinition.PositionTexture { position = new Vector3(right, top, 0.0f), texture = new Vector2(1, 0) };

            DataStream mappedResource;
                   _deviceContext.MapSubresource(VertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None,
                out mappedResource);
            mappedResource.WriteRange(_vertices);
            _deviceContext.UnmapSubresource(VertexBuffer, 0);
        }

        public override void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(VertexBuffer, IndexBuffer);
        }
    }
}
