using PlaneSimulator.Toolkit.IO;

namespace PlaneSimulator
{
    using System;
    using System.Diagnostics;

    internal class EntryPoint
    {
        public static void Main()
        {
            Timer timer = new Timer();
            timer.Initialize();
            World world = new World();
            Airplane plane = new Airplane(world);
            plane.Tanks.Add(new Tank(100, 500));
            plane.Tanks.Add(new Tank(100, 500));
            plane.Thrusters.Add(new Thruster());
            plane.Initialize(1000, 200);
            Stopwatch stopWatch = new Stopwatch();
            CsvLogger flightRecorder = new CsvLogger(@"Logs\FlightRecording_" + DateTime.Now.ToString("yyyyMMddHHmmss") +".csv", 100, ';');
            flightRecorder.Register(timer, plane);
            stopWatch.Start();
            int i = 0;
            while (true)
            {
                i++;
                timer.Tick();
                //plane.Update(timer.Delta);
                plane.Update(0.00001);
                flightRecorder.Log();
                if (plane.IsCrashed())
                    break;
            }
            stopWatch.Stop();
            flightRecorder.Dispose();
            Console.WriteLine();
            Console.WriteLine("Flight recorded to : " + flightRecorder.FileName);
            Console.WriteLine("Number of integration steps : " + i);
            Console.WriteLine("Elapsed : " + stopWatch.ElapsedMilliseconds);
            Console.WriteLine("Your plane crashed");
            Console.ReadKey();
        }
    }
}