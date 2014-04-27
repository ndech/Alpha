using Alpha.Toolkit.Math;
using Alpha.UI.Controls;
using Alpha.UI.Coordinates;

namespace Alpha.UI.Screens
{
    class GameScreen : Screen
    {
        private ICalendar _calendar;

        public GameScreen(IGame game) : base(game)
        {
            _calendar = game.Services.GetService<ICalendar>();
            Label label;
            Register(label = new Label(game, new UniRectangle(new UniScalar(1.0f, -200), 0, 200, 100 ), _calendar.ToString()+"   "));
            _calendar.DayChanged += () =>
            {
                label.Text = _calendar.ToString();
            };
        }
    }
}
