namespace PlaneSimulator
{
    using Toolkit;
    using System;
    using SharpDX;
    using SharpDX.Direct3D11;
    using Graphics;
    using Rectangle = Graphics.Rectangle;
    class MonitoringHeader : RenderableGameComponent
    {
        private readonly CpuUsageCounter _cpuUsageCounter;
        private readonly FpsCounter _fpsCounter;
        private readonly Text _text;
        private readonly String _videoCardInfo;
        private readonly Rectangle _overlay;
        
        public MonitoringHeader(Game game, Renderer renderer) : base(game, 0, false, true)
        {
            _cpuUsageCounter = new CpuUsageCounter();
            _fpsCounter = new FpsCounter();
            _text = renderer.TextManager.Create("Courrier", 14, 80, new Vector4(1,1,1,0.5f));
            _text.Position = new Vector2(3,0);
            _videoCardInfo = renderer.DirectX.VideoCardName + " ("+renderer.DirectX.VideoCardMemorySize+" MB)";
            _overlay = new Rectangle(renderer, renderer.ScreenSize, new Vector2(0,0), new Vector2(485,16), new Vector4(1,0,0,0.2f));
        }

        public override void Update(double delta)
        {
            _cpuUsageCounter.Update(delta);
            _fpsCounter.Update(delta);
        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _text.Content = ((int)_fpsCounter.Value) + " FPS | " + String.Format("{0:0.00}", _cpuUsageCounter.Value) + " % CPU | " + _videoCardInfo;
            _text.Update(deviceContext);
            _overlay.Size = new Vector2( _text.Width+2, 16);
            _overlay.Render(deviceContext, Matrix.Identity, viewMatrix, projectionMatrix);
            _text.Render(deviceContext, Matrix.Identity, viewMatrix, projectionMatrix);
        }

        public override void Dispose()
        {
            _cpuUsageCounter.Dispose();
            _overlay.Dispose();
        }
    }
}
