using System.Collections.Generic;
using Alpha.Core.Events;
using Alpha.Toolkit;

namespace Alpha.Core.Characters
{
    public class CharacterManager : Manager
    {
        private readonly List<Character> _characters = new List<Character>();
        private List<IEvent<Character>> _events;
        public IEnumerable<Character> Fleets { get { return _characters; } }
        public CharacterManager(World world) : base(world)
        { }

        internal override void Initialize()
        {
            _characters.Add(new Character(World, "Nicolas", "Dechamps", World.Calendar.Date(13,3,1991)));
        }

        internal override void DayUpdate(DataLock datalock)
        { }
    }
}
