using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha.Toolkit
{
    static class TimeSpanParser
    {
        public static int Parse(String timeSpan)
        {
            int length = timeSpan.Length;
            int current = 0;
            int days = 0;
            for (int i = 0; i < length; ++i)
            {
                char c = timeSpan[i];

                if (char.IsDigit(c))
                    current = current * 10 + (int)char.GetNumericValue(c);
                else if (char.IsWhiteSpace(c))
                    continue;
                else
                {
                    int multiplier;
                    switch (c)
                    {
                        case 'd': multiplier = 1; break;
                        case 'w': multiplier = 7; break;
                        case 'm': multiplier = 30; break;
                        case 'y': multiplier = 365; break;
                        default:
                            throw new FormatException(
                                String.Format(
                                    "'{0}': Invalid duration character {1} at position {2}. Supported characters are s,m,h,d, and w", timeSpan, c, i));
                    }
                    days += current*multiplier;
                    current = 0;
                }
            }

            if (current != 0)
            {
                throw new FormatException(
                    String.Format("'{0}': missing duration specifier in the end of the string. Supported characters are s,m,h,d, and w", timeSpan));
            }
            return days;
        }
    }
}
