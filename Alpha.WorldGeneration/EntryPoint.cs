using System;

namespace Alpha.WorldGeneration
{
    static class EntryPoint
    {
        static void Main()
        {
            Int32 width = 500;
            Int32 height = 500;
            Int32 pointCount = 5;
            Int32 margin = 15;
            width = 2000;
            height = 2000;
            pointCount = 2000;
            
            Generator generator = new Generator();
            generator.Create(width, height, pointCount, margin);
        }
    }
}
