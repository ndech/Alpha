namespace Alpha.Core.Provinces
{
    public class Resource
    {
        public ResourceLevel Level { get; private set; }
        public ResourceType Type { get; private set; }
        public Population Population { get; private set; }

        public double Output
        {
            get { return Population.Value*0.01; }
        }

        public Resource(ResourceType type)
        {
            Type = type;
            Population = new Population();
        }
    }
}
