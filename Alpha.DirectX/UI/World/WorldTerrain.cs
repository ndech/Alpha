using System;
using Alpha.DirectX.Shaders;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Alpha.DirectX.UI.World
{
    class WorldTerrain
    {
        private Buffer _terrainVertexBuffer;
        private Buffer _terrainIndexBuffer;
        private int _terrainIndexCount;
        private readonly WorldTerrainShader _terrainShader;

        private Buffer _waterVertexBuffer;
        private Buffer _waterIndexBuffer;
        private int _waterIndexCount;
        private readonly WorldWaterShader _waterShader;

        private readonly Texture _heightMap;
        private readonly ShaderResourceView _normalMap;
        private readonly ShaderResourceView _provinceMap;

        internal WorldTerrain(IContext context)
        {
            _heightMap = context.TextureManager.Create("heightmap.bmp", "Data/Map/");
            _normalMap = context.TextureManager.Create("normal.bmp", "Data/Map/").TextureResource;
            _provinceMap = context.TextureManager.Create("provinces.bmp", "Data/Map/").TextureResource;
            _terrainShader = context.Shaders.WorldTerrainShader;
            BuildTerrainBuffers(context);
            BuildWaterBuffers(context);
        }

        private void BuildTerrainBuffers(IContext context)
        {
            const int pitch = 4;
            int width = _heightMap.Width / pitch;
            int height = _heightMap.Height / pitch;

            int vertexCount = (width + 1) * (height + 1);
            VertexDefinition.PositionTexture[] terrainVertices = new VertexDefinition.PositionTexture[vertexCount];
            for (int i = 0; i < (width + 1); i++)
                for (int j = 0; j < (height + 1); j++)
                    terrainVertices[j * (width + 1) + i] = new VertexDefinition.PositionTexture
                    {
                        position = new Vector3((-(width / 2) + i) * pitch, 0.0f, -(-(height / 2) + j) * pitch),
                        texture = new Vector2(((float)i / width), ((float)j / height))
                    };

            _terrainIndexCount = width * height * 6;
            UInt32[] terrainIndices = new UInt32[_terrainIndexCount];
            for (int i = 0; i < (height); i++)
                for (int j = 0; j < (width); j++)
                {
                    terrainIndices[(i * width + j) * 6] = (uint)(i * (width + 1) + j + 1); //Left top
                    terrainIndices[(i * width + j) * 6 + 1] = (uint)((i + 1) * (width + 1) + j); //Right bottom
                    terrainIndices[(i * width + j) * 6 + 2] = (uint)(i * (width + 1) + j); //Left bottom
                    terrainIndices[(i * width + j) * 6 + 3] = (uint)(i * (width + 1) + j + 1); //Left top
                    terrainIndices[(i * width + j) * 6 + 4] = (uint)((i + 1) * (width + 1) + j + 1); //Right top
                    terrainIndices[(i * width + j) * 6 + 5] = (uint)((i + 1) * (width + 1) + j); //Right bottom
                }
            _terrainVertexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.VertexBuffer, terrainVertices);
            _terrainIndexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.IndexBuffer, terrainIndices);
        }

        private void BuildWaterBuffers(IContext context)
        {
            const int waveRepeat = 5;
            const int vertexCount = 4;
            VertexDefinition.PositionTexture[] waterVertices = new VertexDefinition.PositionTexture[vertexCount];
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    waterVertices[j * 2 + i] = new VertexDefinition.PositionTexture
                    {
                        position = new Vector3((-(1 / 2) + i) * _heightMap.Width, 0.0f, -(-(1 / 2) + j) * _heightMap.Height),
                        texture = new Vector2(i, j)
                    };

            _waterIndexCount = 6;
            UInt32[] waterIndices = { 0, 3, 2, 0, 1, 3 };
            _waterVertexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.VertexBuffer, waterVertices);
            _waterIndexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.IndexBuffer, waterIndices);
        }

        public void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_terrainVertexBuffer, Utilities.SizeOf<VertexDefinition.PositionTexture>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(_terrainIndexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _terrainShader.Render(deviceContext, _terrainIndexCount, worldMatrix, viewMatrix, projectionMatrix, _heightMap.TextureResource);

            //deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_terrainVertexBuffer, Utilities.SizeOf<VertexDefinition.PositionTexture>(), 0));
            //deviceContext.InputAssembler.SetIndexBuffer(_terrainIndexBuffer, Format.R32_UInt, 0);
            //deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            //_waterShader.Render(deviceContext, _terrainIndexCount, worldMatrix, viewMatrix, projectionMatrix, _heightMap.TextureResource);
        }

        public void Update(double delta)
        { }
    }
}
