using System.Collections.Generic;
using System.Linq;
using Alpha.Graphics;
using Alpha.UI;
using SharpDX.Windows;

namespace Alpha
{
    class Game : IGame
    {
        public ServiceContainer Services { get; private set; }
        private readonly List<GameComponent> _gameComponents;

        private readonly Timer _timer;
        private readonly Renderer _renderer;

        public Game()
        {
            _gameComponents = new List<GameComponent>();
            Services = new ServiceContainer();

            _timer = new Timer();

            _renderer = new Renderer(this);

            Register(_renderer,
                     new Input(this),
                     new Camera(this),
                     new Calendar(this),
                     new UiManager(this),
                     new MonitoringHeader(this),
                     new ProvinceList(this),
                     new CharacterList(this),
                     new MousePointer(this));
            
            foreach (IService service in _gameComponents.OfType<IService>())
                service.RegisterAsService();

            foreach (GameComponent component in _gameComponents)
                component.Initialize();
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

                _renderer.Render();
            });
        }

        private void OnDayChanged()
        {
        }

        private void OnMonthChanged()
        {
            
        }

        private void OnYearChanged()
        {
            Save();
        }

        public void Exit()
        {
            _renderer.Form.Close();
        }

        private void Save()
        {
            //SaveGame.Create(
            //    "Alpha_" + _calendar.Year + "-" + _calendar.Month + "-" + _calendar.Day + ".xml", 
            //    _gameComponents.OfType<ISavable>());
        }

        private void Load()
        {
            SaveGame.Load(SaveGame.ExistingSaves()[0], Services, _gameComponents.OfType<ISavable>());
        }

        public void Dispose()
        {
            _renderer.Dispose();
            foreach (GameComponent item in _gameComponents)
                item.Dispose();
        }
    }
}
