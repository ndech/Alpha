namespace Alpha.WorldGeneration
{
    static class EntryPoint
    {
        static void Main()
        {
            const int width = 10000;
            const int height = 10000;
            const int pointCount = 10000;
            
            Generator.Create(width, height, pointCount, 1);
        }
    }
}
