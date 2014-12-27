using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Alpha.Core.Dynamic;

namespace Alpha.Core.Calendar
{
    public class Season : IEquatable<Season>
    {
        public String Id { get; private set; }
        public String Name { get; private set; }
        public int Order { get; private set; }
        internal Func<Calendar, bool> Trigger { get; private set; }

        public Season(String id, String name, int order, Func<Calendar, bool> trigger)
        {
            Id = id;
            Name = name;
            Order = order;
            Trigger = trigger;
        }

        private Season(XElement element, int index)
            : this(element.Attribute("id").Value, 
                   element.Element("name").Value,
                   index,
                   Engine.Execute<Func<Calendar, bool>>("(Calendar)=>" + element.Element("trigger").Value, Engine.NewSession))
        { }

        internal static IEnumerable<Season> LoadSeasons()
        {
            return XDocument.Load(@"Data\Calendar\seasons.xml").Descendants("season").Select((item, index) => new Season(item, index));
        }

        internal static Season CurrentSeason(IEnumerable<Season> seasons, Calendar calendar)
        {
            return seasons.OrderBy(s=>s.Order).First(s => s.Trigger(calendar));
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Season other)
        {
            return other.Id == Id;
        }
    }
}
