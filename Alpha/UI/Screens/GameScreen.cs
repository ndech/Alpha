using Alpha.Graphics;
using Alpha.UI.Controls;
using Alpha.UI.Coordinates;

namespace Alpha.UI.Screens
{
    class GameScreen : Screen
    {
        private readonly ICalendar _calendar;
        private bool _wasPaused;
        private Icon _icon;
        public GameScreen(IGame game) : base(game)
        {
            _calendar = game.Services.GetService<ICalendar>();
            Button dateButton = Register(new Button(game, new UniRectangle(new UniScalar(1.0f, -200), 0, 200, 100 ), _calendar+"   "));
            _icon = dateButton.Register(new Icon(game, new UniRectangle(new UniScalar(1.0f, -25), new UniScalar(0.5f, -11), 20, 20),
                game.Services.GetService<IRenderer>().TextureManager.Create("PauseIcon.png", "Data/UI/Icons/")));
            Button menuButton = Register(new Button(game, new UniRectangle(new UniScalar(0.5f, -40), 0, 80, 30), "Menu"));
            menuButton.Clicked += () => UiManager.AddScreen(new MenuScreen(game));
            dateButton.Clicked += () => _calendar.Paused = ! _calendar.Paused;
            _calendar.DayChanged += () => { dateButton.Text = _calendar.ToString(); };
        }

        protected override void Update(double delta)
        {
            _icon.Visible = _calendar.Paused;
        }

        public override void Desactivate()
        {
            _wasPaused = _calendar.Pause();   
        }

        public override void Activate()
        {
            if (!_wasPaused)
                _calendar.Unpause();
        }
    }
}
