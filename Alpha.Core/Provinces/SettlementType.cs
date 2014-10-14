using System;
using System.Collections.Generic;

namespace Alpha.Core.Provinces
{
    public class SettlementType
    {
        public String Id { get; private set; }
        public String Name { get; private set; }
        public String Description { get; private set; }
        public double Cost { get; private set; }

        internal SettlementType(String id, String name, String description, int cost)
        {
            Id = id;
            Name = name;
            Description = description;
            Cost = cost;
        }
    }
}
