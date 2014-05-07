using System;
using System.Linq;
using System.Xml.Linq;

namespace Alpha
{
    class Province : IUpdatable
    {
        private static int _counter = 0;
        public String Id { get; private set; }
        public Character Ruler { get; set; }
        public String Name { get; set; }
        public float YearlyGrowth { get; set; }
        private float _population;
        public Int32 Population { get { return (int)_population; } }
        
        public Province()
        {
            Id = "province_" + _counter++;
            _population = 1000;
            Name = "Province " + _counter;
            Ruler = null;
            YearlyGrowth = 0.1f;
        }

        private Province(String id, String name, Int32 population, Character ruler)
        {
            Id = id;
            Name = name;
            _population = population;
            Ruler = ruler;
        }

        public void Save(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Province");
            writer.WriteAttributeString("id", Id);
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("Population", Population.ToString());
            if(Ruler!=null) writer.WriteElementString("Ruler", Ruler.Id);
            writer.WriteEndElement();
        }

        public override string ToString()
        {
            return Name +" "+ Population+ " pop (" + (Ruler!=null ? Ruler.FullName : "No Ruler") + ")";
        }

        public void Update(double delta)
        {
            _population++;
        }

        public static Province FromXml(XElement element, ServiceContainer services)
        {
            String rulerId = (string) element.Element("Ruler");
            Character ruler = rulerId != null ? services.GetService<ICharacterList>().Characters.First(s => s.Id == rulerId) : null;
            return new Province(
                (string)element.Attribute("id"),
                (string)element.Element("Name"),
                Int32.Parse((string)element.Element("Population")),
                ruler);
        }

        public void DayUpdate()
        {
            _population += _population*(YearlyGrowth/365);
        }
    }
}
