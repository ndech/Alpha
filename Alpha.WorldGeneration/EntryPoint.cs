namespace Alpha.WorldGeneration
{
    static class EntryPoint
    {
        static void Main()
        {
            const int width = 1000;
            const int height = 500;
            const int pointCount = 1000;
            
            Generator.Create(width, height, pointCount, 1);
        }
    }
}
