﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha
{
    interface IGame
    {
        ServiceContainer Services { get; }
        void Exit();
    }
}
