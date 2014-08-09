namespace Alpha.EntryPoint
{
    public static class EntryPoint
    {
        public static void Main()
        {
            new ApiLogger();
            new Game().Run();
        }
    }
}
