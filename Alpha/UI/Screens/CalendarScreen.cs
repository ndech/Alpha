using Alpha.Toolkit.Math;
using Alpha.UI.Controls;
using Alpha.UI.Coordinates;

namespace Alpha.UI.Screens
{
    class CalendarScreen : Screen
    {
        private ICalendar _calendar;

        public CalendarScreen(IGame game) : base(game)
        {
            _calendar = game.Services.GetService<ICalendar>();
            Label label;
            Register(label = new Label(game, new Vector2I(400, 50), new Vector2I(200, 200), _calendar.ToString()+"   "));
            _calendar.DayChanged += () =>
            {
                label.Text = _calendar.ToString();
            };
        }
    }
}
