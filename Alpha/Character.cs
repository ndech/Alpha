using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        internal void Save(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Character");
            writer.WriteAttributeString("id", Id);

            writer.WriteElementString("FirstName", FirstName);
            writer.WriteElementString("LastName", LastName);
            writer.WriteElementString("Sex", Sex.ToString());

            writer.WriteEndElement();
        }
    }
}
