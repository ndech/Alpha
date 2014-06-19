using SharpDX;

namespace Alpha.Graphics
{
    interface ICamera : IService
    {
        Matrix UiMatrix { get; }
        Matrix ViewMatrix { get; }
        Vector3 Position { get; }
        Matrix ReflectionMatrix { get; }
        void Move(int x, int y);
    }
}
