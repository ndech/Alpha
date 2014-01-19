using System;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using PlaneSimulator.Graphics;
using PlaneSimulator.Toolkit.IO;
using SharpDX.D3DCompiler;
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

        public Game()
        {
            _timer = new Timer();
            _flightRecorder = new CsvLogger(@"Logs\FlightRecording_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv", 1, ';');

            World world = new World();
            _playerPlane = new Airplane(world);
            _playerPlane.Tanks.Add(new Tank(100, 500));
            _playerPlane.Tanks.Add(new Tank(100, 500));
            _playerPlane.Thrusters.Add(new Thruster());
            _playerPlane.Initialize(1000, 200);

            _flightRecorder.Register(_timer, _playerPlane);

            _updatables = new List<IUpdatable>();

            _renderer = new Renderer(_playerPlane);

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
                _playerPlane.Update(delta);
                if (_playerPlane.IsCrashed())
                    Close();
                _renderer.Render();
                _flightRecorder.Log();
            });
        }

        private void Close()
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
