using System;
using Alpha.Toolkit.Math;

namespace Alpha.UI.Coordinates
{
    struct UniVector : IEquatable<UniVector>, IEquatable<Object>
    {
        public UniScalar X { get; set; }
        public UniScalar Y { get; set; }

        public static readonly UniVector Zero = new UniVector();
        public UniVector(UniScalar x, UniScalar y) : this()
        {
          X = x;
          Y = y;
        }

        public Vector2I ToOffset(Vector2I parentSize)
        {
            return new Vector2I(X.ToOffset(parentSize.X), Y.ToOffset(parentSize.Y));
        }

        public static UniVector operator +(UniVector first, UniVector second)
        {
            return new UniVector(first.X + second.X, first.Y + second.Y);
        }

        public static UniVector operator -(UniVector first, UniVector second)
        {
            return new UniVector(first.X - second.X, first.Y - second.Y);
        }

        public static bool operator ==(UniVector first, UniVector second) 
        {
            return first.Equals(second);
        }

        public static bool operator !=(UniVector first, UniVector second)
        {
            return !first.Equals(second);
        }

        public bool Equals(UniVector other)
        {
            return (X == other.X) && (Y == other.Y);
        }
        public override bool Equals(object other) 
        {
            if (ReferenceEquals(null, other)) return false;
            return other is UniVector && Equals((UniVector) other);
        }
        public override int GetHashCode() 
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override string ToString() 
        { 
            return string.Format("{{X:{0}, Y:{1}}}", X, Y);
        }
    }
}