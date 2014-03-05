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
        public String NickName { get; set; }

        public String FullName
        {
            get { return FirstName + (NickName != null ? " '" + NickName + "' " : " ") + LastName; }
        }
        public Sex Sex { get; set; }

        public Character()
        {
            Id = "character_" + _counter++;
            FirstName = "TestFirst";
            LastName = "TestSecond";
            Sex = Sex.Male;
        }

        private Character(String id, String firstName, String lastName, String sex, String nickName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            NickName = nickName;
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
            if(NickName != null)
                writer.WriteElementString("NickName", NickName);

            writer.WriteEndElement();
        }

        public override string ToString()
        {
            return Id+": "+ FullName + " (" + Sex + ")";
        }

        public static Character FromXml(XElement element)
        {
            return new Character(
                (string)element.Attribute("id"),
                (string)element.Element("FirstName"),
                (string)element.Element("LastName"),
                (string)element.Element("Sex"),
                (string)element.Element("NickName"));
        }
    }
}
