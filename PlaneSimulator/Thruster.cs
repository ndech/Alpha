namespace PlaneSimulator
{
    public class Thruster : Component
    {
        public override double Mass 
        { 
            get 
            { 
                return 1000; 
            } 
        }

        public double HourlyConsumption 
        { 
            get 
            { 
                return 100.0; 
            } 
        }
    }
}
