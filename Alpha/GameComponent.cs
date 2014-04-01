using System;

namespace Alpha
{
    public abstract class GameComponent : IUpdatable, IDisposable, IComparable<GameComponent>
    {
        public bool Enabled { get; set; }
        public int UpdateOrder { get; protected set; }

        protected IGame Game;

        protected GameComponent(IGame game, int updateOrder = 0)
        {
            Game = game;
            UpdateOrder = updateOrder;
            Enabled = true;
        }

        public abstract void Initialize();
        public abstract void Update(double delta);
        public abstract void Dispose();
        public int CompareTo(GameComponent other)
        {
            return UpdateOrder- other.UpdateOrder;
        }
    }
}
