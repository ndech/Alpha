using System;
using System.Drawing;
using System.Windows.Forms;
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

        public ColorShader ColorShader { get; set; }

        public TextureShader TextureShader { get; set; }

        public LightShader LightShader { get; set; }

        public float Rotation { get; private set; }

        public Renderer()
        {
            CreateWindow();
            DirectX = new Dx11();
            DirectX.AcquireGpu(out _videoCardMemorySize, out _videoCardName);
            DirectX.CreateDeviceAndSwapChain(Form);
            DirectX.InitializeBuffers();
            DirectX.CreateMatrices();
            Camera = new Camera(new Vector3(0, 0, -10), Vector3.Zero);
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
            Rotation = 0;
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
            DirectX.BeginScene(0.5f, 0.5f, 0.5f, 1f);

            Rotation += (float)(MathUtil.PiOverFour*delta);
            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            Model.Render(DirectX.Device.ImmediateContext);

            // Render the model using the color shader.
            LightShader.Render(DirectX.DeviceContext, Model.IndexCount, DirectX.WorldMatrix * Matrix.RotationY(Rotation), Camera.ViewMatrix, DirectX.ProjectionMatrix, Model.Texture, Light, Camera);
            
            DirectX.DrawScene();
        }

        public void Dispose()
        {
            DirectX.Dispose();
        }
    }
}
