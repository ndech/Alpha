using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Calendars;
using Alpha.Core.Events;
using Alpha.Core.Realms;

namespace Alpha.Core.Characters
{
    public class Character : Component, IEventable
    {
        public Date BirthDate { get; private set; }
        public bool IsAlive { get; private set; }
        public int Age { get { return World.Calendar.AgeOf(BirthDate); } }
        public String NickName { get; internal set; }
        public bool HasNickName { get { return NickName != null; } }
        public String FirstName { get; private set; }
        public String LastName { get; private set; }

        private IList<Character> _children = new List<Character>();
        public IEnumerable<Character> Children { get { return _children; } }
        public IEnumerable<Character> Heirs { get { return _children.Where(c => c.IsAlive); } } 

        internal Character(World world, String firstName, String lastName, Date birthDate) : base(world)
        {
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
        }

        public Realm ResponsibleRealm { get { throw new NotImplementedException(); } }

        public override string ToString()
        {
            return FirstName + " " + (HasNickName ? "\"" + NickName + "\" " : "") + LastName;
        }
    }
}
