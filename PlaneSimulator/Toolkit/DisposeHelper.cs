using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator.Toolkit
{
    static class DisposeHelper
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
