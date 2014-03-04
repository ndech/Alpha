using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Alpha
{
    public enum Sex
    {
        Male,
        Female
    }
    class Character
    {
        private static int _counter = 0;
        public String Id { get; private set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public Sex Sex { get; set; }

        public Character()
        {
            Id = "character_" + _counter++;
            FirstName = "TestFirst";
            LastName = "TestSecond";
            Sex = Sex.Male;
        }

        private Character(String id, String firstName, String lastName, String sex)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Sex tempSex;
            Sex.TryParse(sex, out tempSex);
            Sex = tempSex;
        }

        public void Save(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Character");
            writer.WriteAttributeString("id", Id);

            writer.WriteElementString("FirstName", FirstName);
            writer.WriteElementString("LastName", LastName);
            writer.WriteElementString("Sex", Sex.ToString());

            writer.WriteEndElement();
        }

        public override string ToString()
        {
            return Id+": "+FirstName + " " + LastName + " (" + Sex + ")";
        }

        public static Character FromXml(XElement element)
        {
            return new Character(element.Attribute("id").Value,
                element.Element("FirstName").Value,
                element.Element("LastName").Value,
                element.Element("Sex").Value);
        }
    }
}
