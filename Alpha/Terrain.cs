using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Graphics;
using Alpha.Graphics.Shaders;
using Alpha.Voronoi;
using Alpha.WorldGeneration;
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

        public Terrain(IRenderer renderer, List<VoronoiSite> sites)
        {
            _terrainTextures = new[]
            {
                renderer.TextureManager.Create("Sand.png").TextureResource,
                renderer.TextureManager.Create("Grass.png").TextureResource,
                renderer.TextureManager.Create("Ground.png").TextureResource,
                renderer.TextureManager.Create("Rock.png").TextureResource
            };
            BuildBuffers(renderer, sites);
        }
        
        private float TerrainHeight(Vector point, List<VoronoiSite> sites)
        {
            List<VoronoiSite> adjacentSites = sites.Where(s => s.Points.Contains(point)).ToList();
            if (adjacentSites.Count == 0)
                adjacentSites = sites.Where(s => s.Center.Equals(point)).ToList();
            return (float)adjacentSites.Average(s => s.IsWater ? 0 : s.ShoreDistance - 1) * 30;
        }

        private void BuildBuffers(IRenderer renderer, List<VoronoiSite> sites)
        {
            List<Vector> points = (sites.Where(s=>!s.IsWater).SelectMany(s => s.Points).Union(sites.Where(s=>!s.IsWater).Select(s=>s.Center))).Distinct().ToList();
            VertexDefinition.PositionTextureNormal4Weights[] terrainVertices = new VertexDefinition.PositionTextureNormal4Weights[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                List<VoronoiSite> adjacentSites = sites.Where(s => s.Points.Contains(points[i])).ToList();
                float height = TerrainHeight(points[i], sites);
                // Normal calculation (normal of each shared triangle averaged (todo : weighting by the surface of the triangle)
                Vector3 normal = new Vector3(1,0,0);
                //foreach (VoronoiSite site in adjacentSites)
                //{
                //    int pointIndex = site.Points.IndexOf(points[i]);
                //    Vector3 point = new Vector3((float)points[i][0], height, (float)points[i][1]);
                //    Vector3 center = new Vector3((float)site.Center[0], TerrainHeight(site.Center, sites), (float)site.Center[1]);
                //    Vector3 previous = new Vector3((float)site.Points[(pointIndex+site.Points.Count-1)%site.Points.Count][0], TerrainHeight(site.Points[(pointIndex+site.Points.Count-1)%site.Points.Count], sites), (float)points[(pointIndex+site.Points.Count-1)%site.Points.Count][1]);
                //    Vector3 next = new Vector3((float)site.Points[(pointIndex + 1) % site.Points.Count][0], TerrainHeight(site.Points[(pointIndex + 1) % site.Points.Count], sites), (float)points[(pointIndex + 1) % site.Points.Count][1]);
                //    normal += Vector3.Cross(previous-point, center-point);
                //    normal += Vector3.Cross(center-point, next-point);
                //}
                //normal.Normalize();

                terrainVertices[i] = new VertexDefinition.PositionTextureNormal4Weights
                {
                    position = new Vector3((float)points[i][0], height, (float)points[i][1]),
                    texture = new Vector2(((float)points[i][0] / TextureRepeat), ((float)points[i][1] / TextureRepeat)),
                    normal = normal,
                    weights = GetWeights(height)
                };
            }
            _indexCount = sites.Where(s => !s.IsWater).Sum(s => s.Points.Count) * 3;
            UInt32[] terrainIndices = new UInt32[_indexCount];
            int index = 0;
            foreach (VoronoiSite site in sites.Where(s=>!s.IsWater))
            {
                for (int i = 0; i < site.Points.Count; i++)
                {
                    terrainIndices[index] = (uint)points.FindIndex(p => p.Equals(site.Center));
                    terrainIndices[index+2] = (uint)points.FindIndex(p => p.Equals(site.Points[i]));
                    terrainIndices[index+1] = (uint)points.FindIndex(p => p.Equals(site.Points[(i + 1) % site.Points.Count]));
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
