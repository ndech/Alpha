using Alpha.Core.Calendars;
using Alpha.Core.Fleets;
using Alpha.Core.Provinces;
using Alpha.Core.Realms;

namespace Alpha.Core
{
    public interface IWorld
    {
        FleetManager FleetManager { get; }
        RealmManager RealmManager { get; }
        ProvinceManager ProvinceManager { get; }
        Calendar Calendar { get; }
    }
}
