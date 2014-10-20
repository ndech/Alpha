using System;
using Alpha.Core.Provinces;

namespace Alpha.Core.Commands
{
    public class FoundSettlementCommand : Command
    {
        private readonly LandProvince _province;
        private readonly BaseSettlementType _type;

        public FoundSettlementCommand(LandProvince province, BaseSettlementType type)
        {
            _province = province;
            _type = type;
        }

        internal override void Execute()
        {
            //_province.FoundSettlement(_type).Population = 500;
        }

        internal override bool IsValid()
        {
            return true; //_province.Owner.Economy.Treasury >= _type.Cost;
        }
    }
}
