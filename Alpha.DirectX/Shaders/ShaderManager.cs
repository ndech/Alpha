using SharpDX.Direct3D11;

namespace Alpha.DirectX.Shaders
{
    class ShaderManager
    {
        public ColorShader ColorShader { get; private set; }
        public FontShader FontShader { get; private set; }
        public TextureShader TextureShader { get; private set; }
        public WaterShader WaterShader { get; private set; }
        public LightShader LightShader { get; private set; }
        public PathShader PathShader { get; private set; }
        public TerrainShader TerrainShader { get; private set; }
        public WorldTerrainShader WorldTerrainShader { get; private set; }
        public Texture1DShader Texture1DShader { get; private set; }

        public ShaderManager(Device device)
        {
            ColorShader = new ColorShader(device);
            FontShader = new FontShader(device);
            TextureShader = new TextureShader(device);
            WaterShader = new WaterShader(device);
            LightShader = new LightShader(device);
            PathShader = new PathShader(device);
            TerrainShader = new TerrainShader(device);
            WorldTerrainShader = new WorldTerrainShader(device);
            Texture1DShader = new Texture1DShader(device);
        }
    }
}
