using System;
using Alpha.Toolkit;

namespace Alpha.DirectX
{
    public interface IUi : IDisposable
    {
        void StartRenderLoop(DataLock dataLock);
    }
}