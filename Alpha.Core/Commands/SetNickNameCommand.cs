using System;
using Alpha.Core.Characters;

namespace Alpha.Core.Commands
{
    public class SetNickNameCommand  : Command
    {
        private readonly Character _character;
        private readonly string _nickname;

        public SetNickNameCommand(Character character, String nickname)
        {
            _character = character;
            _nickname = nickname;
        }

        internal override void Execute()
        {
            _character.NickName = _nickname;
        }
    }
}
