using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha.Core.Tags
{
    static class ITagableExtension
    {
        public static void Tag(this ITagable tagable, Tag tag)
        {
            tagable.Tags.Add(tag);
        }

        public static bool HasTag(this ITagable tagable, Tag tag)
        {
            return tagable.Tags.Contains(tag);
        }

        public static bool PopTag(this ITagable tagable, Tag tag)
        {
            return tagable.Tags.Pop(tag);
        }
    }
}
