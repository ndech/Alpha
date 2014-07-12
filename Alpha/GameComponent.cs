using System;

namespace Alpha
{
    abstract class GameComponent : IUpdatable, IDisposable, IComparable<GameComponent>
    {
        public bool Enabled { get; set; }
        public int UpdateOrder { get; protected set; }
        public bool RequiredForStartUp { get; set; }

        protected IGame Game;

        protected GameComponent(IGame game, int updateOrder = 0, bool requiredForStartUp = true)
        {
            Game = game;
            UpdateOrder = updateOrder;
            RequiredForStartUp = requiredForStartUp;
            Enabled = true;
        }

        public abstract void Initialize(Action<String> feedback);
        public abstract void Update(double delta);
        public abstract void Dispose();
        public int CompareTo(GameComponent other)
        {
            return UpdateOrder - other.UpdateOrder;
        }
    }
}
