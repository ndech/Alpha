using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Alpha.Core.Save
{
    class SaveGame
    {
        private static string DirectoryPath
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Alpha\Saves\";
                Directory.CreateDirectory(path);
                return path;
            }
        }

        public SaveGame(string fileName, params ISavable[] items)
        {
            XElement save = new XElement("save", new XAttribute("version", "1.0.0"), items.Select(item=>item.Save()));
            save.Save(Path.Combine(DirectoryPath, fileName));
        }
    }
}
