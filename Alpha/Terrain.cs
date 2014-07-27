using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Graphics;
using Alpha.Graphics.Shaders;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Alpha
{
    class Terrain
    {
        private Buffer _vertexBuffer;
        private Buffer _indexBuffer;
        private int _indexCount;

        private readonly ShaderResourceView[] _terrainTextures;
        private const int TextureRepeat = 5;

        public Terrain(IRenderer renderer, IList<LandProvince> provinces)
        {
            _terrainTextures = new[]
            {
                renderer.TextureManager.Create("Sand.png").TextureResource,
                renderer.TextureManager.Create("Grass.png").TextureResource,
                renderer.TextureManager.Create("Ground.png").TextureResource,
                renderer.TextureManager.Create("Rock.png").TextureResource
            };
            BuildBuffers(renderer, provinces);
        }
        
        private float TerrainHeight(Vector3 point, IList<LandProvince> provinces)
        {
            return 0.0f;
        }

        private void BuildBuffers(IRenderer renderer, IList<LandProvince> provinces)
        {
            List<Vector3> points = (provinces.SelectMany(p=>p.Zones).SelectMany(s => s.Points.Union(new[]{s.Center}))).Distinct().ToList();
            VertexDefinition.PositionTextureNormal4Weights[] terrainVertices = new VertexDefinition.PositionTextureNormal4Weights[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                float height = TerrainHeight(points[i], provinces);
                // Normal calculation (normal of each shared triangle averaged (todo : weighting by the surface of the triangle)
                Vector3 normal = new Vector3(1,0,0);

                terrainVertices[i] = new VertexDefinition.PositionTextureNormal4Weights
                {
                    position = new Vector3(points[i].X, height, points[i].Z),
                    texture = new Vector2((points[i].X / TextureRepeat), (points[i].Z / TextureRepeat)),
                    normal = normal,
                    weights = GetWeights(height)
                };
            }
            _indexCount = provinces.SelectMany(p=>p.Zones).Sum(s => s.Points.Count) * 3;
            UInt32[] terrainIndices = new UInt32[_indexCount];
            int index = 0;
            foreach (Zone zone in provinces.SelectMany(p=>p.Zones))
            {
                for (int i = 0; i < zone.Points.Count; i++)
                {
                    terrainIndices[index] = (uint)points.FindIndex(p => p.Equals(zone.Center));
                    terrainIndices[index+2] = (uint)points.FindIndex(p => p.Equals(zone.Points[i]));
                    terrainIndices[index+1] = (uint)points.FindIndex(p => p.Equals(zone.Points[(i + 1) % zone.Points.Count]));
                    index += 3;
                }
            }
            _vertexBuffer = Buffer.Create(renderer.Device, BindFlags.VertexBuffer, terrainVertices);
            _indexBuffer = Buffer.Create(renderer.Device, BindFlags.IndexBuffer, terrainIndices);
        }

        private Vector4 GetWeights(float altitude)
        {
            Vector4 weights = new Vector4(1)
            {
                X = MathUtil.Clamp((-altitude + 40)/20, 0, 1),
                Y = MathUtil.Clamp(Math.Abs(altitude - 75)/40, 0, 1),
                Z = MathUtil.Clamp(Math.Abs(altitude - 175)/80, 0, 1),
                W = MathUtil.Clamp((altitude - 350)/50, 0, 1)
            };
            weights.Normalize();
            return weights;
        }
        
        public void Render(IRenderer renderer, DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, Light light)
        {
            //Render terrain
            deviceContext.InputAssembler.SetVertexBuffers(0, 
                new VertexBufferBinding(_vertexBuffer, Utilities.SizeOf<VertexDefinition.PositionTextureNormal4Weights>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            renderer.TerrainShader.Render(deviceContext, _indexCount, worldMatrix, viewMatrix, projectionMatrix, light, _terrainTextures);
        }
    }
}
