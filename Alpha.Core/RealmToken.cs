using Alpha.Core.Realms;

namespace Alpha.Core
{
    public class RealmToken
    {
        public Realm Realm { get; }

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