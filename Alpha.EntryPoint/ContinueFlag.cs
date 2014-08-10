using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha.EntryPoint
{
    class ContinueFlag
    {
        private volatile bool _continue = true;
        public static implicit operator bool(ContinueFlag flag)
        {
            return flag._continue;
        }

        public void Stop()
        {
            _continue = false;
        }
    }
}
