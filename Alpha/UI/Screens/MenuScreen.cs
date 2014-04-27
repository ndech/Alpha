﻿using Alpha.Toolkit.Math;
using Alpha.UI.Controls;
using Alpha.UI.Coordinates;
using SharpDX;

namespace Alpha.UI.Screens
{
    class MenuScreen : Screen
    {
        private readonly Button _debugButton;
        private readonly Button _calendarButton;
        private readonly Button _provincesButton;
        private readonly Button _exitButton;
        private readonly Button _cancelButton;
        private readonly Panel _panel;
        public MenuScreen(IGame game)
            :base(game, true)
        {
            Register(_panel = new Panel(game, new UniRectangle(new UniScalar(.5f, -150), new UniScalar(.5f, -172), 300, 345), Color.Black));
            _panel.Register(new Label(game, new UniRectangle(0.05f, 15, 0.9f, 40), "Menu"));
            _panel.Register(_calendarButton = new Button(game, new UniRectangle(0.05f, 70, 0.9f, 40), "Calendar"));
            _panel.Register(_provincesButton = new Button(game, new UniRectangle(0.05f, 125, 0.9f, 40), "Provinces"));
            _panel.Register(_debugButton = new Button(game, new UniRectangle(0.05f, 180, 0.9f, 40), "Debug"));
            _panel.Register(_exitButton = new Button(game, new UniRectangle(0.05f, 235, 0.9f, 40), "Exit to desktop"));
            _panel.Register(_cancelButton = new Button(game, new UniRectangle(0.25f, 290, 0.5f, 40), "Cancel"));

            _exitButton.Clicked += () => UiManager.AddScreen(new ExitConfirmationScreen(game));
            _cancelButton.Clicked += () => UiManager.DeleteScreen(this);
            /*Register(new Tooltip(game, _calendarButton, 1d, "Show the calendar"));
            Register(new Tooltip(game, _provincesButton, 1d, "Show the provinces details"));
            Register(new Tooltip(game, _exitButton, 1d, "Return to desktop"));
            Register(new Tooltip(game, _debugButton, 1d, "This button ain't [red]doing anything.[-]\nAt all.\nClicking on it will cost you [yellow] 20[-] [gold] anyway."));
            
            _provincesButton.Clicked +=
                () => UiManager.AddScreen(new ProvincesScreen(game));
            _exitButton.Clicked +=
                () => UiManager.AddScreen(new ExitConfirmationScreen(game));
            _calendarButton.Clicked +=
                () => UiManager.AddScreen(new CalendarScreen(game));*/
        }
    }
}
