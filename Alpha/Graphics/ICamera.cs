using SharpDX;

namespace Alpha.Graphics
{
    interface ICamera : IService
    {
        Matrix UiMatrix { get; }
        Matrix ViewMatrix { get; }
    }
}
