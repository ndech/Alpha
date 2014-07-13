using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Graphics;
using Alpha.Graphics.Shaders;
using Alpha.WorldGeneration;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Alpha
{
    class Water
    {
        private Buffer _vertexBuffer;
        private Buffer _indexBuffer;
        private int _indexCount;
        private readonly ShaderResourceView _bumpMapTexture;
        private readonly ShaderResourceView _borderTexture;
        private const int WaveTextureRepeat = 3;
        private Vector2 _waveTranslation = new Vector2(0,0);

        public Water(IRenderer renderer, List<VoronoiSite> sites)
        {
            _bumpMapTexture = renderer.TextureManager.Create("OceanWater.png").TextureResource;
            _borderTexture = renderer.TextureManager.Create("Border.png").TextureResource;
            BuildBuffers(renderer, sites);
        }

        public void Render(IRenderer renderer, DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, Sun sun)
        {
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, Utilities.SizeOf<VertexDefinition.WaterVertex>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            renderer.WaterShader.Render(deviceContext, _indexCount, worldMatrix, viewMatrix, projectionMatrix, _bumpMapTexture, _borderTexture, _waveTranslation, sun);
        }

        public void Update(double delta)
        {
            _waveTranslation += new Vector2(0.2f, 0.5f) * (float)delta;
        }

        private void BuildBuffers(IRenderer renderer, List<VoronoiSite> sites)
        {
            _indexCount = sites.Where(s => s.IsWater).Sum(s => s.Points.Count) * 3;
            UInt32[] waterIndices = new UInt32[_indexCount];
            VertexDefinition.WaterVertex[] waterVertices = new VertexDefinition.WaterVertex[_indexCount];
            int index = 0;
            foreach (VoronoiSite site in sites.Where(s => s.IsWater))
            {
                for (int i = 0; i < site.Points.Count; i++)
                {
                    Vector3 pointA = new Vector3((float)site.Points[i][0], 0.0f, (float)site.Points[i][1]);
                    Vector3 pointB = new Vector3((float)site.Points[(i + 1) % site.Points.Count][0], 0.0f,
                        (float)site.Points[(i + 1) % site.Points.Count][1]);
                    Vector3 center = new Vector3((float)site.Center[0], 0.0f, (float)site.Center[1]);
                    float x = Vector3.Dot(center - pointA, pointB - pointA);
                    x /= Vector3.DistanceSquared(pointA, pointB);
                    Vector3 intersection = new Vector3(pointA.X + (x * (pointB.X - pointA.X)), 0.0f, pointA.Z + (x * (pointB.Z - pointA.Z)));


                    waterVertices[index] = new VertexDefinition.WaterVertex
                    {
                        position = center,
                        bumpTexture = new Vector2((center.X / WaveTextureRepeat), (center.Z / WaveTextureRepeat)),
                        borderTexture = new Vector2(Vector3.Distance(intersection, center) / 8, x)
                    };
                    waterVertices[index + 1] = new VertexDefinition.WaterVertex
                    {
                        position = pointB,
                        bumpTexture = new Vector2((pointB.X / WaveTextureRepeat), (pointB.Z / WaveTextureRepeat)),
                        borderTexture = new Vector2(0.0f, 0.0f)
                    };
                    waterVertices[index + 2] = new VertexDefinition.WaterVertex
                    {
                        position = pointA,
                        bumpTexture = new Vector2((pointA.X / WaveTextureRepeat), (pointA.Z / WaveTextureRepeat)),
                        borderTexture = new Vector2(0.0f, 1.0f)
                    };

                    waterIndices[index] = (uint)index;
                    waterIndices[index + 1] = (uint)index + 1;
                    waterIndices[index + 2] = (uint)index + 2;
                    index += 3;
                }
            }
            _vertexBuffer = Buffer.Create(renderer.Device, BindFlags.VertexBuffer, waterVertices);
            _indexBuffer = Buffer.Create(renderer.Device, BindFlags.IndexBuffer, waterIndices);
        }
    }
}
