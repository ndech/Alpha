namespace Alpha.Core
{
    public class WorldContainer
    {
        private volatile World _world;
        public World World
        {
            get { return _world; }
            set { _world = value; }
        }

        private volatile RealmToken _playerRealm;
        public RealmToken PlayerRealm
        {
            get { return _playerRealm; }
            set { _playerRealm = value; }
        }

        public bool Ready { get { return _playerRealm != null && _world != null; } }
    }
}
