namespace Alpha.Core.Fleets
{
    public class Ship
    {
        internal Ship()
        {
            Model = new ShipModel();
        }
        public ShipModel Model { get; internal set; }
        public float Speed { get { return Model.MaxSpeed; } }
    }
}
