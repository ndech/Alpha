using System;
using Alpha.Core.Tags;

namespace Alpha.Core.Commands
{
    public class RemoveTagCommand : Command
    {
        private readonly ITagable _tagable;
        private readonly string _tag;


        public RemoveTagCommand(ITagable tagable, string tag)
        {
            _tagable = tagable;
            _tag = tag;
        }

        internal override void Execute()
        {
            _tagable.PopTag(_tag);
        }
    }
}