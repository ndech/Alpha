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
    class Terrain
    {
        private int _width;
        private int _height;
        private int _pitch;

        private Buffer TerrainVertexBuffer { get; set; }
        private Buffer TerrainIndexBuffer { get; set; }
        public int TerrainIndexCount { get; private set; }

        private Buffer WaterVertexBuffer { get; set; }
        private Buffer WaterIndexBuffer { get; set; }
        public int WaterIndexCount { get; private set; }
        private System.Drawing.Bitmap HeightMap { get; set; }

        public WaterShader WaterShader { get; private set; }

        public Terrain(Device device, String texture, int pitch)
        {
            HeightMap = new System.Drawing.Bitmap(@"Data/Textures/"+texture);
            WaterShader = new WaterShader(device);
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
            ColorShader.Vertex[] terrainVertices = new ColorShader.Vertex[(_width+1)*(_height+1)];
            for (int i = 0; i < (_width + 1); i++)
                for (int j = 0; j < (_height + 1); j++)
                    terrainVertices[i*(_width + 1) + j] = new ColorShader.Vertex
                    {
                        position = new Vector3((-(_width / 2) + i) * _pitch, GetHeight(i,j) , (-(_height / 2) + j) * _pitch), 
                        color = new Vector4(1, 0, 0, 1)
                    };
            TerrainIndexCount = _width*_height*6;
            UInt32[] terrainIndices = new UInt32[TerrainIndexCount];
            for (int i = 0; i < (_width); i++)
                for (int j = 0; j < (_height); j++)
                {
                    terrainIndices[(i * _width + j) * 6] = (uint)(i * (_width + 1) + j + 1); //Left top
                    terrainIndices[(i * _width + j) * 6 + 1] = (uint)((i + 1) * (_width + 1) + j); //Right bottom
                    terrainIndices[(i * _width + j) * 6 + 2] = (uint)(i * (_width + 1) + j); //Left bottom
                    terrainIndices[(i * _width + j) * 6 + 3] = (uint)(i * (_width + 1) + j + 1); //Left top
                    terrainIndices[(i * _width + j) * 6 + 4] = (uint)((i + 1) * (_width + 1) + j + 1); //Right top
                    terrainIndices[(i * _width + j) * 6 + 5] = (uint)((i + 1) * (_width + 1) + j); //Right bottom
                }
            TerrainVertexBuffer = Buffer.Create(device, BindFlags.VertexBuffer, terrainVertices);
            TerrainIndexBuffer = Buffer.Create(device, BindFlags.IndexBuffer, terrainIndices);

            ColorShader.Vertex[] waterVertices = new ColorShader.Vertex[(_width + 1) * (_height + 1)];
            for (int i = 0; i < (_width + 1); i++)
                for (int j = 0; j < (_height + 1); j++)
                    waterVertices[i * (_width + 1) + j] = new ColorShader.Vertex
                    {
                        position = new Vector3((-(_width / 2) + i) * _pitch, 0.0f, (-(_height / 2) + j) * _pitch),
                        color = new Vector4(0, 0, 1, 1)
                    };
            WaterIndexCount = _width * _height * 6;
            UInt32[] waterIndices = new UInt32[WaterIndexCount];
            for (int i = 0; i < (_width); i++)
                for (int j = 0; j < (_height); j++)
                {
                    waterIndices[(i * _width + j) * 6] = (uint)(i * (_width + 1) + j + 1); //Left top
                    waterIndices[(i * _width + j) * 6 + 1] = (uint)((i + 1) * (_width + 1) + j); //Right bottom
                    waterIndices[(i * _width + j) * 6 + 2] = (uint)(i * (_width + 1) + j); //Left bottom
                    waterIndices[(i * _width + j) * 6 + 3] = (uint)(i * (_width + 1) + j + 1); //Left top
                    waterIndices[(i * _width + j) * 6 + 4] = (uint)((i + 1) * (_width + 1) + j + 1); //Right top
                    waterIndices[(i * _width + j) * 6 + 5] = (uint)((i + 1) * (_width + 1) + j); //Right bottom
                }
            WaterVertexBuffer = Buffer.Create(device, BindFlags.VertexBuffer, waterVertices);
            WaterIndexBuffer = Buffer.Create(device, BindFlags.IndexBuffer, waterIndices);

        }

        public void Render(DeviceContext deviceContext)
        {
        }

        public void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            //Render terrain
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(TerrainVertexBuffer, Utilities.SizeOf<ColorShader.Vertex>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(TerrainIndexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            WaterShader.Render(deviceContext, TerrainIndexCount, worldMatrix, viewMatrix, projectionMatrix);
            //Render water
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(WaterVertexBuffer, Utilities.SizeOf<ColorShader.Vertex>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(WaterIndexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            WaterShader.Render(deviceContext, WaterIndexCount, worldMatrix, viewMatrix, projectionMatrix);
        }
    }
}
