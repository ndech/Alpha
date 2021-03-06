﻿using SharpDX;

namespace Alpha.DirectX
{
    interface ICamera
    {
        Matrix UiMatrix { get; }
        Matrix ViewMatrix { get; }
        Vector3 Position { get; set; }
        void Move(int x, int y);
        void Rotate(int tick);
        void Zoom(int tick);
        void ForcePosition(Vector3 position);
    }
}
