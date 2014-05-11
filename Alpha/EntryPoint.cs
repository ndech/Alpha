using System;

namespace Alpha
{
    using System.Globalization;
    using System.Threading;
    internal static class EntryPoint
    {
        [STAThread]
        public static void Main()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Game game = new Game();
            game.Run();
            game.Dispose();
        }
    }
}