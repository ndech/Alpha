namespace Alpha.Core.Provinces
{
    public class Resource : IDailyUpdatableItem
    {
        public ResourceLevel Level { get; private set; }
        public ResourceType Type { get; private set; }
        public Population Population { get; private set; }
        public int MaxPopulation { get { return 10000; } }
        public int StorageCapacity { get { return 1000; } }
        public double StorageLevel { get; set; }

        public double Output
        {
            get { return Population.Value*0.002; }
        }

        public Resource(ResourceType type, ResourceLevel level)
        {
            Type = type;
            Level = level;
            Population = new Population();
            StorageLevel = 20;
        }

        void IDailyUpdatableItem.DayUpdate()
        {
            StorageLevel += Output;
            Population.DayUpdate();
        }
    }
}
