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

        public ResourceType ResourceType { get; }
        public int TargetValue { get; }
        public double CurrentValue { get; internal set; }
        public bool IsFullfilled => TargetValue <= CurrentValue;
        public double RemainingValue => TargetValue - CurrentValue;
    }
}
