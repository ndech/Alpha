using System;
using System.Threading;

namespace Alpha.UI
{
    public class DirectXUi : IUi
    {
        private readonly Toolkit.Timer _timer;
        public DirectXUi()
        {
            _timer = new Toolkit.Timer();
        }
        public void Update(double delta)
        {
            Console.WriteLine("UI Update begin");
            Thread.Sleep(1000);
            Console.WriteLine("UI Update end");
        }

        public void Draw()
        {
            Console.WriteLine("UI Draw begin");
            Thread.Sleep(1000);
            Console.WriteLine("UI Draw end");
        }

        public void StartRenderLoop(object dataLock)
        {
            while (true)
            {
                lock (dataLock)
                    Update(_timer.Tick());
                Draw();
            }
        }
    }
}
