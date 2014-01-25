using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PlaneSimulator.Toolkit;
using SharpDX;
using SharpDX.Windows;
using PlaneSimulator.Graphics.Shaders;

namespace PlaneSimulator.Graphics
{
    public class Renderer : IDisposable
    {
        public RenderForm Form { get; private set; }
        public Dx11 DirectX { get; private set; }
        public Camera Camera { get; set; }

        public Light Light { get; set; }

        public ObjModel Model { get; set; }

        public Bitmap Model2D { get; set; }

        public ColorShader ColorShader { get; set; }

        public TextureShader TextureShader { get; set; }

        public LightShader LightShader { get; set; }
        public TranslateShader TranslateShader { get; set; }
        public FontShader CircleShader { get; set; }

        public Vector2 ScreenSize { get; private set; }
        
        public TextManager TextManager { get; private set; }
        
        public Terrain Terrain { get; private set; }

        private readonly List<RenderableGameComponent> _renderables;

        public Renderer()
        {
            CreateWindow();
            DirectX = new Dx11(Form);
            ScreenSize = new Vector2(ConfigurationManager.Config.Width, ConfigurationManager.Config.Height);
            Light = new Light
            {
                Direction = new Vector3(1.0f, -1.0f, 0.0f),
                Color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                AmbiantColor = new Vector4(0.16f, 0.16f, 0.16f, 1.0f),
                SpecularPower = 32.0f,
                SpecularColor = new Vector4(1.0f, 1.0f, 0.7f, 1.0f)
            };
            ColorShader = new ColorShader(DirectX.Device);
            TextureShader = new TextureShader(DirectX.Device);
            LightShader = new LightShader(DirectX.Device);
            TranslateShader = new TranslateShader(DirectX.Device);
            CircleShader = new FontShader(DirectX.Device);
            Model2D = new Bitmap(DirectX.Device, "Circle.png", ConfigurationManager.Config.Width, ConfigurationManager.Config.Height, 100, 100)
            {
                Position = new Vector2(ConfigurationManager.Config.Width-100, 0)
            };
            TextManager = new TextManager(DirectX.Device, ConfigurationManager.Config.Width, ConfigurationManager.Config.Height);

            Terrain = new Terrain(DirectX.Device, "Heightmap.png", 100);
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
            DirectX.BeginScene(0.75f, 0.75f, 0.75f, 1f);
            Terrain.Render(DirectX.DeviceContext, DirectX.WorldMatrix, Camera.ViewMatrix, DirectX.ProjectionMatrix, Light);
            foreach (RenderableGameComponent item in _renderables)
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

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(DirectX, Model, Model2D);
            DirectX.Dispose();
        }

        public void Register(RenderableGameComponent item)
        {
            _renderables.Add(item);
        }
    }
}
