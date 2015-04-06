using Alpha.Core.Provinces;
namespace Alpha.Core.Buildings
{
    class ResourceRequirement
    {
        public ResourceRequirement(ResourceType resourceType, int targetValue)
        {
            ResourceType = resourceType;
            TargetValue = targetValue;
            CurrentValue = 0;
        }

        public ResourceType ResourceType { get; private set; }
        public int TargetValue { get; private set; }
        public double CurrentValue { get; private set; }
        public bool IsFullfilled { get { return TargetValue >= CurrentValue; } }
    }
}
