namespace Alpha.WorldGeneration
{
    static class EntryPoint
    {
        static void Main()
        {
            const int width = 5000;
            const int height = 5000;
            const int pointCount = 2000;
            
            Generator.Create(width, height, pointCount, 1);
        }
    }
}
