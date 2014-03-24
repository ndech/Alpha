using System;
using System.Xml;

namespace Alpha
{
    interface ISavable
    {
        int SaveOrder { get; }
        String SaveName { get; }

        void Save(XmlWriter writer);
        void Load(XmlReader reader);
    }
}
