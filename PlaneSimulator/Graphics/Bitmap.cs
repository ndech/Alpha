using System;
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
    public class Bitmap : IDisposable
    {
        public Texture Texture { get; private set; }
        public Buffer VertexBuffer { get; private set; }
        public Buffer IndexBuffer { get; private set; }
        public int VertexCount { get; private set; }
        public int IndexCount { get; private set; }

        public Vector2 ScreenSize { get; set; }

        public Vector2 Size
        {
            get { return _size; }
            set
            {
                if (value != _size)
                    _changed = true;
                _size = value;
            }
        }

        private Vector2 _size;
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
        private TranslateShader.Vertex[] _vertices;
        private bool _changed;
        public float Depth { get; set; }

        public Bitmap(Device device, Texture texture, int screenWidth, int screenHeight, int width, int height, float depth = 0.0f)
        {
            Texture = texture;
            ScreenSize = new Vector2(screenWidth, screenHeight);
            Size = new Vector2(width, height);
            _changed = true;
            Depth = depth;

            VertexCount = 4;
            IndexCount = 6;

            _vertices = new TranslateShader.Vertex[VertexCount];
            UInt32[] indices =  {0, 1, 2, 0, 3, 1};

            VertexBuffer = Buffer.Create(device, _vertices, 
                new BufferDescription
                {
					Usage = ResourceUsage.Dynamic,
					SizeInBytes = Utilities.SizeOf<TranslateShader.Vertex>() * VertexCount,
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

                _vertices[0] = new TranslateShader.Vertex { position = new Vector3(left, top, Depth), texture = new Vector2(0.0f, 0.0f) };
                _vertices[1] = new TranslateShader.Vertex { position = new Vector3(right, bottom, Depth), texture = new Vector2(1.0f, 1.0f) };
                _vertices[2] = new TranslateShader.Vertex { position = new Vector3(left, bottom, Depth), texture = new Vector2(0.0f, 1.0f) };
                _vertices[3] = new TranslateShader.Vertex { position = new Vector3(right, top, Depth), texture = new Vector2(1.0f, 0.0f) };

                DataStream mappedResource;
                deviceContext.MapSubresource(VertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);
			    mappedResource.WriteRange(_vertices);
			    deviceContext.UnmapSubresource(VertexBuffer, 0);
                _changed = false;
            }

            // Set vertex buffer stride and offset.
            int stride = Utilities.SizeOf<TranslateShader.Vertex>(); //Gets or sets the stride between vertex elements in the buffer (in bytes). 
            int offset = 0; //Gets or sets the offset from the start of the buffer of the first vertex to use (in bytes). 
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, stride, offset));
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
    }
}
