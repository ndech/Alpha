using System;

namespace Alpha.Toolkit
{
    public static class DisposeHelper
    {
        public static void DisposeAndSetToNull(params IDisposable[] disposables)
        {
            for (int i = 0; i < disposables.GetLength(0); i++)
                if (disposables[i] != null)
                {
                    disposables[i].Dispose();
                    disposables[i] = null;
                }
        }
    }
}
