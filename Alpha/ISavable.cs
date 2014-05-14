using System;
using System.Xml;

namespace Alpha
{
    interface ISavable
    {
        int SaveOrder { get; }
        String SaveName { get; }

        void Save(XmlWriter writer);
        void PreLoading();
        void Load(SaveGame save);
        void PostLoading();
    }
}

