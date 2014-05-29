using System.Windows.Input;
using Alpha.Graphics;
using Alpha.UI.Coordinates;
using SharpDX;

namespace Alpha.UI.Controls.Custom
{
    class CalendarWidget : Panel
    {
        private ICalendar _calendar;
        private bool _wasPaused;
        private Icon _icon;
        private Label _label;
        public CalendarWidget(IGame game) 
            : base(game, "calendar", new UniRectangle(new UniScalar(1.0f, -270), 20, 250, 40), Color.MediumOrchid)
        { }

        public override void Initialize()
        {
            base.Initialize();
            _calendar = Game.Services.GetService<ICalendar>();
            IRenderer renderer = Game.Services.GetService<IRenderer>();

            Button dateButton = Register(new Button(Game, "calendar_date", new UniRectangle(0.05f, 0.1f, 0.82f, 0.8f), _calendar + "   "));
            dateButton.Clicked += (b) => _calendar.Paused = !_calendar.Paused;
            _calendar.DayChanged += () => { dateButton.Text = _calendar.ToString(); }; 
            dateButton.Shortcut = Key.Space;

            _icon = dateButton.Register(new Icon(Game, "calendar_date_pause_icon", new UniRectangle(new UniScalar(1.0f, -24), new UniScalar(0.5f, -10), 20, 20),
                renderer.TextureManager.Create("PauseIcon.png", "Data/UI/Icons/")));

            _label = dateButton.Register(new Label(Game, "calendar_multiplier_label",
                new UniRectangle(new UniScalar(1.0f, -150), new UniScalar(0.5f, -15), 120, 30), "x1"));
            _label.Overlay = true;

            Button upButton =
                Register(new Button(Game, "calendar_speed_up",
                    new UniRectangle(new UniScalar(1.0f, -20), 0.05f, 20, 0.4f), "+"));
            upButton.Clicked += (b) => _calendar.IncreaseMultiplier();
            upButton.Shortcut = Key.Up;

            Button downButton =
                Register(new Button(Game, "calendar_speed_down",
                    new UniRectangle(new UniScalar(1.0f, -20), 0.55f, 20, 0.4f), "-"));
            downButton.Clicked += (b) => _calendar.DecreaseMultiplier();
            downButton.Shortcut = Key.Down;
        }

        protected override void Update(double delta)
        {
            _icon.Visible = _calendar.Paused;
            _label.Text = "x" + _calendar.Multiplier;
        }

        protected override void Desactivate()
        {
            _wasPaused = _calendar.Pause();
        }

        protected override void Activate()
        {
            if (!_wasPaused)
                _calendar.Unpause();
        }
    }
}
