using System;

namespace Alpha.Common
{
    public interface IWorldGenerator
    {
        IProcessableWorld Generate(Action<String> feedback);
    }
}
