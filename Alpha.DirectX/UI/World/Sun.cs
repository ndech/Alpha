﻿using SharpDX;

namespace Alpha.DirectX.UI.World
{
    class Sun : Light
    {
        public Sun()
        {
            Direction = new Vector3(1.0f, -1.0f, 0.0f);
            Color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            AmbiantColor = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            SpecularPower = 32.0f;
            SpecularColor = new Vector4(1.0f, 1.0f, 0.7f, 1.0f);
        }
    }
}
