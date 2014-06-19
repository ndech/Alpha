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
            Int32 margin = 3;
            width = 5000;
            height = 5000;
            pointCount = 3000;
            
            Generator generator = new Generator();
            generator.Create(width, height, pointCount, margin);
        }
    }
}
