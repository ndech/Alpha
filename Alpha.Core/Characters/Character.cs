using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Alpha.Core.Calendars;
using Alpha.Core.Events;
using Alpha.Core.Realms;
using Alpha.Core.Tags;
using Alpha.Core.Save;

namespace Alpha.Core.Characters
{
    public enum Sex
    {
        Male,
        Female
    }
    public class Character : Component, IEventable, ITagable, ISavable
    {
        public Date BirthDate { get; }
        public Date DeathDate { get; private set; }
        public bool IsAlive { get { return DeathDate == null; } }
        public int Age { get { return World.Calendar.AgeOf(BirthDate); } }
        public string NickName { get; internal set; }
        public bool HasNickName { get { return NickName != null; } }
        public string FirstName { get; }
        public string LastName { get; }

        private IList<Character> _children = new List<Character>();
        public IEnumerable<Character> Children { get { return _children; } }
        public IEnumerable<Character> Heirs { get { return _children.Where(c => c.IsAlive); } }
        public Character Mother { get; private set; }
        public Character Father { get; private set; }
        public TagCollection Tags { get; set; }

        public Sex Gender { get; }
        public bool IsMale { get { return Gender == Sex.Male; } }
        public bool IsFemale { get { return Gender == Sex.Female; } }

        internal Character(World world, string firstName, string lastName, Sex gender, Date birthDate) : base(world)
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

        public XElement Save()
        {
            return new XElement("character",
                new XElement("firstName", FirstName),
                new XElement("lastName", LastName),
                new XElement("nickName", NickName),
                new XElement("gender", Gender),
                Tags.Save()
                );
        }

        public void Load()
        {
            throw new NotImplementedException();
        }
    }
}
