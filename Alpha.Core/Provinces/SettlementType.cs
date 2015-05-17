using System;

namespace Alpha.Core.Provinces
{
    public class SettlementType
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        public SettlementType(string id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
