using System;

namespace Alpha.Core.Events
{
    class InvalidEventDataException : Exception
    {
        public InvalidEventDataException(string message)
            : base(message)
        {
        }
    }
}
