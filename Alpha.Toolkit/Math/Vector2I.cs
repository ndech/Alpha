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

        public static Vector2I Zero = new Vector2I(0,0);

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
        public static Vector2I operator /(Vector2I v1, int i2)
        {
            return new Vector2I(v1.X / i2, v1.Y / i2);
        }
        public static Vector2I operator +(Vector2I v1, Vector2I v2)
        {
            return
                (
                    new Vector2I
                        (
                        v1.X + v2.X,
                        v1.Y + v2.Y
                        )
                    );
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

        public override string ToString()
        {
            return "X : " + X + ", Y:" + Y;
        }

        public static double Distance(Vector2I a, Vector2I b)
        {
            return System.Math.Sqrt(System.Math.Pow((a.X - b.X), 2)+System.Math.Pow((a.Y - b.Y), 2));
        }
    }
}
