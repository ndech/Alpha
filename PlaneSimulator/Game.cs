using System;
using System.Collections.Generic;
using PlaneSimulator.Graphics;
using PlaneSimulator.Toolkit.IO;
using SharpDX.Windows;

namespace PlaneSimulator
{
    class Game
    {
        private readonly CsvLogger _flightRecorder;
        private readonly Timer _timer;
        private readonly Airplane _playerPlane;
        private readonly Renderer _renderer;
        private readonly List<IUpdatable> _updatables;
        private readonly World _world;

        public Game()
        {
            _updatables = new List<IUpdatable>();
            _timer = new Timer();

            _world = new World();
            _playerPlane = AirplaneFactory.Create(_world);
            _playerPlane.Initialize(1000, 200);

            _flightRecorder = new CsvLogger(@"Logs\FlightRecording_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv", 1, ';');
            _flightRecorder.Register(_timer, _playerPlane);

            _renderer = new Renderer(_playerPlane);

            Register(_playerPlane);
            Register(new MonitoringHeader(_renderer));
        }

        private void Register(IUpdatable item)
        {
            _updatables.Add(item);
            if (item is IRenderable)
                _renderer.Register(item as IRenderable);
        }

        public void Run()
        {
            RenderLoop.Run(_renderer.Form, () =>
            {
                double delta = _timer.Tick();
                foreach (IUpdatable item in _updatables)
                    item.Update(delta);
                if (_playerPlane.IsCrashed())
                    Exit();
                _renderer.Render();
                _flightRecorder.Log();
            });
        }

        private void Exit()
        {
            _renderer.Form.Close();
        }

        public void Dispose()
        {
            _renderer.Dispose();
            _flightRecorder.Dispose();
        }
    }
}
