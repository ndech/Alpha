using System;
using System.Drawing;
using System.Windows.Forms;
using PlaneSimulator.Toolkit;
using SharpDX;
using SharpDX.Windows;
using PlaneSimulator.Graphics.Shaders;
using MathUtil = PlaneSimulator.Toolkit.Math.MathUtil;

namespace PlaneSimulator.Graphics
{
    class Renderer : IDisposable
    {
        public RenderForm Form { get; private set; }
        private readonly int _videoCardMemorySize;
        private readonly String _videoCardName;
        public int VideoCardMemorySize { get { return _videoCardMemorySize; } }
        public String VideoCardName { get { return _videoCardName; } }
        public Dx11 DirectX { get; private set; }
        public Camera Camera { get; set; }

        public Light Light { get; set; }

        public ObjModel Model { get; set; }

        public Bitmap Model2D { get; set; }

        public ColorShader ColorShader { get; set; }

        public TextureShader TextureShader { get; set; }

        public LightShader LightShader { get; set; }
        public TranslateShader TranslateShader { get; set; }

        public float Rotation { get; private set; }

        public TextManager TextManager { get; private set; }

        public Text cpuText { get; private set; }
        public Text fpsText { get; private set; }

        public Text altitudeText { get; private set; }

        private CpuUsageCounter _cpuUsageCounter;
        private FpsCounter _fpsCounter;
        private Airplane _airplane;
        public Terrain Terrain { get; private set; }
        private int i;

        public Renderer(CpuUsageCounter cpuUsageCounter, FpsCounter fpsCounter,  Airplane airplane)
        {
            CreateWindow();
            DirectX = new Dx11();
            DirectX.AcquireGpu(out _videoCardMemorySize, out _videoCardName);
            DirectX.CreateDeviceAndSwapChain(Form);
            DirectX.InitializeBuffers();
            DirectX.CreateMatrices();
            Camera = new Camera(new Vector3(0, 0, -10), new Vector3(0,0,0));
            Light = new Light
            {
                Direction = new Vector3(1.0f, 0.0f, 0.0f),
                Color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                AmbiantColor = new Vector4(0.16f, 0.16f, 0.16f, 1.0f),
                SpecularPower = 32.0f,
                SpecularColor = new Vector4(1.0f, 1.0f, 0.7f, 1.0f)
            };
            Model = new ObjModel(DirectX.Device, "model.obj", "stone01.dds");
            ColorShader = new ColorShader(DirectX.Device);
            TextureShader = new TextureShader(DirectX.Device);
            LightShader = new LightShader(DirectX.Device);
            TranslateShader = new TranslateShader(DirectX.Device);
            Model2D = new Bitmap(DirectX.Device, "seafloor.dds", ConfigurationManager.Config.Width, ConfigurationManager.Config.Height, 256, 256)
            {
                Position = new Vector2(0, 0)
            };
            Rotation = 0;
            TextManager = new TextManager(DirectX.Device, ConfigurationManager.Config.Width, ConfigurationManager.Config.Height);
            _cpuUsageCounter = cpuUsageCounter;
            _fpsCounter = fpsCounter;
            _airplane = airplane;
            cpuText = TextManager.Create("Arial", 20, 10, new Vector4(1, 1, 1, 1));
            cpuText.Position = new Vector2(50, 50);
            fpsText = TextManager.Create("Arial", 20, 10, new Vector4(1, 1, 1, 1));
            fpsText.Position = new Vector2(50, 80);
            altitudeText = TextManager.Create("Arial", 20, 25, new Vector4(1, 1, 1, 1));
            altitudeText.Position = new Vector2(50, 110);
            Terrain = new Terrain(DirectX.Device, 200, 200, 100);
            i = 0;
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

        public void Render(double delta)
        {
            Camera.Position = new Vector3((float)_airplane.CurrentState.Position.Y, _airplane.Altitude, (float)_airplane.CurrentState.Position.X-3450);
            //Camera.Position = new Vector3(0,100,-_airplane.Altitude);
            
            i++;
            DirectX.BeginScene(0.5f, 0.5f, 0.5f, 1f);

            Rotation += (float)(MathUtil.PiOverFour*delta);
            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            Model.Render(DirectX.DeviceContext);
            // Render the model using the color shader.
            LightShader.Render(DirectX.DeviceContext, Model.IndexCount, DirectX.WorldMatrix * Matrix.RotationY(Rotation), Camera.ViewMatrix, DirectX.ProjectionMatrix, Model.Texture, Light, Camera);

            DirectX.EnableWireFrame();

            Terrain.Render(DirectX.DeviceContext);
            ColorShader.Render(DirectX.DeviceContext, Terrain.IndexCount, DirectX.WorldMatrix, Camera.ViewMatrix, DirectX.ProjectionMatrix);

            DirectX.DisableWireFrame();

            DirectX.DisableZBuffer();

            //Model2D.Render(DirectX.DeviceContext);

            //TranslateShader.Render(DirectX.DeviceContext, Model2D.IndexCount, DirectX.WorldMatrix, Camera.ViewMatrix, DirectX.OrthoMatrix, Model2D.Texture, new Vector2(((float)i) / 1000, ((float)i) / 1000));

            DirectX.EnableAlphaBlending();

            cpuText.Content = String.Format("CPU : {0:0.00}%", _cpuUsageCounter.Value);
            cpuText.Render(DirectX.DeviceContext, DirectX.WorldMatrix, Camera.UiMatrix, DirectX.OrthoMatrix);
            fpsText.Content = "FPS : " + (int)_fpsCounter.Value;
            fpsText.Render(DirectX.DeviceContext, DirectX.WorldMatrix, Camera.UiMatrix, DirectX.OrthoMatrix);
            altitudeText.Content = String.Format("Atitude : {0:0.0} m", _airplane.Altitude);
            altitudeText.Render(DirectX.DeviceContext, DirectX.WorldMatrix, Camera.UiMatrix, DirectX.OrthoMatrix);


            DirectX.DisableAlphaBlending();

            DirectX.EnableZBuffer();

            DirectX.DrawScene();
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(DirectX, Model, Model2D);
            DirectX.Dispose();
        }
    }
}
