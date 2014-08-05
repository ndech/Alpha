namespace Alpha.Core.Fleets
{
    class Ship : IShip
    {
        public ShipModel Model { get; private set; }
        public float Speed { get; set; }
    }
}
