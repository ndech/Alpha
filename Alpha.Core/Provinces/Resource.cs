namespace Alpha.Core.Provinces
{
    public class Resource
    {
        public ResourceType Type { get; private set; }

        public Resource(ResourceType type)
        {
            Type = type;
        }
    }
}
