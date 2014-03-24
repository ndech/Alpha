using System.Collections.Generic;
using System.Linq;
using Alpha.Graphics;
using PlaneSimulator;
using SharpDX.DirectInput;
using SharpDX.Windows;

namespace Alpha
{
    public class Game
    {
        private readonly Timer _timer;
        private readonly Renderer _renderer;
        private readonly Calendar _calendar;
        public Input Input { get; private set; }
        private readonly List<GameComponent> _gameComponents;
        public ServiceContainer Services { get; private set; }
        private bool _newRegisteredElement = false;

        public Game()
        {
            _gameComponents = new List<GameComponent>();
            Services = new ServiceContainer();
            _calendar = new Calendar(this);
            _calendar.DayChanged += OnDayChanged;
            _calendar.MonthChanged += OnMonthChanged;
            _calendar.YearChanged += OnYearChanged;

            _timer = new Timer();
            _renderer = new Renderer();
            Input = new Input(this, _renderer.Form.Handle);
            Register(Input);
            Camera camera = new Camera(this); 
            Register(camera);
            _renderer.Camera = camera;
            Register(new MonitoringHeader(this, _renderer));
            Register(new ProvinceList(this));
            Register(new CharacterList(this));
        }

        public void Register(GameComponent item)
        {
            _newRegisteredElement = true;
            _gameComponents.Add(item);
            if (item is RenderableGameComponent)
                _renderer.Register(item as RenderableGameComponent);
        }

        public void Run()
        {
            RenderLoop.Run(_renderer.Form, () =>
            {
                double delta = _timer.Tick();
                if (_newRegisteredElement)
                {
                    _gameComponents.Sort();
                    _newRegisteredElement = false;
                }

                foreach (GameComponent item in _gameComponents)
                    if(item.Enabled)
                        item.Update(delta);
                
                _calendar.Update(delta);

                if (Input.IsKeyPressed(Key.Escape))
                    Exit();

                if (Input.IsKeyPressed(Key.S))
                    Save();

                if (Input.IsKeyPressed(Key.L))
                    Load();

                if (Input.IsKeyPressed(Key.P))
                    _calendar.Paused = !_calendar.Paused;

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

        private void Exit()
        {
            _renderer.Form.Close();
        }

        private void Save()
        {
            SaveGame.Create(
                "Alpha_" + _calendar.Year + "-" + _calendar.Month + "-" + _calendar.Day + ".xml", 
                _gameComponents.OfType<ISavable>());
        }

        private void Load()
        {
            SaveGame.Load(SaveGame.ExistingSaves()[0], _gameComponents.OfType<ISavable>());
        }


        public void Dispose()
        {
            _renderer.Dispose();
            foreach (GameComponent item in _gameComponents)
                item.Dispose();
        }
    }
}
