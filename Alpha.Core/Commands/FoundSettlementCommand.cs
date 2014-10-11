using Alpha.Core.Provinces;

namespace Alpha.Core.Commands
{
    public class FoundSettlementCommand : Command
    {
        private readonly LandProvince _province;

        public FoundSettlementCommand(LandProvince province)
        {
            _province = province;
        }

        internal override void Execute()
        {
            _province.FoundSettlement().Population = 500;
        }
    }
}
