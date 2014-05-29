namespace Alpha
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    class SaveGame
    {
        private readonly String _fileName;
        public XmlReader Reader { get; private set; }
        private readonly ServiceContainer _services;

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
            using (XmlWriter writer = XmlWriter.Create(Path.Combine(DirectoryPath, fileName),
                new XmlWriterSettings {Indent = true}))
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

        public static void Load(string fileName, ServiceContainer services, IEnumerable<ISavable> items, Action<string> feedback)
        {
            SaveGame save = new SaveGame(fileName, services);
            save.Load(items, feedback);
        }

        private void Load(IEnumerable<ISavable> items, Action<string> feedback)
        {
            foreach (ISavable item in items)
                item.PreLoading();
            using (Reader = XmlReader.Create(Path.Combine(DirectoryPath, _fileName), 
                new XmlReaderSettings { IgnoreWhitespace = true }))
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
                        feedback.Invoke(currentItemName);
                        System.Threading.Thread.Sleep(200);
                        bool isEmpty = Reader.IsEmptyElement;
                        Reader.Read();
                        items.First(s => s.SaveName == currentItemName).Load(this);
                        if(!isEmpty)
                            Reader.Read();
                    }
                }
            }
            foreach (ISavable item in items)
                item.PostLoading();
        }

        public void LoadCollection<T>(ICollection<T> collection, Func<XElement, ServiceContainer, T> factory, String tag)
        {
            if(collection.Count > 0)
                throw new InvalidOperationException("Collection should be empty");
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
