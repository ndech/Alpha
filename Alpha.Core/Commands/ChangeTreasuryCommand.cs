using Alpha.Core.Realms;

namespace Alpha.Core.Commands
{
    public class ChangeTreasuryCommand : EventOnlyCommand
    {
        private readonly Realm _realm;
        private readonly float _value;

        public ChangeTreasuryCommand(Realm realm, float value)
        {
            _realm = realm;
            _value = value;
        }

        internal override void Execute()
        {
            _realm.Economy.Treasury += _value;
        }
    }
}
