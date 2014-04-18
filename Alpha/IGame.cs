using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha
{
    public interface IGame
    {
        ServiceContainer Services { get; }
        void Exit();
    }
}
