using System;
using System.Collections.Generic;
using System.IO;
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
    class ObjModel : IDisposable
    {
        private Buffer VertexBuffer { get; set; }
        private Buffer IndexBuffer { get; set; }
        public int VertexCount { get; private set; }
        public int IndexCount { get; private set; }
        public Texture Texture { get; private set; }

        public ObjModel(Device device, String modelFileName, String textureFileName)
        {
            StreamReader reader = new StreamReader("Data/Models/"+modelFileName);
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> textures = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();
            List<Tuple<int,int,int>> points = new List<Tuple<int, int, int>>();
            while (!reader.EndOfStream)
            {
                String line = reader.ReadLine();
                if (String.IsNullOrWhiteSpace(line) || line[0] =='#')
                    continue;
                String[] items = line.Split(' ');
                if(items[0] == "v")
                    vertices.Add(new Vector3(float.Parse(items[1]), float.Parse(items[2]), -float.Parse(items[3])));
                if (items[0] == "vt")
                    textures.Add(new Vector2(float.Parse(items[1]), 1.0f - float.Parse(items[2])));
                if (items[0] == "vn")
                    normals.Add(new Vector3(float.Parse(items[1]), float.Parse(items[2]), -float.Parse(items[3])));
                if (items[0] == "f")
                {
                    for (int i = 3; i >0; i--)
                    {
                        String[] items2 = items[i].Split('/');
                            points.Add(new Tuple<int, int, int>(Int32.Parse(items2[0]),Int32.Parse(items2[1]),Int32.Parse(items2[2])));
                    }
                }
            }
            LightShader.Vertex[] verticesDefinition = new LightShader.Vertex[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                verticesDefinition[i].position = vertices[points[i].Item1-1];
                verticesDefinition[i].texture = textures[points[i].Item2-1];
                verticesDefinition[i].normal = normals[points[i].Item3-1];
            }
            VertexCount = points.Count;
            Int32[] indicesDefinition = new int[points.Count];
            for (int i = 0; i < points.Count; i++)
                indicesDefinition[i] = i;
            IndexCount = points.Count;
            Texture = new Texture(device, textureFileName);
            VertexBuffer = Buffer.Create(device, BindFlags.VertexBuffer, verticesDefinition);
            IndexBuffer = Buffer.Create(device, BindFlags.IndexBuffer, indicesDefinition);
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