using System;
using System.Xml.Linq;

namespace Alpha
{
    public interface IProvince
    {
        Int32 Population { get; set; }
        float YearlyGrowth { get; }
    }
    class Province : Territory, IUpdatable, IProvince
    {
        private static int _counter = 0;
        public String Id { get; private set; }
        public float YearlyGrowth { get; set; }

        public Province()
        {
            Id = "province_" + _counter++;
            _population = 1000;
            Name = "Province " + _counter;
            YearlyGrowth = 0.1f;
        }

        private Province(String id, String name, Int32 population)
        {
            Id = id;
            Name = name;
            _population = population;
        }

        public void Save(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Province");
            writer.WriteAttributeString("id", Id);
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("Population", Population.ToString());
            writer.WriteEndElement();
        }

        public override string ToString()
        {
            return Name +" "+ Population+ " pop";
        }

        public void Update(double delta)
        {
            _population++;
        }

        public static Province FromXml(XElement element, ServiceContainer services)
        {
            return new Province(
                (string)element.Attribute("id"),
                (string)element.Element("Name"),
                Int32.Parse((string)element.Element("Population")));
        }

        public void DayUpdate()
        {
            _population += _population*(YearlyGrowth/365);
        }
    }
}
