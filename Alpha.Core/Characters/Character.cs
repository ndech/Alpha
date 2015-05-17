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
        public bool IsAlive => DeathDate == null;
        public int Age => World.Calendar.AgeOf(BirthDate);
        public string NickName { get; internal set; }
        public bool HasNickName => NickName != null;
        public string FirstName { get; }
        public string LastName { get; }

        private IList<Character> _children = new List<Character>();
        public IEnumerable<Character> Children => _children;
        public IEnumerable<Character> Heirs => _children.Where(c => c.IsAlive);
        public Character Mother { get; }
        public Character Father { get;  }
        public TagCollection Tags { get; } = new TagCollection();

        public Sex Gender { get; }
        public bool IsMale => Gender == Sex.Male;
        public bool IsFemale => Gender == Sex.Female;

        internal Character(World world, string firstName, string lastName, Sex gender, Date birthDate) : base(world)
        {
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
            Gender = gender;
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
