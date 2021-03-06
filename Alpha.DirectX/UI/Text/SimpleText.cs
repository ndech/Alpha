﻿using System;
using System.Collections.Generic;
using Alpha.DirectX.Shaders;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace Alpha.DirectX.UI.Text
{
    class SimpleText : IDisposable
    {
        public SimpleText(IContext context, Font font, int maxLength, Color color)
        {
            Device device = context.DirectX.Device;
            Font = font;
            MaxLength = maxLength;
            Color = color;
            _shader = context.Shaders.Get<FontShader>();
            _icons = new List<TexturedRectangle>();
            // The index buffer is static and do not change when the text changes
            uint[] indices = new uint[maxLength * 6]; // 6 indices per character

            for (uint i = 0; i < maxLength; i++)
            {
                indices[i * 6] = i * 4;
                indices[i * 6 + 1] = i * 4 + 1;
                indices[i * 6 + 2] = i * 4 + 2;
                indices[i * 6 + 3] = i * 4;
                indices[i * 6 + 4] = i * 4 + 3;
                indices[i * 6 + 5] = i * 4 + 1;
            }

            _indexBuffer = Buffer.Create(device, BindFlags.IndexBuffer, indices);

            // The vertex buffer is initialized empty
            _vertices = new VertexDefinition.PositionTextureColor[maxLength * 4]; // 4 vertices per character

            var vertexBufferDesc = new BufferDescription
            {
                Usage = ResourceUsage.Dynamic,
                SizeInBytes = Utilities.SizeOf<VertexDefinition.PositionTextureColor>() * maxLength * 4,
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            };

            // Create the vertex buffer.
            _vertexBuffer = Buffer.Create(device, _vertices, vertexBufferDesc);
        }

        private readonly Buffer _indexBuffer;
        private Buffer _vertexBuffer;
        private FontShader _shader;
        public int MaxLength { get; }

        public Vector2I Size { get; private set; }

        private VertexDefinition.PositionTextureColor[] _vertices;
        private string _content;
        private readonly List<TexturedRectangle> _icons;

        public string Content
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

        public Color Color { get; set; }
        public Font Font { get; }
        
        public void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix orthoMatrix)
        {
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, Utilities.SizeOf<VertexDefinition.PositionTextureColor>(), 0));
			deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
			deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            
			_shader.Render(deviceContext, _content.Length*6, worldMatrix, viewMatrix, orthoMatrix, Font.Texture.TextureResource);
            foreach (TexturedRectangle icon in _icons)
                icon.Render(deviceContext, worldMatrix, viewMatrix, orthoMatrix);
        }

        public void Update()
        {
            // Use the font class to build the vertex array from the sentence text and sentence draw location.
            Size = Font.UpdateVertexArray(_content, ref _vertices, ref _vertexBuffer, Color, _icons);
        }

        public void Dispose()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }
    }
}
