using System;

namespace PlaneSimulator
{
    public abstract class GameComponent : IUpdatable, IDisposable, IComparable<GameComponent>
    {
        public bool Enabled { get; set; }
        public int UpdateOrder { get; protected set; }

        protected GameComponent(Game game, int updateOrder = 0)
        {
            UpdateOrder = updateOrder;
            Enabled = true;
        }
        public abstract void Update(double delta);
        public abstract void Dispose();
        public int CompareTo(GameComponent other)
        {
            return UpdateOrder- other.UpdateOrder;
        }
    }
}
