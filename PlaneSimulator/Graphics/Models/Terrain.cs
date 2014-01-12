﻿using System;
using PlaneSimulator.Graphics.Shaders;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace PlaneSimulator.Graphics
{
    class Terrain
    {
        private int _width;
        private int _height;
        private int _pitch;

        private Buffer VertexBuffer { get; set; }
        private Buffer IndexBuffer { get; set; }
        public int IndexCount { get; private set; }
        private System.Drawing.Bitmap HeightMap { get; set; }

        public Terrain(Device device, String texture, int pitch)
        {
            HeightMap = new System.Drawing.Bitmap(@"Data/Textures/"+texture);
            _width = HeightMap.Width-1;
            _height = HeightMap.Height-1;
            _pitch = pitch;
            BuildBuffers(device);
        }

        public float GetHeight(int x, int y)
        {
            return (HeightMap.GetPixel(x, y).G - 80)*5;
        }

        public float GetHeight(float x, float y)
        {
            return 0;
        }
        private void BuildBuffers(Device device)
        {
            ColorShader.Vertex[] vertices = new ColorShader.Vertex[(_width+1)*(_height+1)];
            for (int i = 0; i < (_width + 1); i++)
                for (int j = 0; j < (_height + 1); j++)
                    vertices[i*(_width + 1) + j] = new ColorShader.Vertex
                    {
                        position = new Vector3((-(_width / 2) + i) * _pitch, GetHeight(i,j) , (-(_height / 2) + j) * _pitch), 
                        color = GetHeight(i,j) > 0 ? new Vector4(1, 0, 0, 1) : new Vector4(0,0,1,1)
                    };
            IndexCount = _width*_height*6;
            UInt32[] indices = new UInt32[IndexCount];
            for (int i = 0; i < (_width); i++)
                for (int j = 0; j < (_height); j++)
                {
                    indices[(i * _width + j) * 6] = (uint)(i * (_width + 1) + j + 1); //Left top
                    indices[(i * _width + j) * 6 + 1] = (uint)((i + 1) * (_width + 1) + j); //Right bottom
                    indices[(i * _width + j) * 6 + 2] = (uint)(i * (_width + 1) + j); //Left bottom
                    indices[(i * _width + j) * 6 + 3] = (uint)(i * (_width + 1) + j + 1); //Left top
                    indices[(i * _width + j) * 6 + 4] = (uint)((i + 1) * (_width + 1) + j + 1); //Right top
                    indices[(i * _width + j) * 6 + 5] = (uint)((i + 1) * (_width + 1) + j); //Right bottom
                }
            VertexBuffer = Buffer.Create(device, BindFlags.VertexBuffer, vertices);
            IndexBuffer = Buffer.Create(device, BindFlags.IndexBuffer, indices);
        }

        public void Render(DeviceContext deviceContext)
        {
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<ColorShader.Vertex>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
    }
}
