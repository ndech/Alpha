using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Calendars;
using Alpha.Core.Events;
using Alpha.Core.Realms;
using Alpha.Core.Tags;

namespace Alpha.Core.Characters
{
    public enum Sex
    {
        Male,
        Female
    }
    public class Character : Component, IEventable, ITagable
    {
        public Date BirthDate { get; private set; }
        public Date DeathDate { get; private set; }
        public bool IsAlive { get { return DeathDate == null; } }
        public int Age { get { return World.Calendar.AgeOf(BirthDate); } }
        public String NickName { get; internal set; }
        public bool HasNickName { get { return NickName != null; } }
        public String FirstName { get; private set; }
        public String LastName { get; private set; }

        private IList<Character> _children = new List<Character>();
        public IEnumerable<Character> Children { get { return _children; } }
        public IEnumerable<Character> Heirs { get { return _children.Where(c => c.IsAlive); } }
        public Character Mother { get; private set; }
        public Character Father { get; private set; }
        public TagCollection Tags { get; set; }

        public Sex Gender { get; private set; }
        public bool IsMale { get { return Gender == Sex.Male; } }
        public bool IsFemale { get { return Gender == Sex.Female; } }

        internal Character(World world, String firstName, String lastName, Sex gender, Date birthDate) : base(world)
        {
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
            Gender = gender;
            Tags = new TagCollection();
        }

        public Realm ResponsibleRealm { get { throw new NotImplementedException(); } }

        public override string ToString()
        {
            return FirstName + " " + (HasNickName ? "\"" + NickName + "\" " : "") + LastName;
        }
    }
}
