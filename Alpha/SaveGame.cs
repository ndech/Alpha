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
        private String _fileName;
        public XmlReader Reader { get; private set; }
        private ServiceContainer _services;

        private SaveGame(string fileName, ServiceContainer services)
        {
            _fileName = fileName;
            _services = services;
        }

        private static String DirectoryPath
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

        public static void Load(string fileName, ServiceContainer services, IEnumerable<ISavable> items)
        {
            SaveGame save = new SaveGame(fileName, services);
            save.Load(items);
        }

        private void Load(IEnumerable<ISavable> items)
        {
            using (Reader = XmlReader.Create(_fileName))
            {
                Reader.ReadStartElement("Save");
                while (true)
                {
                    if (Reader.Name == "") // remove whitespaces and carriage returns
                        Reader.Read();
                    else if (Reader.Name == "Save")
                        break;
                    else
                    {
                        String currentItemName = Reader.Name;
                        Reader.Read();
                        items.First(s => s.SaveName == currentItemName).Load(this);
                        Reader.Read();
                    }
                }
            }
        }

        public void LoadCollection<T>(ICollection<T> collection, Func<XElement, ServiceContainer, T> factory, String tag)
        {
            collection.Clear();
            while (true)
            {
                if (Reader.Name == "") // remove whitespaces and carriage returns
                    Reader.Read();
                else if (Reader.Name == tag)
                    collection.Add(factory((XElement)XNode.ReadFrom(Reader), _services));
                else
                    break;
            }
        }
    }
}
