using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Alpha.Graphics;
using Alpha.Graphics.Shaders;
using Alpha.UI.Coordinates;
using Alpha.Voronoi;
using Alpha.WorldGeneration;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace Alpha.UI.Controls
{
    class Terrain : Control
    {

        private int _width;
        private int _height;

        private Buffer TerrainVertexBuffer { get; set; }
        private Buffer TerrainIndexBuffer { get; set; }
        public int TerrainIndexCount { get; private set; }

        private Buffer WaterVertexBuffer { get; set; }
        private Buffer WaterIndexBuffer { get; set; }
        public int WaterIndexCount { get; private set; }

        private IRenderer Renderer { get; set; }
        private readonly Vector4 _reflectionClippingPlane;
        private readonly Vector4 _refractionClippingPlane;
        private readonly Vector4 _noClippingPlane;
        private RenderTexture _refractionTexture;
        private RenderTexture _reflectionTexture;
        private ShaderResourceView _waterBumpMap;
        private readonly ShaderResourceView[] _terrainTextures;
        private ObjModel _skydome;
        private const int TerrainTextureRepeat = 5;
        private const int WaveTextureRepeat = 1;
        private Vector2 _waveTranslation;
        private Light _light;
        private ICamera _camera;

        public Terrain(IGame game) : base(game, "terrain", new UniRectangle(0, 0, 1.0f, 1.0f))
        {
            Overlay = true;
            _width = 2000;
            _height = 1000;
            _terrainTextures = new ShaderResourceView[4];

            _reflectionClippingPlane = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);
            _refractionClippingPlane = new Vector4(0.0f, -1.0f, 0.0f, 0.0f);
            _noClippingPlane = new Vector4(0.0f, 1.0f, 0.0f, 10000);

            _waveTranslation = new Vector2(0, 0);
        }

        public override void Initialize()
        {
            Renderer = Game.Services.GetService<IRenderer>();
            _terrainTextures[0] = Renderer.TextureManager.Create("Sand.png").TextureResource;
            _terrainTextures[1] = Renderer.TextureManager.Create("Grass.png").TextureResource;
            _terrainTextures[2] = Renderer.TextureManager.Create("Ground.png").TextureResource;
            _terrainTextures[3] = Renderer.TextureManager.Create("Rock.png").TextureResource;

            _waterBumpMap = Renderer.TextureManager.Create("OceanWater.png").TextureResource;

            _reflectionTexture = new RenderTexture(Renderer.Device, Renderer.ScreenSize);
            _refractionTexture = new RenderTexture(Renderer.Device, Renderer.ScreenSize);
            _skydome = new ObjModel(Renderer.Device, "skydome.obj", Renderer.TextureManager.Create("Sky.png"));

            _light = new Light
            {
                Direction = new Vector3(1.0f, -1.0f, 0.0f),
                Color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                AmbiantColor = new Vector4(1),
                SpecularPower = 1.0f,
                SpecularColor = new Vector4(1.0f, 1.0f, 0.7f, 1.0f)
            };

            _camera = Game.Services.GetService<ICamera>();
            List<VoronoiSite> sites = Generator.Create(_width, _height, 1000, 1);
            BuildBuffers(sites);
        }


        //public Vector3 GetNormal(int x, int y)
        //{
        //    if (x > 1 && y > 1 && x < _width && y < _height)
        //    {
        //        var temp = Vector3.Normalize(new Vector3((GetHeight(x - 1, y) - GetHeight(x + 1, y))
        //                                            , 2.0f
        //                                            , (GetHeight(x, y - 1) - GetHeight(x, y + 1))
        //                                            ));
        //        return temp;
        //    }
        //    else
        //        return new Vector3(0.0f, 1.0f, 0.0f);
        //}

        private void BuildBuffers(List<VoronoiSite> sites)
        {
            List<Vector> points = (sites.SelectMany(s => s.Points).Union(sites.Select(s=>s.Center))).Distinct().ToList();
            VertexDefinition.PositionTextureNormal4Weights[] terrainVertices = new VertexDefinition.PositionTextureNormal4Weights[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                List<VoronoiSite> adjacentSites = sites.Where(s => s.Points.Contains(points[i])).ToList();
                if (adjacentSites.Count == 0)
                    adjacentSites = sites.Where(s => s.Center.Equals(points[i])).ToList();
                float height = (float)adjacentSites.Average(s => s.ShoreDistance)*20;
                terrainVertices[i] = new VertexDefinition.PositionTextureNormal4Weights
                {
                    position = new Vector3((float)points[i][0], height, (float)points[i][1]),
                    texture = new Vector2(((float)points[i][0] / TerrainTextureRepeat), ((float)points[i][1] / TerrainTextureRepeat)),
                    normal = new Vector3(0, 1, 0),
                    weights = GetWeights(height)
                };
            }
            TerrainIndexCount = sites.Where(s => !s.IsWater).Sum(s => s.Points.Count) * 3;
            UInt32[] terrainIndices = new UInt32[TerrainIndexCount];
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
            TerrainVertexBuffer = Buffer.Create(Renderer.Device, BindFlags.VertexBuffer, terrainVertices);
            TerrainIndexBuffer = Buffer.Create(Renderer.Device, BindFlags.IndexBuffer, terrainIndices);
            //TerrainIndexCount = _width * _height * 6;
            //UInt32[] terrainIndices = new UInt32[TerrainIndexCount];
            //for (int i = 0; i < (_width); i++)
            //    for (int j = 0; j < (_height); j++)
            //    {
            //        terrainIndices[(i * _width + j) * 6] = (uint)(i * (_width + 1) + j + 1); //Left top
            //        terrainIndices[(i * _width + j) * 6 + 1] = (uint)((i + 1) * (_width + 1) + j); //Right bottom
            //        terrainIndices[(i * _width + j) * 6 + 2] = (uint)(i * (_width + 1) + j); //Left bottom
            //        terrainIndices[(i * _width + j) * 6 + 3] = (uint)(i * (_width + 1) + j + 1); //Left top
            //        terrainIndices[(i * _width + j) * 6 + 4] = (uint)((i + 1) * (_width + 1) + j + 1); //Right top
            //        terrainIndices[(i * _width + j) * 6 + 5] = (uint)((i + 1) * (_width + 1) + j); //Right bottom
            //    }
            //TerrainVertexBuffer = Buffer.Create(Renderer.Device, BindFlags.VertexBuffer, terrainVertices);
            //TerrainIndexBuffer = Buffer.Create(Renderer.Device, BindFlags.IndexBuffer, terrainIndices);

            //VertexDefinition.PositionTexture[] waterVertices = new VertexDefinition.PositionTexture[(_width + 1) * (_height + 1)];
            //for (int i = 0; i < (_width + 1); i++)
            //    for (int j = 0; j < (_height + 1); j++)
            //        waterVertices[i * (_width + 1) + j] = new VertexDefinition.PositionTexture
            //        {
            //            position = new Vector3((-(_width / 2) + i) * _pitch, GetHeight(i, j), (-(_height / 2) + j) * _pitch),
            //            texture = new Vector2(((float)i / WaveTextureRepeat), ((float)j / WaveTextureRepeat)),
            //        };
            //WaterIndexCount = _width * _height * 6;
            //UInt32[] waterIndices = new UInt32[WaterIndexCount];
            //for (int i = 0; i < (_width); i++)
            //    for (int j = 0; j < (_height); j++)
            //    {
            //        waterIndices[(i * _width + j) * 6] = (uint)(i * (_width + 1) + j + 1); //Left top
            //        waterIndices[(i * _width + j) * 6 + 1] = (uint)((i + 1) * (_width + 1) + j); //Right bottom
            //        waterIndices[(i * _width + j) * 6 + 2] = (uint)(i * (_width + 1) + j); //Left bottom
            //        waterIndices[(i * _width + j) * 6 + 3] = (uint)(i * (_width + 1) + j + 1); //Left top
            //        waterIndices[(i * _width + j) * 6 + 4] = (uint)((i + 1) * (_width + 1) + j + 1); //Right top
            //        waterIndices[(i * _width + j) * 6 + 5] = (uint)((i + 1) * (_width + 1) + j); //Right bottom
            //    }
            //WaterVertexBuffer = Buffer.Create(Renderer.Device, BindFlags.VertexBuffer, waterVertices);
            //WaterIndexBuffer = Buffer.Create(Renderer.Device, BindFlags.IndexBuffer, waterIndices);
        }

        private Vector4 GetWeights(float altitude)
        {
            Vector4 weights = new Vector4(1);
            weights.X = MathUtil.Clamp((-altitude + 40)/20, 0, 1);
            weights.Y = MathUtil.Clamp(Math.Abs(altitude-75)/40,0,1);
            weights.Z = MathUtil.Clamp(Math.Abs(altitude - 175)/80, 0, 1);
            weights.W = MathUtil.Clamp((altitude - 350)/50, 0, 1);
            weights.Normalize();
            return weights;
        }

        public override string ComponentType { get { return "terrain"; } }

        protected override void DisposeItem()
        { }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix,
            Matrix projectionMatrix)
        {
            Renderer.EnableZBuffer();
            viewMatrix = _camera.ViewMatrix;
            projectionMatrix = Renderer.ProjectionMatrix;
            //_skydome.Render(deviceContext);
            //Renderer.LightShader.Render(deviceContext, _skydome.IndexCount, Matrix.Scaling(10000) * Matrix.RotationY(MathUtil.PiOverTwo-_waveTranslation.X/8), viewMatrix, projectionMatrix, _skydome.Texture, _light, _camera);

            //deviceContext.ClearDepthStencilView(Renderer.RenderToTextureDepthStencilView, DepthStencilClearFlags.Depth, 1, 0);
            //_refractionTexture.SetRenderTarget(deviceContext, Renderer.RenderToTextureDepthStencilView);
            //_refractionTexture.ClearRenderTarget(deviceContext, Renderer.RenderToTextureDepthStencilView, 0.0f, 0.0f, 0.0f, 1.0f);
            //deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(TerrainVertexBuffer, Utilities.SizeOf<VertexDefinition.PositionTextureNormal4Weights>(), 0));
            //deviceContext.InputAssembler.SetIndexBuffer(TerrainIndexBuffer, Format.R32_UInt, 0);
            //deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            //Renderer.TerrainShader.Render(deviceContext, TerrainIndexCount, worldMatrix, viewMatrix, projectionMatrix, _light, _terrainTextures, _refractionClippingPlane);

            //deviceContext.ClearDepthStencilView(Renderer.RenderToTextureDepthStencilView, DepthStencilClearFlags.Depth, 1, 0);
            //_reflectionTexture.SetRenderTarget(deviceContext, Renderer.RenderToTextureDepthStencilView);
            //_reflectionTexture.ClearRenderTarget(deviceContext, Renderer.RenderToTextureDepthStencilView, 0.0f, 0.0f, 0.0f, 1.0f);
            //_skydome.Render(deviceContext);
            //Renderer.LightShader.Render(deviceContext, _skydome.IndexCount, Matrix.Scaling(10000) * Matrix.RotationY(MathUtil.PiOverTwo - _waveTranslation.X/8), viewMatrix, projectionMatrix, _skydome.Texture, _light, _camera);
            //deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(TerrainVertexBuffer, Utilities.SizeOf<VertexDefinition.PositionTextureNormal4Weights>(), 0));
            //deviceContext.InputAssembler.SetIndexBuffer(TerrainIndexBuffer, Format.R32_UInt, 0);
            //deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            //Renderer.TerrainShader.Render(deviceContext, TerrainIndexCount, worldMatrix, _camera.ReflectionMatrix, projectionMatrix, _light, _terrainTextures, _reflectionClippingPlane);
            //Renderer.SetBackBufferAsRenderTarget();
            ////Render water
            //deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(WaterVertexBuffer, Utilities.SizeOf<VertexDefinition.PositionTexture>(), 0));
            //deviceContext.InputAssembler.SetIndexBuffer(WaterIndexBuffer, Format.R32_UInt, 0);
            //deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            //Renderer.WaterShader.Render(deviceContext, WaterIndexCount, worldMatrix, viewMatrix, projectionMatrix, _camera.ReflectionMatrix, _reflectionTexture.ShaderResourceView, _refractionTexture.ShaderResourceView, _waterBumpMap, _waveTranslation, _camera.Position);
            ////Render terrain
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(TerrainVertexBuffer, Utilities.SizeOf<VertexDefinition.PositionTextureNormal4Weights>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(TerrainIndexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            Renderer.TerrainShader.Render(deviceContext, TerrainIndexCount, worldMatrix, viewMatrix, projectionMatrix, _light, _terrainTextures, _noClippingPlane);

            Renderer.DisableZBuffer();
        }

        protected override void Update(double delta)
        {
            _waveTranslation += new Vector2(0.1f, 0.15f)*(float)delta;
            if (UiManager.IsAnyKeyPressed(Key.LeftShift, Key.RightShift))
            {
                if (UiManager.IsKeyPressed(Key.Left))
                    _camera.Rotate(1);
                if (UiManager.IsKeyPressed(Key.Right))
                    _camera.Rotate(-1);
            }
            else
            {
                if (UiManager.IsKeyPressed(Key.Left))
                    _camera.Move(-1, 0);
                if (UiManager.IsKeyPressed(Key.Right))
                    _camera.Move(1, 0);
                if (UiManager.IsKeyPressed(Key.Up))
                    _camera.Move(0, 1);
                if (UiManager.IsKeyPressed(Key.Down))
                    _camera.Move(0, -1);
            }
        }
    }
}
