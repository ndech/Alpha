using System;

namespace Alpha.Toolkit.Math
{
    public struct Vector2I : IEquatable<Vector2I>
    {
        public int X;
        public int Y;

        public Vector2I(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Vector2I other)
        {
            return X == other.X && Y == other.Y;
        }

        public static bool operator ==(Vector2I v1, Vector2I v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(Vector2I v1, Vector2I v2)
        {
            return !v1.Equals(v2);
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            return other is Vector2I && Equals((Vector2I)other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
    }
}
