namespace Alpha.Core.Fleets
{
    public class Ship
    {
        internal Ship()
        {
            
        }
        public ShipModel Model { get; internal set; }
        public float Speed { get { return Model.MaxSpeed; } }
    }
}
