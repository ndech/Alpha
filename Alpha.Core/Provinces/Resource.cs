namespace Alpha.Core.Provinces
{
    public class Resource
    {
        public ResourceLevel Level { get; private set; }
        public ResourceType Type { get; private set; }
        public Population Population { get; private set; }
        public int MaxPopulation { get { return 10000; } }

        public double Output
        {
            get { return Population.Value*0.01; }
        }

        public Resource(ResourceType type, ResourceLevel level)
        {
            Type = type;
            Level = level;
            Population = new Population();
        }
    }
}
