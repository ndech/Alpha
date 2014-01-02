﻿namespace PlaneSimulator.Toolkit.Math
{
    internal class MathUtil
    {
        public const float ZeroTolerance = 1e-6f;

        public const float Pi = 3.141592653589793239f;

        public const float TwoPi = 6.283185307179586477f;

        public const float PiOverTwo = 1.570796326794896619f;

        public const float PiOverFour = 0.785398163397448310f;

        public static bool NearEqual(double p1, double p2)
        {
            return p1 == p2;
        }
    }
}