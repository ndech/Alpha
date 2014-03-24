using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Alpha
{
    class SaveGame
    {
        public static String DirectoryPath
        {
            get
            {
                String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Alpha\Saves\";
                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static void Create(String fileName, IEnumerable<ISavable> items)
        {
            using (XmlWriter writer = XmlWriter.Create(fileName))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Save");
                foreach (ISavable item in items.OrderBy(s => s.SaveOrder))
                {
                    writer.WriteStartElement(item.SaveName);
                    item.Save(writer);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public static List<String> ExistingSaves()
        {
            return Directory.GetFiles(DirectoryPath, "*.xml").ToList();
        }

        public static void Load(string fileName, IEnumerable<ISavable> items)
        {
            using (XmlReader reader = XmlReader.Create(fileName))
            {
                reader.ReadStartElement("Save");
                while (true)
                {
                    if (reader.Name == "") // remove whitespaces and carriage returns
                        reader.Read();
                    else if (reader.Name == "Save")
                        break;
                    else
                    {
                        String currentItemName = reader.Name;
                        reader.Read();
                        items.First(s => s.SaveName == currentItemName).Load(reader);
                        reader.Read();
                    }
                }
            }
        }

        public static void LoadCollection<T>(XmlReader reader, ICollection<T> collection, Func<XElement, T> factory, String tag)
        {
            collection.Clear();
            while (true)
            {
                if (reader.Name == "") // remove whitespaces and carriage returns
                    reader.Read();
                else if (reader.Name == tag)
                    collection.Add(factory((XElement)XNode.ReadFrom(reader)));
                else
                    break;
            }
        }
    }
}
