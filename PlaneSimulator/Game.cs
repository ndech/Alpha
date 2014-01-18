using System;
using PlaneSimulator.Graphics;
using PlaneSimulator.Toolkit;
using PlaneSimulator.Toolkit.IO;
using SharpDX.Windows;

namespace PlaneSimulator
{
    class Game
    {
        private readonly CsvLogger _flightRecorder;
        private readonly Timer _timer;
        private readonly Airplane _playerPlane;
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
        }

        public void Run()
        {
            CpuUsageCounter cpuUsageCounter = new CpuUsageCounter();
            FpsCounter fpsCounter = new FpsCounter();
            Renderer renderer = new Renderer(cpuUsageCounter, fpsCounter, _playerPlane);
            RenderLoop.Run(renderer.Form, () =>
            {
                _timer.Tick();
                _playerPlane.Update(_timer.Delta);
                cpuUsageCounter.Update(_timer.Delta);
                fpsCounter.Update(_timer.Delta);
                _flightRecorder.Log();
                if (_playerPlane.IsCrashed())
                    renderer.Form.Close();
                renderer.Render();
            });
        }

        public void Dispose()
        {
            _flightRecorder.Dispose();
        }
    }
}
