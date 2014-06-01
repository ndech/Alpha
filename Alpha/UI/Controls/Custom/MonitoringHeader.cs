using System;
using Alpha.Graphics;
using Alpha.Toolkit;
using Alpha.UI.Coordinates;
using SharpDX;

namespace Alpha.UI.Controls.Custom
{
    class MonitoringHeader : Panel
    {
        private readonly CpuUsageCounter _cpuUsageCounter;
        private readonly FpsCounter _fpsCounter;
        private String _videoCardInfo;
        private Label _label;

        public MonitoringHeader(IGame game)
            : base(game, "monitoring_header", new UniRectangle(0, 0, 1.0f, 25), new Color(0.7f, 0.7f, 0.7f, 0.9f))
        {
            _cpuUsageCounter = new CpuUsageCounter();
            _fpsCounter = new FpsCounter();
        }

        public override void Initialize()
        {
            base.Initialize();
            IRenderer renderer = Game.Services.GetService<IRenderer>();
            _videoCardInfo = renderer.VideoCardName + " (" + renderer.VideoCardMemorySize + " MB)";
            _label = Register(new Label(Game, "monitoring_header_label", Coordinates, ""));
        }

        protected override void Update(double delta)
        {
            if(!this.Visible) return;
            _cpuUsageCounter.Update(delta);
            _fpsCounter.Update(delta);
            _label.Text = ((int)_fpsCounter.Value) + " FPS | " + String.Format("{0:0.00}", _cpuUsageCounter.Value) + " % CPU | " + _videoCardInfo;
        }
    }
}
