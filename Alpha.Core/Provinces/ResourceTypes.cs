using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace Alpha.Core.Provinces
{
    public class ResourceTypes
    {
        private static List<ResourceType> _resourceTypes;

        internal static void Initialize()
        {
            _resourceTypes = XDocument.Load(@"Data\Resources\Resources.xml")
                .Descendants("resource")
                .Select(x => new ResourceType(x))
                .ToList();
        }

        public static IEnumerable<ResourceType> Types
        {
            get
            {
                Debug.Assert(_resourceTypes != null, "Resource types not initialized");
                return _resourceTypes;
            }
        }
    }
}
