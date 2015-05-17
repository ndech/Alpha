using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Provinces;
using Alpha.DirectX.Shaders;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Alpha.DirectX.UI.World
{
    class Water : IDisposable
    {
        private Buffer _vertexBuffer;
        private Buffer _indexBuffer;
        private int _indexCount;
        private readonly ShaderResourceView _bumpMapTexture;
        private readonly ShaderResourceView _borderTexture;
        private const int WaveTextureRepeat = 3;
        private Vector2 _waveTranslation = new Vector2(0,0);
        private readonly WaterShader _shader;

        public Water(IContext context, IEnumerable<SeaProvince> provinces)
        {
            _bumpMapTexture = context.TextureManager.Create("OceanWater.png").TextureResource;
            _borderTexture = context.TextureManager.Create("Border.png").TextureResource;
            _shader = context.Shaders.Get<WaterShader>();
            BuildBuffers(context, provinces);
        }

        public void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, Sun sun)
        {
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, Utilities.SizeOf<VertexDefinition.WaterVertex>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _shader.Render(deviceContext, _indexCount, worldMatrix, viewMatrix, projectionMatrix, _bumpMapTexture, _borderTexture, _waveTranslation, sun);
        }

        public void Update(double delta)
        {
            _waveTranslation += new Vector2(0.4f, 0.9f) * (float)delta;
        }

        private void BuildBuffers(IContext context, IEnumerable<SeaProvince> provinces)
        {
            _indexCount = provinces.SelectMany(p=>p.Zones).Sum(z => z.Points.Count) * 3;
            uint[] waterIndices = new uint[_indexCount];
            VertexDefinition.WaterVertex[] waterVertices = new VertexDefinition.WaterVertex[_indexCount];
            int index = 0;
            foreach (Zone zone in provinces.SelectMany(p=>p.Zones))
            {
                for (int i = 0; i < zone.Points.Count; i++)
                {
                    Vector3 pointA = (Vector3)zone.Points[i];
                    Vector3 pointB = (Vector3)zone.Points[(i + 1)%zone.Points.Count];
                    Vector3 center = (Vector3)zone.Center;

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
            _vertexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.VertexBuffer, waterVertices);
            _indexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.IndexBuffer, waterIndices);
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(_vertexBuffer, _indexBuffer);
        }
    }
}
