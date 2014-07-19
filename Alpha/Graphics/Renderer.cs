using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Alpha.Graphics.Shaders;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Windows;
using Device = SharpDX.Direct3D11.Device;

namespace Alpha.Graphics
{
    class Renderer : GameComponent, IRenderer
    {
        public RenderForm Form { get; private set; }
        public string VideoCardName { get { return DirectX.VideoCardName; } }
        public int VideoCardMemorySize { get { return DirectX.VideoCardMemorySize; } }
        public DepthStencilView RenderToTextureDepthStencilView { get { return DirectX.RenderToTextureDepthStencilView; } }
        public Matrix ProjectionMatrix { get { return DirectX.ProjectionMatrix; } }

        public void SetBackBufferAsRenderTarget()
        {
            DirectX.SetBackBufferAsRenderTarget();
        }

        public void EnableZBuffer()
        {
            DirectX.EnableZBuffer();
        }

        public Dx11 DirectX { get; private set; }
        protected ICamera Camera { get; set; }
        public ColorShader ColorShader { get; set; }
        public TextureShader TextureShader { get; set; }
        public LightShader LightShader { get; set; }
        public WaterShader WaterShader { get; set; }
        public TerrainShader TerrainShader { get; set; }
        public FontShader FontShader { get; set; }
        public TranslateShader TranslateShader { get; set; }
        public PathShader PathShader { get; set; }
        public FontShader CircleShader { get; set; }
        public Device Device { get { return DirectX.Device; } }
        public Vector2I ScreenSize { get; private set; }
        public TextManager TextManager { get; private set; }
        public TextureManager TextureManager { get; private set; }

        private readonly List<RenderableGameComponent> _renderables;

        public Renderer(IGame game)
            : base(game, 10000)
        {
            CreateWindow();
            DirectX = new Dx11(Form);
            ScreenSize = new Vector2I(ConfigurationManager.Config.Width, ConfigurationManager.Config.Height);
            
            ColorShader = new ColorShader(DirectX.Device);
            TextureShader = new TextureShader(DirectX.Device);
            LightShader = new LightShader(DirectX.Device);
            TranslateShader = new TranslateShader(DirectX.Device);
            CircleShader = new FontShader(DirectX.Device);
            TerrainShader = new TerrainShader(DirectX.Device);
            WaterShader = new WaterShader(DirectX.Device);
            FontShader = new FontShader(DirectX.Device);
            PathShader = new PathShader(DirectX.Device);
            TextManager = new TextManager(this);
            TextureManager = new TextureManager(DirectX.Device);
            _renderables = new List<RenderableGameComponent>();
        }

        private void CreateWindow()
        {
            Form = new RenderForm(ConfigurationManager.Config.Title)
            {
                ClientSize = new Size(ConfigurationManager.Config.Width, ConfigurationManager.Config.Height),
                FormBorderStyle = FormBorderStyle.FixedSingle
            };
            Form.Show();
        }

        public void Render()
        {
            Render(_renderables);
        }
        public void Render(List<RenderableGameComponent> items)
        {
            DirectX.BeginScene(0.75f, 0.75f, 0.75f, 1f);
            foreach (RenderableGameComponent item in items)
            {
                if(item.BlendingEnabled)
                    DirectX.EnableAlphaBlending();
                else
                    DirectX.DisableAlphaBlending();
                if(item.DisplayWireframe)
                    DirectX.EnableWireFrame();
                else
                    DirectX.DisableWireFrame();
                if (item.ZBufferEnabled)
                {
                    DirectX.EnableZBuffer();
                    item.Render(DirectX.DeviceContext, Camera.ViewMatrix, DirectX.ProjectionMatrix);
                }
                else
                {
                    DirectX.DisableZBuffer();
                    item.Render(DirectX.DeviceContext, Camera.UiMatrix, DirectX.OrthoMatrix);
                }
            }
            DirectX.DrawScene();
        }
        
        public override void Initialize(Action<string> feedback)
        {
            Camera = Game.Services.Get<ICamera>();
        }

        public override void Update(double delta)
        {

        }

        public override void Dispose()
        {
            DirectX.Dispose();
        }

        public void Register(RenderableGameComponent item)
        {
            _renderables.Add(item);
        }

        public void RegisterAsService()
        {
            Game.Services.Register<IRenderer>(this);
        }
    }
}
