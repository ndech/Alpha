using System;
using System.Drawing;
using System.Windows.Forms;
using SharpDX.Windows;

namespace PlaneSimulator
{
    class Renderer
    {
        private RenderForm _form;
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
            DirectX.CreateDeviceAndSwapChain(_form);
            DirectX.InitializeBuffers();
        }

        private void CreateWindow()
        {
            _form = new RenderForm(ConfigurationManager.Config.Title)
            {
                ClientSize = new Size(ConfigurationManager.Config.Width, ConfigurationManager.Config.Height),
                FormBorderStyle = FormBorderStyle.FixedSingle
            };

            _form.Show();
        }
    }
}
