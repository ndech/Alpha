using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Alpha
{
    class Province : IUpdatable
    {
        private static int _counter = 0;
        public String Id { get; private set; }
        public Character Ruler { get; set; }
        public String Name { get; set; }
        public Int32 Population { get; set; }
        
        public Province()
        {
            Id = "province_" + _counter++;
            Population = 1000;
            Name = "Province " + _counter;
            Ruler = null;
        }

        private Province(String id, String name, Int32 population, Character ruler)
        {
            Id = id;
            Name = name;
            Population = population;
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
            return Id + ": " + Name +" "+ Population+ " pop (" + (Ruler!=null ? Ruler.FullName : "No Ruler") + ")";
        }

        public void Update(double delta)
        {
            Population++;
        }

        public static Province FromXml(XElement element, IEnumerable<Character> characters )
        {
            String rulerId = (string) element.Element("Ruler");
            Character ruler = rulerId != null ? characters.First(s => s.Id == rulerId) : null;
            return new Province(
                (string)element.Attribute("id"),
                (string)element.Element("Name"),
                Int32.Parse((string)element.Element("Population")),
                ruler);
        }
    }
}
