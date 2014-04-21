using System;
using System.Collections.Generic;
using Alpha.Graphics.Shaders;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Alpha.Graphics
{
    class Text
    {
        public Text(IRenderer renderer, FontShader shader, Font font, Int32 maxLength, Color color)
        {
            Font = font;
            MaxLength = maxLength;
            Color = color;
            _shader = shader;
            _screenSize = renderer.ScreenSize;
            _icons = new List<TexturedRectangle>();
            // The index buffer is static and do not change when the text changes
            UInt32[] indices = new UInt32[maxLength * 6]; // 6 indices per character

            for (UInt32 i = 0; i < maxLength; i++)
            {
                indices[i * 6] = i * 4;
                indices[i * 6 + 1] = i * 4 + 1;
                indices[i * 6 + 2] = i * 4 + 2;
                indices[i * 6 + 3] = i * 4;
                indices[i * 6 + 4] = i * 4 + 3;
                indices[i * 6 + 5] = i * 4 + 1;
            }

            _indexBuffer = Buffer.Create(renderer.Device, BindFlags.IndexBuffer, indices);

            // The vertex buffer is initialized empty
            _vertices = new FontShader.Vertex[maxLength * 4]; // 4 vertices per character

            var vertexBufferDesc = new BufferDescription
            {
                Usage = ResourceUsage.Dynamic,
                SizeInBytes = Utilities.SizeOf<FontShader.Vertex>() * maxLength * 4,
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            };

            // Create the vertex buffer.
            _vertexBuffer = Buffer.Create(renderer.Device, _vertices, vertexBufferDesc);
        }

        private readonly Buffer _indexBuffer;
        private Buffer _vertexBuffer;
        public int MaxLength { get; private set; }

        public Vector2I Size { get; private set; }

        private FontShader.Vertex[] _vertices;
        private String _content;
        private readonly FontShader _shader;
        private readonly List<TexturedRectangle> _icons;

        public String Content
        {
            get { return _content; }
            set
            {
                if (value != _content)
                {
                    _content = value;
                    if (_content.Length > MaxLength)
                        _content = _content.Substring(0, MaxLength);
                    Update();
                }
            }
        }


        public Vector2I Position { get; set; }
        public Color Color { get; set; }
        public Font Font { get; private set; }

        private Vector2I _screenSize;

        public void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix orthoMatrix)
        {
			deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, Utilities.SizeOf<FontShader.Vertex>(), 0));
			deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
			deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            int drawX = -(_screenSize.X >> 1) + Position.X;
            int drawY = (_screenSize.Y >> 1) - Position.Y;
            Matrix position = Matrix.Translation(drawX, drawY, 0);
			_shader.Render(deviceContext, _content.Length*6, worldMatrix*position, viewMatrix, orthoMatrix, Font.Texture.TextureResource);
            foreach (TexturedRectangle icon in _icons)
                icon.Render(deviceContext, worldMatrix * position, viewMatrix, orthoMatrix);
        }

        public void Update()
        {
            // Use the font class to build the vertex array from the sentence text and sentence draw location.
            Size = Font.UpdateVertexArray(_content, ref _vertices, ref _vertexBuffer, Color, _icons);
        }
    }
}
