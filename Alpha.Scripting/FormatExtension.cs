using System;

namespace Alpha.Scripting
{
    public static class FormatExtension
    {
        public static String Format(this double value)
        {
            return String.Format("{0:N2}", value);
        }
    }
}
