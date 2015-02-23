using System;

namespace Alpha.Core.Provinces
{
    public class SettlementType
    {
        public String Id { get; private set; }
        public String Name { get; private set; }
        public String Description { get; private set; }

        public SettlementType(String id, String name, String description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
