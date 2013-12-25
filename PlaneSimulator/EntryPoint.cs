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
            stopWatch.Start();
            int i = 0;
            while (true)
            {
                i++;
                timer.Tick();
                //plane.Update(timer.Delta);
                plane.Update(0.00001);
                //Console.Clear();
                //Console.WriteLine(timer.ToString());
                //Console.Write(plane.ToString());
                if (plane.IsCrashed())
                    break;
            }
            stopWatch.Stop();
            Console.WriteLine();
            Console.WriteLine("Number of integration steps : " + i);
            Console.WriteLine("Elapsed : " + stopWatch.ElapsedMilliseconds);
            Console.WriteLine("Your plane crashed");
            Console.ReadKey();
        }
    }
}