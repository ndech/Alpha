using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Provinces;
using Alpha.DirectX.Shaders;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Alpha.DirectX.UI.World
{
    class Terrain
    {
        private Buffer _vertexBuffer;
        private Buffer _indexBuffer;
        private int _indexCount;
        private readonly ShaderResourceView _borderTexture;
        private ShaderResourceView _provinceColorTexture;
        private ShaderResourceView _realmColorTexture;
        private readonly TerrainShader _shader;
        public RenderingMode CurrentRenderingMode { get; set; }

        public enum RenderingMode
        {
            Province,
            Realm
        }

        public Terrain(IContext context, IEnumerable<LandProvince> provinces)
        {
            _borderTexture = context.TextureManager.Create("Border.png").TextureResource;
            _shader = context.Shaders.TerrainShader;
            BuildBuffers(context, provinces);
            GenerateProvinceTexture(context, provinces);
            GenerateRealmTexture(context, provinces);
        }

        private void GenerateProvinceTexture(IContext context, IEnumerable<LandProvince> provinces)
        {
            int maxId = provinces.Max(p => p.NumericId);
            Texture1D provinceTexture = new Texture1D(context.DirectX.Device, new Texture1DDescription
            {
                Width = maxId + 1,
                Format = Format.R32G32B32A32_Float,
                BindFlags = BindFlags.ShaderResource,
                ArraySize = 1,
                MipLevels = 1
            });
            int rowPitch = 16 * provinceTexture.Description.Width;
            var byteArray = new byte[rowPitch];
            foreach (LandProvince province in provinces)
            {
                Array.Copy(BitConverter.GetBytes(province.Color.Item1), 0, byteArray, province.NumericId * 16, 4);
                Array.Copy(BitConverter.GetBytes(province.Color.Item2), 0, byteArray, province.NumericId * 16 + 4, 4);
                Array.Copy(BitConverter.GetBytes(province.Color.Item3), 0, byteArray, province.NumericId * 16 + 8, 4);
                Array.Copy(BitConverter.GetBytes(1.0f), 0, byteArray, province.NumericId * 16 + 12, 4);
            }
            DataStream dataStream = new DataStream(rowPitch,true, true);
            dataStream.Write(byteArray,0,rowPitch);
            DataBox data = new DataBox(dataStream.DataPointer, rowPitch, rowPitch);
            //ResourceRegion region = new ResourceRegion();
            context.DirectX.DeviceContext.UpdateSubresource(data, provinceTexture);
            _provinceColorTexture = new ShaderResourceView(context.DirectX.Device, provinceTexture);
        }

        private void GenerateRealmTexture(IContext context, IEnumerable<LandProvince> provinces)
        {
            int maxId = provinces.Max(p => p.NumericId);
            Texture1D provinceTexture = new Texture1D(context.DirectX.Device, new Texture1DDescription
            {
                Width = maxId + 1,
                Format = Format.R32G32B32A32_Float,
                BindFlags = BindFlags.ShaderResource,
                ArraySize = 1,
                MipLevels = 1
            });
            int rowPitch = 16 * provinceTexture.Description.Width;
            var byteArray = new byte[rowPitch];
            foreach (LandProvince province in provinces)
            {
                Array.Copy(BitConverter.GetBytes(province.Owner.Color.Item1), 0, byteArray, province.NumericId * 16, 4);
                Array.Copy(BitConverter.GetBytes(province.Owner.Color.Item2), 0, byteArray, province.NumericId * 16 + 4, 4);
                Array.Copy(BitConverter.GetBytes(province.Owner.Color.Item3), 0, byteArray, province.NumericId * 16 + 8, 4);
                Array.Copy(BitConverter.GetBytes(1.0f), 0, byteArray, province.NumericId * 16 + 12, 4);
            }
            DataStream dataStream = new DataStream(rowPitch, true, true);
            dataStream.Write(byteArray, 0, rowPitch);
            DataBox data = new DataBox(dataStream.DataPointer, rowPitch, rowPitch);
            //ResourceRegion region = new ResourceRegion();
            context.DirectX.DeviceContext.UpdateSubresource(data, provinceTexture);
            _realmColorTexture = new ShaderResourceView(context.DirectX.Device, provinceTexture);
        }

        public void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, Utilities.SizeOf<VertexDefinition.WaterVertex>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _shader.Render(deviceContext, _indexCount, worldMatrix, viewMatrix, projectionMatrix, _borderTexture,
                CurrentRenderingMode == RenderingMode.Realm ? _realmColorTexture : _provinceColorTexture);
        }

        public void Update(double delta)
        { }

        private void BuildBuffers(IContext context, IEnumerable<LandProvince> provinces)
        {
            _indexCount = provinces.SelectMany(p => p.Zones).Sum(z => z.Points.Count) * 3;
            UInt32[] terrainIndices = new UInt32[_indexCount];
            VertexDefinition.TerrainVertex[] terrainVertices = new VertexDefinition.TerrainVertex[_indexCount];
            int index = 0;
            int maxId = provinces.Max(p => p.NumericId);
            foreach (LandProvince province in provinces)
            {
                foreach (Zone zone in province.Zones)
                {
                    for (int i = 0; i < zone.Points.Count; i++)
                    {
                        Vector3 pointA = (Vector3)zone.Points[i];
                        Vector3 pointB = (Vector3)zone.Points[(i + 1) % zone.Points.Count];
                        Vector3 center = (Vector3)zone.Center;

                        float x = Vector3.Dot(center - pointA, pointB - pointA);
                        x /= Vector3.DistanceSquared(pointA, pointB);
                        Vector3 intersection = new Vector3(pointA.X + (x * (pointB.X - pointA.X)), 0.0f, pointA.Z + (x * (pointB.Z - pointA.Z)));
                        int oppositeId = province.Adjacencies.Single(
                            a =>
                                a.CommonPoints.Contains(zone.Points[i]) &&
                                a.CommonPoints.Contains(zone.Points[(i + 1)%zone.Points.Count])).Province.NumericId;
                        Vector2 provinceId = new Vector2((float)province.NumericId/maxId, (float)oppositeId/maxId);
                        terrainVertices[index] = new VertexDefinition.TerrainVertex
                        {
                            position = center,
                            borderTexture = new Vector2(Vector3.Distance(intersection, center) / 8, x),
                            provinceIds = provinceId
                        };
                        terrainVertices[index + 1] = new VertexDefinition.TerrainVertex
                        {
                            position = pointB,
                            borderTexture = new Vector2(0.0f, 0.0f),
                            provinceIds = provinceId
                        };
                        terrainVertices[index + 2] = new VertexDefinition.TerrainVertex
                        {
                            position = pointA,
                            borderTexture = new Vector2(0.0f, 1.0f),
                            provinceIds = provinceId
                        };

                        terrainIndices[index] = (uint)index;
                        terrainIndices[index + 1] = (uint)index + 1;
                        terrainIndices[index + 2] = (uint)index + 2;
                        index += 3;
                    }
                }
            }
            _vertexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.VertexBuffer, terrainVertices);
            _indexBuffer = Buffer.Create(context.DirectX.Device, BindFlags.IndexBuffer, terrainIndices);
        }
    }
}