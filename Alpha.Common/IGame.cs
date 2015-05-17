using System;
using System.Threading;

namespace Alpha.Common
{
    public interface IGame
    {
        void Exit();
        AutoResetEvent GenerateWorldEvent { get; }
        string LoadingMessage { get; }
        bool IsWorldGenerationDone { get; }
    }
}
