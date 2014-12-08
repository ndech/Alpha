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
        private Buffer _vertexBuffer;
        private Buffer _indexBuffer;
        private int _indexCount;

        private readonly Texture _heightMap;
        private readonly ShaderResourceView _normalMap;
        private readonly ShaderResourceView _provinceMap;
        private readonly WorldTerrainShader _shader;

        internal WorldTerrain(IContext context)
        {
            _heightMap = context.TextureManager.Create("heightmap.bmp", "Data/Map/");
            _normalMap = context.TextureManager.Create("normal.bmp", "Data/Map/").TextureResource;
            _provinceMap = context.TextureManager.Create("provinces.bmp", "Data/Map/").TextureResource;
            _shader = context.Shaders.WorldTerrainShader;
            BuildBuffers(context);
        }

        private void BuildBuffers(IContext context)
        {
            const int pitch = 4;
            int width = _heightMap.Width / pitch;
            int height = _heightMap.Height/ pitch;
            VertexDefinition.PositionTexture[] terrainVertices = new VertexDefinition.PositionTexture[(width + 1) * (height + 1)];
            for (int i = 0; i < (height + 1); i++)
                for (int j = 0; j < (width + 1); j++)
                    terrainVertices[i * (width + 1) + j] = new VertexDefinition.PositionTexture
                    {
                        position = new Vector3((-(width / 2) + i) * pitch, 0.0f, (-(height / 2) + j) * pitch),
                        texture = new Vector2(((float)j / width), ((float)i / height))
                    };
            _indexCount = width * height * 6;
            UInt32[] terrainIndices = new UInt32[_indexCount];
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
            _vertexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.VertexBuffer, terrainVertices);
            _indexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.IndexBuffer, terrainIndices);
        }

        public void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, Utilities.SizeOf<VertexDefinition.PositionTexture>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _shader.Render(deviceContext, _indexCount, worldMatrix, viewMatrix, projectionMatrix, _heightMap.TextureResource);
        }

        public void Update(double delta)
        { }
    }
}
