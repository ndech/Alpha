using System.Linq;

namespace Alpha.EntryPoint
{
    public static class EntryPoint
    {
        public static void Main()
        {
            Api.LogPublicElements();
            new Game().Run();
        }
    }
}
