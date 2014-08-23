using Alpha.Core.Realms;

namespace Alpha.Core
{
    public class RealmToken
    {
        public Realm Realm { get; private set; }

        internal RealmToken(Realm realm)
        {
            Realm = realm;
        }

        public static implicit operator Realm(RealmToken token)
        {
            return token.Realm;
        }
    }
}