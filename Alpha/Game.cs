using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Events;
using Alpha.Graphics;
using Alpha.UI;
using Alpha.UI.Controls.Custom;
using SharpDX.Windows;

namespace Alpha
{
    class Game : IGame
    {
        public ServiceContainer Services { get; private set; }
        private readonly List<GameComponent> _gameComponents;

        private readonly Timer _timer;
        private readonly Renderer _renderer;
        private readonly Calendar _calendar;

        public Game()
        {
            _gameComponents = new List<GameComponent>();
            Services = new ServiceContainer();

            _timer = new Timer();
            _calendar = new Calendar(this);
            _renderer = new Renderer(this);

            Register(_renderer,
                     _calendar,
                     new Input(this),
                     new Camera(this),
                     new UiManager(this),
                     new ProvinceManager(this),
                     new CharacterManager(this),
                     new EventManager(this),
                     new RealmManager(this),
                     new MousePointer(this));
            
            foreach (IService service in _gameComponents.OfType<IService>())
                service.RegisterAsService();

            foreach (GameComponent component in _gameComponents)
                component.Initialize();
            _calendar.DayChanged += OnDayChanged;
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

        public void Draw()
        {
            _renderer.Render();
        }
        private void OnMonthChanged()
        {
            
        }

        private void OnYearChanged()
        {
        }

        public void Exit()
        {
            _renderer.Form.Close();
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
