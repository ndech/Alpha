using System;

namespace Alpha.Events
{
    class InvalidEventDataException : Exception
    {
        public InvalidEventDataException(string message)
            : base(message)
        {
        }
    }
}
