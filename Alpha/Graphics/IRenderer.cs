﻿using System;
using Alpha.Graphics.Shaders;
using Alpha.Toolkit.Math;
using Device = SharpDX.Direct3D11.Device;
using SharpDX.Windows;

namespace Alpha.Graphics
{
    interface IRenderer : IService
    {
        RenderForm Form { get; }
        TextManager TextManager { get; }
        ColorShader ColorShader { get; }
        Device Device { get; }
        Vector2I ScreenSize { get; }
        String VideoCardName { get; }
        Int32 VideoCardMemorySize { get; }
    }
}
