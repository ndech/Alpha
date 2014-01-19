using System;
using PlaneSimulator.Graphics;
using SharpDX;
using SharpDX.Direct3D11;
using Rectangle = PlaneSimulator.Graphics.Rectangle;

namespace PlaneSimulator
{
    using Toolkit;
    class MonitoringHeader : IUpdatable, IRenderable
    {
        private readonly CpuUsageCounter _cpuUsageCounter;
        private readonly FpsCounter _fpsCounter;
        private readonly Text _text;
        private readonly String _videoCardInfo;
        private readonly Rectangle _rectangle;

        public MonitoringHeader(Renderer renderer)
        {
            _cpuUsageCounter = new CpuUsageCounter();
            _fpsCounter = new FpsCounter();
            _text = renderer.TextManager.Create("Courrier", 14, 80, new Vector4(1,1,1,0.5f));
            _text.Position = new Vector2(3,0);
            _videoCardInfo = renderer.VideoCardName + " ("+renderer.VideoCardMemorySize+" MB)";
            _rectangle = new Rectangle();
        }

        public void Update(double delta)
        {
            _cpuUsageCounter.Update(delta);
            _fpsCounter.Update(delta);
        }
        
        public void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _text.Content = ((int)_fpsCounter.Value) + " FPS | " + String.Format("{0:0.00}", _cpuUsageCounter.Value) + " % CPU | " + _videoCardInfo;
            _text.Render(deviceContext, Matrix.Identity, viewMatrix, projectionMatrix);
        }
    }
}
