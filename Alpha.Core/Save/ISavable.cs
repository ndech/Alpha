using System.Xml.Linq;

namespace Alpha.Core.Save
{
    interface ISavable
    {
        XElement Save();
        void Load();
    }
}
