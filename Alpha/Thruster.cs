using Alpha.Toolkit.Math;

namespace Alpha
{
    public class Thruster : Component
    {
        public override double Mass
        {
            get { return 1000; }
        }

        public double HourlyConsumption
        {
            get { return 100.0; }
        }

        public Vector3D Trust
        {
            get { return new Vector3D(100000,0,0); }
        }
    }
}