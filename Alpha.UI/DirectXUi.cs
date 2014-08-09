using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Alpha.Common;
using SharpDX.Windows;

namespace Alpha.UI
{
    public class DirectXUi : IUi
    {
        private readonly Toolkit.Timer _timer;

        private RenderForm _form;
        private Dx11 _directX;
        private readonly IGame _game;

        public DirectXUi(IGame game)
        {
            _timer = new Toolkit.Timer();
            _game = game;
        }

        private void Update(double delta)
        {
            Console.WriteLine("UI Update begin");
            Thread.Sleep((int)(delta+100));
            Console.WriteLine("UI Update end");
        }

        private void Draw()
        {
            Console.WriteLine("UI Draw begin");
            _directX.BeginScene(0.75f, 0.75f, 0.75f, 1f);
            Thread.Sleep(50);
            _directX.DrawScene();
            Console.WriteLine("UI Draw end");
        }

        public void StartRenderLoop(object dataLock)
        {
            CreateWindow();
            _directX = new Dx11(_form);
            RenderLoop.Run(_form, () =>
            {
                lock (dataLock)
                    Update(_timer.Tick());
                Draw();
            });
            _game.Exit();
        }

        private void CreateWindow()
        {
            _form = new RenderForm(ConfigurationManager.Config.Title)
            {
                ClientSize = new Size(ConfigurationManager.Config.Width, ConfigurationManager.Config.Height),
                FormBorderStyle = FormBorderStyle.FixedSingle
            };
            _form.Show();
            _form.Name = "Alpha";
        }
    }
}
