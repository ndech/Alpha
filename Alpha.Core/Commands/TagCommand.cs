using System;
using Alpha.Core.Tags;

namespace Alpha.Core.Commands
{
    public class TagCommand : Command
    {
        private readonly ITagable _tagable;
        private readonly string _tag;
        private readonly int? _duration;


        public TagCommand(ITagable tagable, String tag)
        {
            _tagable = tagable;
            _tag = tag;
        }
        public TagCommand(ITagable tagable, String tag, int duration)
        {
            _tagable = tagable;
            _tag = tag;
            _duration = duration;
        }
        internal override void Execute()
        {
            if(_duration== null)
                _tagable.Tag(_tag);
            else
                _tagable.Tag(_tag, _duration.Value);
        }
    }
}
