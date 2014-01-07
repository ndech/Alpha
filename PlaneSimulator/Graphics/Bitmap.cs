using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using PlaneSimulator.Graphics.Shaders;
using PlaneSimulator.Toolkit;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace PlaneSimulator.Graphics
{
    class Bitmap : IDisposable
    {
        public Texture Texture { get; private set; }
        public Buffer VertexBuffer { get; private set; }
        public Buffer IndexBuffer { get; private set; }
        public int VertexCount { get; private set; }
        public int IndexCount { get; private set; }

        public Vector2 ScreenSize { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (value != _position)
                    _changed = true;
                _position = value;
            }
        }
        private Vector2 _position;
        private TextureShader.Vertex[] _vertices;
        private bool _changed;

        public Bitmap(Device device, String textureName, int screenWidth, int screenHeight, int width, int height)
        {
            Texture = new Texture(device, textureName);
            ScreenSize = new Vector2(screenWidth, screenHeight);
            Size = new Vector2(width, height);
            _changed = true;

            VertexCount = 4;
            IndexCount = 6;

            _vertices = new TextureShader.Vertex[VertexCount];
            UInt32[] indices =  {0, 1, 2, 0, 3, 1};

            VertexBuffer = Buffer.Create(device, _vertices, 
                new BufferDescription
                {
					Usage = ResourceUsage.Dynamic,
					SizeInBytes = Utilities.SizeOf<TextureShader.Vertex>() * VertexCount,
					BindFlags = BindFlags.VertexBuffer,
					CpuAccessFlags = CpuAccessFlags.Write,
					OptionFlags = ResourceOptionFlags.None,
					StructureByteStride = 0
				});

            IndexBuffer = Buffer.Create(device, BindFlags.IndexBuffer, indices);
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(Texture, VertexBuffer, IndexBuffer);
        }

        public void Render(DeviceContext deviceContext)
        {
            if (_changed)
            {
                float left = (float)((ScreenSize.X / 2) * -1) + (float)Position.X;
                float right = left + (float)Size.X;
                float top = (float)(ScreenSize.Y / 2) - (float)Position.Y;
                float bottom = top - (float)Size.Y;

                _vertices[0] = new TextureShader.Vertex { position = new Vector3(left, top, 0.0f), texture = new Vector2(0.0f, 0.0f) };
                _vertices[1] = new TextureShader.Vertex { position = new Vector3(right, bottom, 0.0f), texture = new Vector2(1.0f, 1.0f) };
                _vertices[2] = new TextureShader.Vertex { position = new Vector3(left, bottom, 0.0f), texture = new Vector2(0.0f, 1.0f) };
                _vertices[3] = new TextureShader.Vertex { position = new Vector3(right, top, 0.0f), texture = new Vector2(1.0f, 0.0f) };

                DataStream mappedResource;
                deviceContext.MapSubresource(VertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);
			    mappedResource.WriteRange(_vertices);
			    deviceContext.UnmapSubresource(VertexBuffer, 0);
            }

            // Set vertex buffer stride and offset.
            int stride = Utilities.SizeOf<TextureShader.Vertex>(); //Gets or sets the stride between vertex elements in the buffer (in bytes). 
            int offset = 0; //Gets or sets the offset from the start of the buffer of the first vertex to use (in bytes). 
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, stride, offset));
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
    }
}
