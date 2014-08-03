using System;

namespace Alpha.UI
{
    public interface IUi
    {
        void Update(double delta);
        void Draw();
        void StartRenderLoop(Object dataLock);
    }
}
