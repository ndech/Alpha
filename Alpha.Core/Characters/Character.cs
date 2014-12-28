using System;
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
        public String NickName { get; private set; }
        public bool HasNickName { get { return NickName != null; } }
        public String FirstName { get; private set; }
        public String LastName { get; private set; }

        internal Character(World world, String firstName, String lastName, Date birthDate) : base(world)
        {
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
        }

        public Realm ResponsibleRealm { get { throw new NotImplementedException(); } }
    }
}
