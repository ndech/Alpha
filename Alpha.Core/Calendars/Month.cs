using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Alpha.Core.Dynamic;

namespace Alpha.Core.Calendars
{
    public class Month : IEquatable<Month>, IComparable<Month>
    {
        public String Name { get; private set; }
        internal String Id { get; private set; }
        public int Position { get; private set; }
        internal Func<int, int> Length { get; private set; }
        public Month Next { get; private set; }

        private Month(string name, string id, int position, Func<int, int> length)
        {
            Name = name;
            Id = id;
            Position = position;
            Length = length;
        }

        private Month(XElement element, int index) 
            : this(
                element.Element("name").Value, 
                element.Attribute("id").Value, 
                index, 
                Engine.Execute<Func<int, int>>("(Year)=>"+element.Element("length").Value, Engine.NewSession))
        { }

        public bool Equals(Month other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Position == other.Position;
        }

        public bool Equals(int other)
        {
            return other == Position;
        }

        public static bool operator ==(Month month, int other)
        {
            return month != null && month.Equals(other);
        }

        public static bool operator !=(Month month, int other)
        {
            return !(month == other);
        }

        public int CompareTo(Month other)
        {
            return Position - other.Position;
        }

        public override string ToString()
        {
            return Name;
        }

        internal static List<Month> LoadMonths()
        {
            List<Month> months = XDocument.Load(@"Data\Calendar\months.xml").Descendants("month").Select((item, index) => new Month(item, index + 1)).ToList();
            for (int i = 0; i < months.Count - 1; i++)
                months[i].Next = months[i + 1];
            months.Last().Next = months.First();
            return months;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Month)obj);
        }

        public override int GetHashCode()
        {
            return Position;
        }
    }
}