﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha
{
    class InvalidEventDataException : Exception
    {
        public InvalidEventDataException(string message)
            : base(message)
        {
        }
    }
}
