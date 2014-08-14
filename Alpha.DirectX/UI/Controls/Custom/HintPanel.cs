using Alpha.DirectX.UI.Coordinates;
using SharpDX;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class HintPanel : Panel
    {
        private double _counter;
        private Label _label;

        public HintPanel(IContext context, UniRectangle coordinates)
            : base(context, "hint_panel", coordinates, Color.Crimson)
        { }

        public override void Initialize()
        {
            base.Initialize();
            _label = Register(new Label(Context, "hint_label", new UniRectangle(0, 0, 1.0f, 1.0f), ""));
            GenerateNewHint();
        }

        protected override void Update(double delta)
        {
            _counter += delta;
            if (_counter < 3.0) return;
            GenerateNewHint();
            _counter = 0.0;
        }

        private void GenerateNewHint()
        {
            _label.Text += "Lol";
        }
    }
}
