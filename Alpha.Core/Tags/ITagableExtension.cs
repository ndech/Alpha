using System;
using System.Linq;

namespace Alpha.Core.Tags
{
    public static class TagableExtension
    {
        public static void Tag(this ITagable tagable, Tag tag)
        {
            tagable.Tags.Add(tag);
        }

        public static void Tag(this ITagable tagable, String tag)
        {
            tagable.Tag(new BaseTag(tag));
        }

        public static void Tag(this ITagable tagable, String tag, int duration)
        {
            tagable.Tag(new TimerTag(tag, duration));
        }

        public static bool HasTag(this ITagable tagable, Tag tag)
        {
            return tagable.Tags.Contains(tag);
        }
        public static bool HasTag(this ITagable tagable, String tag)
        {
            return tagable.Tags.Any(t => t is BaseTag && ((BaseTag)t).Key == tag);
        }

        public static bool PopTag(this ITagable tagable, Tag tag)
        {
            return tagable.Tags.Pop(tag);
        }

        public static int PopTag(this ITagable tagable, String tag)
        {
            return tagable.Tags.PopWhere(t => t is BaseTag && ((BaseTag) t).Key == tag);
        }
    }
}
