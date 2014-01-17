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
        public ObjModel Model2 { get; set; }

        public Bitmap Model2D { get; set; }

        public ColorShader ColorShader { get; set; }

        public TextureShader TextureShader { get; set; }

        public LightShader LightShader { get; set; }
        public TranslateShader TranslateShader { get; set; }
        public FontShader CircleShader { get; set; }

        public float Rotation { get; private set; }

        public TextManager TextManager { get; private set; }

        public Text cpuText { get; private set; }
        public Text fpsText { get; private set; }

        public Text altitudeText { get; private set; }

        public Text gpuText { get; private set; }

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
                Direction = new Vector3(1.0f, -1.0f, 0.0f),
                Color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                AmbiantColor = new Vector4(0.16f, 0.16f, 0.16f, 1.0f),
                SpecularPower = 32.0f,
                SpecularColor = new Vector4(1.0f, 1.0f, 0.7f, 1.0f)
            };
            Model = new ObjModel(DirectX.Device, "Airplane.obj", "Metal.png");
            Model2 = new ObjModel(DirectX.Device, "Airplane.obj", "Metal.png");
            ColorShader = new ColorShader(DirectX.Device);
            TextureShader = new TextureShader(DirectX.Device);
            LightShader = new LightShader(DirectX.Device);
            TranslateShader = new TranslateShader(DirectX.Device);
            CircleShader = new FontShader(DirectX.Device);
            Model2D = new Bitmap(DirectX.Device, "Circle.png", ConfigurationManager.Config.Width, ConfigurationManager.Config.Height, 100, 100)
            {
                Position = new Vector2(ConfigurationManager.Config.Width-100, 0)
            };
            Rotation = 0;
            TextManager = new TextManager(DirectX.Device, ConfigurationManager.Config.Width, ConfigurationManager.Config.Height);
            _cpuUsageCounter = cpuUsageCounter;
            _fpsCounter = fpsCounter;
            _airplane = airplane;
            cpuText = TextManager.Create("Arial", 20, 10, new Vector4(1, 1, 1, 1));
            cpuText.Position = new Vector2(10, 40);
            fpsText = TextManager.Create("Arial", 20, 10, new Vector4(1, 1, 1, 1));
            fpsText.Position = new Vector2(10, 70);
            altitudeText = TextManager.Create("Arial", 20, 25, new Vector4(1, 1, 1, 1));
            altitudeText.Position = new Vector2(10, 100);
            gpuText = TextManager.Create("Arial", 20, 50, new Vector4(1, 1, 1, 1));
            gpuText.Position = new Vector2(10, 10);
            Terrain = new Terrain(DirectX.Device, "Heightmap.png", 100);
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
            Camera.Position = new Vector3((float)_airplane.CurrentState.Position.Y, _airplane.Altitude+10, (float)_airplane.CurrentState.Position.X-6450);
            //Camera.Position = new Vector3(0,100,-_airplane.Altitude);
            
            i++;
            DirectX.BeginScene(0.75f, 0.75f, 0.75f, 1f);

            Rotation += (float)(MathUtil.PiOverFour*delta);

            //DirectX.EnableWireFrame();

            Matrix matrix = DirectX.WorldMatrix * Camera.ViewMatrix * DirectX.ProjectionMatrix;
            var vec = new Vector3(300 - (float)i/5, _airplane.Altitude, (float)_airplane.CurrentState.Position.X - 5090);
            var vector = Vector3.Project(vec, 0, 0, ConfigurationManager.Config.Width, ConfigurationManager.Config.Height, 0.0f, 1.0f,
                matrix);

            Terrain.Render(DirectX.DeviceContext, DirectX.WorldMatrix, Camera.ViewMatrix, DirectX.ProjectionMatrix, Light);

            //DirectX.DisableWireFrame();


            Model.Render(DirectX.DeviceContext);

            LightShader.Render(DirectX.DeviceContext, Model.IndexCount, DirectX.WorldMatrix * Matrix.RotationY(SharpDX.MathUtil.Pi) *
                //Matrix.RotationZ(Rotation/2) *
                Matrix.Translation(0, _airplane.Altitude, (float)_airplane.CurrentState.Position.X - 6390), Camera.ViewMatrix, DirectX.ProjectionMatrix, Model.Texture, Light, Camera);

            Model2.Render(DirectX.DeviceContext);

            LightShader.Render(DirectX.DeviceContext, Model.IndexCount, DirectX.WorldMatrix * Matrix.RotationY(SharpDX.MathUtil.Pi) *
                Matrix.RotationZ(-Rotation / 2) *
                Matrix.Translation(300 - (float)i / 5, _airplane.Altitude, (float)_airplane.CurrentState.Position.X - 5090), Camera.ViewMatrix, DirectX.ProjectionMatrix, Model.Texture, Light, Camera);



            DirectX.EnableAlphaBlending();

            Model2D.Position = new Vector2(vector.X - 20, vector.Y - 20);
            Model2D.Size = new Vector2(40,40);
            Model2D.Depth = 0;
            //DirectX.DisableZBuffer();
            Model2D.Render(DirectX.DeviceContext);
            CircleShader.Render(DirectX.DeviceContext, Model2D.IndexCount, DirectX.WorldMatrix, Camera.UiMatrix, DirectX.OrthoMatrix, Model2D.Texture, new Vector4(0.2f,0,0,0.5f));


            cpuText.Content = String.Format("CPU : {0:0.00}%", _cpuUsageCounter.Value);
            cpuText.Render(DirectX.DeviceContext, DirectX.WorldMatrix, Camera.UiMatrix, DirectX.OrthoMatrix);
            fpsText.Content = "FPS : " + (int)_fpsCounter.Value;
            fpsText.Render(DirectX.DeviceContext, DirectX.WorldMatrix, Camera.UiMatrix, DirectX.OrthoMatrix);
            altitudeText.Content = String.Format("Atitude : {0:0.0} m", _airplane.Altitude);
            altitudeText.Render(DirectX.DeviceContext, DirectX.WorldMatrix, Camera.UiMatrix, DirectX.OrthoMatrix);
            gpuText.Content = VideoCardName;
            gpuText.Render(DirectX.DeviceContext, DirectX.WorldMatrix, Camera.UiMatrix, DirectX.OrthoMatrix);

            DirectX.DisableAlphaBlending();

            //DirectX.EnableZBuffer();

            DirectX.DrawScene();
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(DirectX, Model, Model2D);
            DirectX.Dispose();
        }
    }
}
