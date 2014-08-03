using System;
using System.Collections.Generic;
using Alpha.Core.Commands;

namespace Alpha.AI
{
    public interface IAi
    {
        IList<ICommand> Process();
    }
}
