using Alpha.Toolkit.Math;

namespace Alpha
{
    using Toolkit;
    using System;
    using SharpDX;
    using SharpDX.Direct3D11;
    using Graphics;

    class MonitoringHeader : RenderableGameComponent
    {
        private readonly CpuUsageCounter _cpuUsageCounter;
        private readonly FpsCounter _fpsCounter;
        private SimpleText _simpleText;
        private String _videoCardInfo;
        private PlainRectangle _overlay;
        
        public MonitoringHeader(IGame game) : base(game, 0, false, true)
        {
            _cpuUsageCounter = new CpuUsageCounter();
            _fpsCounter = new FpsCounter();
        }

        public override void Initialize()
        {
            IRenderer renderer = Game.Services.GetService<IRenderer>();
            _simpleText = renderer.TextManager.Create("Courrier", 14, 80, Color.Gray);
            _simpleText.Position = new Vector2I(3, 0);
            _videoCardInfo = renderer.VideoCardName + " (" + renderer.VideoCardMemorySize + " MB)";
            _overlay = new PlainRectangle(renderer, new Vector2I(0, 0), new Vector2I(485, 16), Color.MistyRose);
        }

        public override void Update(double delta)
        {
            _cpuUsageCounter.Update(delta);
            _fpsCounter.Update(delta);
            _simpleText.Content = ((int)_fpsCounter.Value) + " FPS | " + String.Format("{0:0.00}", _cpuUsageCounter.Value) + " % CPU | " + _videoCardInfo;
            _simpleText.Update();
            _overlay.Size = new Vector2I(_simpleText.Size.X + 2, 16);
        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _overlay.Render(deviceContext, Matrix.Identity, viewMatrix, projectionMatrix);
            _simpleText.Render(deviceContext, Matrix.Identity, viewMatrix, projectionMatrix);
        }

        public override void Dispose()
        {
            _cpuUsageCounter.Dispose();
            _overlay.Dispose();
        }
    }
}
