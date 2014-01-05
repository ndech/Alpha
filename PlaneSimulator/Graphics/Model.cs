using System;
using PlaneSimulator.Toolkit;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using PlaneSimulator.Graphics.Shaders;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace PlaneSimulator.Graphics
{
    class Model : IDisposable
    {
        Buffer VertexBuffer { get; set; }
        Buffer IndexBuffer { get; set; }
        public int VertexCount { get; private set; }
        public int IndexCount { get; private set; }
        public Texture Texture { get; private set; }

        public Model(Device device)
        {
            VertexCount = 4;
            IndexCount = 6;

            // Create the vertex array and load it with data.
            var vertices = new[]
			{   
                new LightShader.Vertex
				{
					position = new Vector3(-1, -1, 0),
					texture = new Vector2(0, 0),
                    normal = new Vector3(0, 0, -1.0f)
				},
				new LightShader.Vertex
				{
					position = new Vector3(-1, 1, 0),
					texture = new Vector2(0, 1),
                    normal = new Vector3(0, 0, -1.0f)
				},
				new LightShader.Vertex
				{
					position = new Vector3(1, 1, 0),
					texture = new Vector2(1, 1),
                    normal = new Vector3(0, 0, -1.0f)
				},
				new LightShader.Vertex
				{
					position = new Vector3(1, -1, 0),
					texture = new Vector2(1, 0),
                    normal = new Vector3(0, 0, -1.0f)
				}
			};
            var indices = new[]
            {
                0, 1, 3,
                1, 2, 3
            };
            Texture = new Texture(device, "seafloor.dds");
            VertexBuffer = Buffer.Create(device, BindFlags.VertexBuffer, vertices);
            IndexBuffer = Buffer.Create(device, BindFlags.IndexBuffer, indices);
        }

        public void Render(DeviceContext deviceContext)
        {
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<LightShader.Vertex>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(VertexBuffer, IndexBuffer, Texture);
        }
    }
}
