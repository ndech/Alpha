using PlaneSimulator.Toolkit.Math;

namespace PlaneSimulator
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

        public Vector3 Trust
        {
            get { return new Vector3(100000,0,0); }
        }
    }
}