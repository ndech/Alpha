using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Alpha.Graphics.Shaders;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Windows;

namespace Alpha.Graphics
{
    public class Renderer : IDisposable
    {
        public RenderForm Form { get; private set; }
        public Dx11 DirectX { get; private set; }
        public Camera Camera { get; set; }
        public Light Light { get; set; }
        public ColorShader ColorShader { get; set; }
        public TextureShader TextureShader { get; set; }
        public LightShader LightShader { get; set; }
        public FontShader FontShader { get; set; }
        public TranslateShader TranslateShader { get; set; }
        public FontShader CircleShader { get; set; }
        public Vector2I ScreenSize { get; private set; }
        public TextManager TextManager { get; private set; }
        public TextureManager TextureManager { get; private set; }

        private readonly List<RenderableGameComponent> _renderables;

        public Renderer()
        {
            CreateWindow();
            DirectX = new Dx11(Form);
            ScreenSize = new Vector2I(ConfigurationManager.Config.Width, ConfigurationManager.Config.Height);
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
            FontShader = new FontShader(DirectX.Device);
            TextManager = new TextManager(DirectX.Device, ConfigurationManager.Config.Width, ConfigurationManager.Config.Height);
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
            DirectX.BeginScene(0.75f, 0.75f, 0.75f, 1f);
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
            DirectX.Dispose();
        }

        public void Register(RenderableGameComponent item)
        {
            _renderables.Add(item);
        }
    }
}
