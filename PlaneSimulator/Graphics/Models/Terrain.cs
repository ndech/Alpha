using System;
using System.Threading;
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
        
        private readonly Renderer _renderer;

        private readonly Vector4 _reflectionClippingPlane;

        private readonly Vector4 _refractionClippingPlane;

        private readonly Vector4 _noClippingPlane;

        private readonly RenderTexture _refractionTexture;

        private readonly RenderTexture _reflectionTexture;

        private readonly Texture _bumpMap;

        private readonly Bitmap _bitmap;

        private readonly Bitmap _bitmap2;

        private readonly ShaderResourceView[] _terrainTextures;

        private readonly ObjModel _skydome;

        private const int TerrainTextureRepeat = 5;

        private const int WaveTextureRepeat = 1;

        private Vector2 WaveTranslation;

        public Terrain(Device device, String texture, int pitch, Renderer renderer)
        {
            HeightMap = new System.Drawing.Bitmap(@"Data/Textures/"+texture);
            WaterShader = new WaterShader(device);
            TerrainShader = new TerrainShader(device);
            _width = HeightMap.Width-1;
            _height = HeightMap.Height-1;
            _pitch = pitch;
            _terrainTextures = new ShaderResourceView[4];
            _terrainTextures[0] = new Texture(device, "Sand.png").TextureResource;
            _terrainTextures[1] = new Texture(device, "Grass.png").TextureResource;
            _terrainTextures[2] = new Texture(device, "Ground.png").TextureResource;
            _terrainTextures[3] = new Texture(device, "Rock.png").TextureResource;
            _reflectionClippingPlane = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);
            _refractionClippingPlane = new Vector4(0.0f, -1.0f, 0.0f, 0.0f);
            _noClippingPlane = new Vector4(0.0f, 1.0f, 0.0f, 10000);
            _reflectionTexture = new RenderTexture(device, renderer.ScreenSize);
            _refractionTexture = new RenderTexture(device, renderer.ScreenSize);
            _renderer = renderer;
            _bitmap = new Bitmap(device,_refractionTexture.ShaderResourceView, (int)renderer.ScreenSize.X, (int)renderer.ScreenSize.Y,100, 100, 0);
            _bitmap.Position = new Vector2((int)renderer.ScreenSize.X-100, 0);
            _bitmap2 = new Bitmap(device, _reflectionTexture.ShaderResourceView, (int)renderer.ScreenSize.X, (int)renderer.ScreenSize.Y, 100, 100, 0);
            _bitmap2.Position = new Vector2((int)renderer.ScreenSize.X - 100, 120);
            _bumpMap = _renderer.TextureManager.Create("OceanWater.png");
            _skydome = new ObjModel(device, "skydome.obj", renderer.TextureManager.Create("Sky.png"));
            BuildBuffers(device);
            WaveTranslation = new Vector2(0,0);
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
            VertexDefinition.PositionTextureNormal4Weights[] terrainVertices = new VertexDefinition.PositionTextureNormal4Weights[(_width+1)*(_height+1)];
            for (int i = 0; i < (_width + 1); i++)
                for (int j = 0; j < (_height + 1); j++)
                    terrainVertices[i * (_width + 1) + j] = new VertexDefinition.PositionTextureNormal4Weights
                    {
                        position = new Vector3((-(_width / 2) + i) * _pitch, GetHeight(i, j), (-(_height / 2) + j) * _pitch),
                        texture = new Vector2(((float)i / TerrainTextureRepeat), ((float)j / TerrainTextureRepeat)),
                        normal = GetNormal(i,j),
                        weights = GetWeights(GetHeight(i,j))
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

            VertexDefinition.PositionTexture[] waterVertices = new VertexDefinition.PositionTexture[(_width + 1) * (_height + 1)];
            for (int i = 0; i < (_width + 1); i++)
                for (int j = 0; j < (_height + 1); j++)
                    waterVertices[i * (_width + 1) + j] = new VertexDefinition.PositionTexture
                    {
                        position = new Vector3((-(_width / 2) + i) * _pitch, GetHeight(i, j), (-(_height / 2) + j) * _pitch),
                        texture = new Vector2(((float)i / WaveTextureRepeat), ((float)j / WaveTextureRepeat)),
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

        public void Update(float delta)
        {
            WaveTranslation += new Vector2(0.1f, 0.15f)*delta;
        }

        public void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, Light light)
        {
            Light newLight = new Light(light.Direction, light.Color, new Vector4(1), light.SpecularPower, light.SpecularColor);
            _skydome.Render(deviceContext);
            _renderer.LightShader.Render(deviceContext, _skydome.IndexCount, Matrix.Scaling(10000) * Matrix.RotationY(MathUtil.PiOverTwo-WaveTranslation.X/8), viewMatrix, projectionMatrix, _skydome.Texture, newLight, _renderer.Camera);

            deviceContext.ClearDepthStencilView(_renderer.DirectX.RenderToTextureDepthStencilView, DepthStencilClearFlags.Depth, 1, 0);
            _refractionTexture.SetRenderTarget(deviceContext, _renderer.DirectX.RenderToTextureDepthStencilView);
            _refractionTexture.ClearRenderTarget(deviceContext, _renderer.DirectX.RenderToTextureDepthStencilView, 0.0f, 0.0f, 0.0f, 1.0f);
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(TerrainVertexBuffer, Utilities.SizeOf<VertexDefinition.PositionTextureNormal4Weights>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(TerrainIndexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            TerrainShader.Render(deviceContext, TerrainIndexCount, worldMatrix, viewMatrix, projectionMatrix, light, _terrainTextures, _refractionClippingPlane);

            deviceContext.ClearDepthStencilView(_renderer.DirectX.RenderToTextureDepthStencilView, DepthStencilClearFlags.Depth, 1, 0);
            _reflectionTexture.SetRenderTarget(deviceContext, _renderer.DirectX.RenderToTextureDepthStencilView);
            _reflectionTexture.ClearRenderTarget(deviceContext, _renderer.DirectX.RenderToTextureDepthStencilView, 0.0f, 0.0f, 0.0f, 1.0f);
            _skydome.Render(deviceContext);
            _renderer.LightShader.Render(deviceContext, _skydome.IndexCount, Matrix.Scaling(10000) * Matrix.RotationY(MathUtil.PiOverTwo - WaveTranslation.X/8), viewMatrix, projectionMatrix, _skydome.Texture, newLight, _renderer.Camera);
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(TerrainVertexBuffer, Utilities.SizeOf<VertexDefinition.PositionTextureNormal4Weights>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(TerrainIndexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            TerrainShader.Render(deviceContext, TerrainIndexCount, worldMatrix, _renderer.Camera.ReflectionMatrix, projectionMatrix, light, _terrainTextures, _reflectionClippingPlane);
            _renderer.DirectX.SetBackBufferAsRenderTarget();
            //Render water
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(WaterVertexBuffer, Utilities.SizeOf<VertexDefinition.PositionTexture>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(WaterIndexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            WaterShader.Render(deviceContext, WaterIndexCount, worldMatrix, viewMatrix, projectionMatrix, _renderer.Camera.ReflectionMatrix, _reflectionTexture.ShaderResourceView, _refractionTexture.ShaderResourceView, _bumpMap.TextureResource, WaveTranslation, _renderer.Camera.Position);
            //Render terrain
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(TerrainVertexBuffer, Utilities.SizeOf<VertexDefinition.PositionTextureNormal4Weights>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(TerrainIndexBuffer, Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            TerrainShader.Render(deviceContext, TerrainIndexCount, worldMatrix, viewMatrix, projectionMatrix, light, _terrainTextures, _noClippingPlane);

            _bitmap.Render(deviceContext);
            _renderer.TextureShader.Render(deviceContext, _bitmap.IndexCount, Matrix.Identity, _renderer.Camera.UiMatrix, _renderer.DirectX.OrthoMatrix, _refractionTexture.ShaderResourceView);
            _bitmap2.Render(deviceContext);
            _renderer.TextureShader.Render(deviceContext, _bitmap.IndexCount, Matrix.Identity, _renderer.Camera.UiMatrix, _renderer.DirectX.OrthoMatrix, _reflectionTexture.ShaderResourceView);
        }
    }
}
