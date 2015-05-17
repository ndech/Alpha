using System;
using System.Diagnostics;

namespace Alpha.Toolkit
{
    public static class DebugConsole
    {
        [Conditional("DEBUG")]
        public static void WriteLine(string value)
        {
            Console.WriteLine(value);
        }
    }
}
