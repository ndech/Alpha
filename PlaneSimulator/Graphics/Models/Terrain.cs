using System;
using PlaneSimulator.Graphics.Shaders;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace PlaneSimulator.Graphics.Models
{
    public class Terrain
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

        public TerrainShader TerrainShader { get; private set; }

        private Texture _texture;

        private const int TextureRepeat = 5;

        public Terrain(Device device, String texture, int pitch)
        {
            HeightMap = new System.Drawing.Bitmap(@"Data/Textures/"+texture);
            WaterShader = new WaterShader(device);
            TerrainShader = new TerrainShader(device);
            _width = HeightMap.Width-1;
            _height = HeightMap.Height-1;
            _pitch = pitch;
            _texture = new Texture(device, "Ground.png");
            BuildBuffers(device);
        }

        public Vector3 GetNormal(int x, int y)
        {
            if (x > 1 && y > 1 && x < _width && y < _height)
            {
                var temp = Vector3.Normalize(new Vector3((GetHeight(x - 1, y) - GetHeight(x + 1, y))
                                                    , 2.0f
                                                    , (GetHeight(x, y - 1) - GetHeight(x, y + 1))
                                                    ));
                return temp;
            }
            else
                return new Vector3(0.0f, 1.0f, 0.0f);
        }

        public float GetHeight(int x, int y)
        {
            if (x < 0 || y < 0 || x > _width || y > _height)
                return 0.0f;
            return (HeightMap.GetPixel(x, y).G - 80)*5;
        }

        public float GetHeight(double x, double y)
        {
            double positionX = (y/_pitch) + (_width/2.0);
            double positionY = (x/_pitch) + (_height/2.0);
            int coordX = (int) positionX;
            int coordY = (int) positionY;
            positionX %= 1;
            positionY %= 1;

            float height = (float)
                (GetHeight(coordX, coordY)*(1-positionX)*(1-positionY)
                +GetHeight(coordX+1, coordY+1)*(positionX)*(positionY)
                +GetHeight(coordX, coordY+1)*(1-positionX)*(positionY)
                +GetHeight(coordX+1, coordY)*(positionX)*(1-positionY));
            return Math.Max(height, 0.0f);
        }

        private void BuildBuffers(Device device)
        {
            VertexDefinition.PositionTextureNormal[] terrainVertices = new VertexDefinition.PositionTextureNormal[(_width+1)*(_height+1)];
            for (int i = 0; i < (_width + 1); i++)
                for (int j = 0; j < (_height + 1); j++)
                    terrainVertices[i * (_width + 1) + j] = new VertexDefinition.PositionTextureNormal
                    {
                        position = new Vector3((-(_width / 2) + i) * _pitch, GetHeight(i, j), (-(_height / 2) + j) * _pitch),
                        texture = new Vector2(((float)i / TextureRepeat), ((float)j / TextureRepeat)),
                        normal = GetNormal(i,j)
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
                        position = new Vector3((-(_width / 2) + i) * _pitch, GetHeight(i, j), (-(_height / 2) + j) * _pitch),
                        color = new Vector4(0.2f, 0.4f, 0.9f, 1)
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

        public void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, Light light)
        {
            //Render water
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(WaterVertexBuffer, Utilities.SizeOf<ColorShader.Vertex>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(WaterIndexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            WaterShader.Render(deviceContext, WaterIndexCount, worldMatrix, viewMatrix, projectionMatrix);
            //Render terrain
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(TerrainVertexBuffer, Utilities.SizeOf<VertexDefinition.PositionTextureNormal>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(TerrainIndexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            TerrainShader.Render(deviceContext, TerrainIndexCount, worldMatrix, viewMatrix, projectionMatrix, light, _texture);
        }
    }
}
