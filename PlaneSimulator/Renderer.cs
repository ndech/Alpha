using System;
using System.Drawing;
using System.Windows.Forms;
using SharpDX.Windows;

namespace PlaneSimulator
{
    class Renderer : IDisposable
    {
        public RenderForm Form { get; private set; }
        private readonly int _videoCardMemorySize;
        private readonly String _videoCardName;
        public int VideoCardMemorySize { get { return _videoCardMemorySize; } }
        public String VideoCardName { get { return _videoCardName; } }

        public Dx11 DirectX { get; private set; }

        public Renderer()
        {
            CreateWindow();
            DirectX = new Dx11();
            DirectX.AcquireGpu(out _videoCardMemorySize, out _videoCardName);
            DirectX.CreateDeviceAndSwapChain(Form);
            DirectX.InitializeBuffers();
            DirectX.CreateMatrices();
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
            DirectX.BeginScene(0.5f, 0.5f, 0.5f, 1f);
            DirectX.DrawScene();
        }

        public void Dispose()
        {
            DirectX.Dispose();
        }
    }
}
