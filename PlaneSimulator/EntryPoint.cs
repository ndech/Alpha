﻿using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using PlaneSimulator.Toolkit;
using PlaneSimulator.Toolkit.IO;
using SharpDX;
using SharpDX.Windows;
using PlaneSimulator.Graphics;

namespace PlaneSimulator
{
    using System;
    using System.Diagnostics;

    internal class EntryPoint
    {
        public static void Main()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Game game = new Game();
            game.Run();
            game.Dispose();
            Timer timer = new Timer();
            World world = new World();
            Airplane plane = new Airplane(world);
            plane.Tanks.Add(new Tank(100, 500));
            plane.Tanks.Add(new Tank(100, 500));
            plane.Thrusters.Add(new Thruster());
            plane.Initialize(1000, 200);
            Stopwatch stopWatch = new Stopwatch();
            CpuUsageCounter cpuUsageCounter = new CpuUsageCounter();
            FpsCounter fpsCounter = new FpsCounter();
            CsvLogger flightRecorder = new CsvLogger(@"Logs\FlightRecording_" + DateTime.Now.ToString("yyyyMMddHHmmss") +".csv", 1, ';');
            Renderer renderer = new Renderer(cpuUsageCounter, fpsCounter, plane);
            flightRecorder.Register(timer, plane);
            stopWatch.Start();
            RenderLoop.Run(renderer.Form, () =>
            {
                timer.Tick();
                plane.Update(timer.Delta);
                cpuUsageCounter.Update(timer.Delta);
                fpsCounter.Update(timer.Delta);
                flightRecorder.Log();
                if (plane.IsCrashed())
                    renderer.Form.Close();
                renderer.Render(timer.Delta);
            });
            stopWatch.Stop();
            flightRecorder.Dispose();
            Console.WriteLine();
            Console.WriteLine("Flight recorded to : " + flightRecorder.FileName);
            Console.WriteLine("Elapsed : " + stopWatch.ElapsedMilliseconds);
            Console.WriteLine("Your plane crashed");
        }
    }
}