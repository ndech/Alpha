using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Alpha.Core.Events;
using Alpha.Core.Save;
using Alpha.Toolkit;

namespace Alpha.Core.Characters
{
    public class CharacterManager : Manager, ISavable
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
                _characters.Add(new Character(World, "Eddard", "Stark" + i, Sex.Male, World.Calendar.Date(02, 3, 2000)));
            for (int i = 0; i < 5; i++)
                _characters.Add(new Character(World, "Catelyn", "Stark " + i, Sex.Female, World.Calendar.Date(05, 3, 2004)));
        }

        internal override void DayUpdate(DataLock dataLock)
        {
            TryTriggerEvents(_events, LivingCharacters, dataLock);
        }

        public XElement Save()
        {
            return new XElement("characters", AllCharacters.Select(c=>c.Save()));
        }

        public void Load()
        {
            throw new System.NotImplementedException();
        }
    }
}
