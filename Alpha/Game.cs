using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Events;
using Alpha.Toolkit;
using Alpha.Graphics;
using Alpha.UI;
using Alpha.UI.Screens;
using SharpDX.Windows;

namespace Alpha
{
    class Game : IGame
    {
        public ServiceContainer Services { get; private set; }
        private readonly List<GameComponent> _gameComponents;

        private readonly Timer _timer;
        private readonly Renderer _renderer;
        private readonly UiManager _uiManager;
        private readonly StartUpScreen _startUpScreen;

        public Game()
        {
            Services = new ServiceContainer();

            _gameComponents = new List<GameComponent>();
            _timer = new Timer();
            _renderer = new Renderer(this);
            _uiManager = new UiManager(this);
            Calendar calendar = new Calendar(this);
            MousePointer pointer = new MousePointer(this);

            Register(_renderer,
                     calendar,
                     new Input(this),
                     new Camera(this),
                     new World(this),
                     new FleetManager(this),
                     _uiManager,
                     new ProvinceManager(this),
                     new CharacterManager(this),
                     new EventManager(this),
                     new RealmManager(this),
                     pointer);
            
            foreach (IService service in _gameComponents.OfType<IService>())
                service.RegisterAsService();

            pointer.Type = MousePointer.CursorType.None;
            foreach (GameComponent component in _gameComponents.Where(c=> c.RequiredForStartUp))
                component.Initialize(InitializingFeedback);
            _uiManager.AddScreen(_startUpScreen = new StartUpScreen(this));
            InitializingFeedback("Loading content.");
            foreach (GameComponent component in _gameComponents.Where(c => !c.RequiredForStartUp))
                component.Initialize(InitializingFeedback);
            _uiManager.DeleteScreen(_startUpScreen);
            _uiManager.AddScreen(new GameScreen(this));
            pointer.Type = MousePointer.CursorType.Default;
            calendar.DayChanged += OnDayChanged;
            calendar.MonthChanged += OnMonthChanged;
            calendar.YearChanged += OnYearChanged;
        }

        private void InitializingFeedback(String loadedItem)
        {
            _startUpScreen.Text = loadedItem;
            Draw(_uiManager);
        }

        private void Draw(params RenderableGameComponent[] items)
        {
            _renderer.Render(items.ToList());
        }
        public void Draw()
        {
            _renderer.Render();
        }

        private void Register(params GameComponent[] items)
        {
            foreach (GameComponent item in items)
            {
                _gameComponents.Add(item);
                if (item is RenderableGameComponent)
                    _renderer.Register(item as RenderableGameComponent);
            }
            _gameComponents.Sort();
        }

        public void Run()
        {
            RenderLoop.Run(_renderer.Form, () =>
            {
                double delta = _timer.Tick();
                
                foreach (GameComponent item in _gameComponents)
                    if(item.Enabled)
                        item.Update(delta);
                Draw();
            });
        }

        private void OnDayChanged()
        {
            foreach (IDailyUpdatable component in _gameComponents.OfType<IDailyUpdatable>())
                component.DayUpdate();
        }

        private void OnMonthChanged()
        {}

        private void OnYearChanged()
        {}

        public void Exit()
        {
            _renderer.Form.Close();
            Dispose();
        }

        public void Save(String fileName)
        {
            SaveGame.Create(fileName + ".xml", _gameComponents.OfType<ISavable>());
        }

        public void Load(String fileName, Action<String> feedback)
        {
            SaveGame.Load(fileName + ".xml", Services, _gameComponents.OfType<ISavable>(), feedback);
        }

        public void Dispose()
        {
            _renderer.Dispose();
            foreach (GameComponent item in _gameComponents)
                item.Dispose();
        }
    }
}
