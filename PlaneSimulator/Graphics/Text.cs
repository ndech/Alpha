using System;
using PlaneSimulator.Graphics.Shaders;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace PlaneSimulator.Graphics
{
    class Text
    {
        public Text(Device device, FontShader shader, int screenWidth, int screenHeight, Font font, Int32 maxLength, Vector4 color)
        {
            Font = font;
            MaxLength = maxLength;
            Color = color;
            _shader = shader;
            _screenHeight = screenHeight;
            _screenWidth = screenWidth;
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

            IndexBuffer = Buffer.Create(device, BindFlags.IndexBuffer, indices);

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
            VertexBuffer = Buffer.Create(device, _vertices, vertexBufferDesc);
        }

        public Buffer IndexBuffer { get; private set; }
        public Buffer VertexBuffer { get; private set; }
        public int MaxLength { get; private set; }

        private FontShader.Vertex[] _vertices;
        private bool _changed = true;
        private String _content;
        private readonly FontShader _shader;
        private int _screenWidth;
        private int _screenHeight;

        public String Content
        {
            set
            {
                if (value != _content)
                {
                    _changed = true;
                    _content = value;
                    if (_content.Length > MaxLength)
                        _content = _content.Substring(0, MaxLength);
                }
            }
        }

        private Vector2 _position;
        public Vector2 Position
        {
            set
            {
                if (value != _position)
                {
                    _changed = true;
                    _position = value;
                }
            }
        }

        public Vector4 Color { get; set; }
        public Font Font { get; private set; }

        public void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix orthoMatrix)
        {
            if (_changed) // The vertex buffer need to be rebuilt and transfered to the GPU
            {
                // Calculate the X and Y pixel position on screen to start drawing to.
                int drawX = -(_screenWidth / 2) + (int)_position.X;
                int drawY = (_screenHeight >> 1) - (int)_position.Y;

                int width, height;
                // Use the font class to build the vertex array from the sentence text and sentence draw location.
                Font.BuildVertexArray(_content, drawX, drawY, ref _vertices, out width, out height);

                DataStream mappedResource;
                deviceContext.MapSubresource(VertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);
                mappedResource.WriteRange<FontShader.Vertex>(_vertices);
                deviceContext.UnmapSubresource(VertexBuffer, 0);
                _changed = false;
            }
            
            var stride = Utilities.SizeOf<TextureShader.Vertex>();
			deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, stride, 0));
			deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
			deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

			_shader.Render(deviceContext, _content.Length*6, worldMatrix, viewMatrix, orthoMatrix, Font.Texture, Color);
        }
    }
}
