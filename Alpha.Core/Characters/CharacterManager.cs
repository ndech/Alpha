using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Events;
using Alpha.Toolkit;

namespace Alpha.Core.Characters
{
    public class CharacterManager : Manager
    {
        private readonly List<Character> _characters = new List<Character>();
        private readonly List<IEvent<Character>> _events;
        public IEnumerable<Character> AllCharacters { get { return _characters; } }
        public IEnumerable<Character> LivingCharacters { get { return _characters.Where(c=>c.IsAlive); } }

        public CharacterManager(World world) : base(world)
        {
            _events = World.EventManager.LoadEvents<Character>();
        }

        internal override void Initialize()
        {
            for(int i = 0; i < 5; i++)
                _characters.Add(new Character(World, "Eddard", "Stark", Sex.Male, World.Calendar.Date(02, 3, 2000)));
            for (int i = 0; i < 5; i++)
                _characters.Add(new Character(World, "Catelyn", "Stark "+i, Sex.Female, World.Calendar.Date(05, 3, 2004)));
        }

        internal override void DayUpdate(DataLock dataLock)
        {
            TryTriggerEvents(_events, LivingCharacters, dataLock);
        }
    }
}
