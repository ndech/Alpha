using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator
{
    class EntryPoint
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
            plane.Initialize(10000, 200);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while(true)
            {
                timer.Tick();
                plane.Update(timer.Delta);
                Console.Clear();
                Console.WriteLine(timer.ToString());
                Console.Write(plane.ToString());
                if (plane.IsCrashed())
                    break;
            }
            stopWatch.Stop();
            Console.WriteLine();
            Console.WriteLine("Elapsed : " + stopWatch.ElapsedMilliseconds);
            Console.WriteLine("Your plane crashed");
            Console.ReadKey();
        }
    }
}
