namespace Alpha.Core
{
    public class Component
    {
        protected World World;
        internal Component(World world)
        {
            World = world;
        }
    }
}