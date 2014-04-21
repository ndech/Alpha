using System;

namespace Alpha.UI.Coordinates
{
    struct UniScalar : IEquatable<UniScalar>, IEquatable<Object>
    {
        public static readonly UniScalar Zero = new UniScalar();
        public float Fraction { get; set; }
        public int Offset { get; set; }

        public UniScalar(float fraction, int offset = 0)
            : this()
        {
            Fraction = fraction;
            Offset = offset;
        }

        public UniScalar(int offset) : this(0.0f, offset) { }

        public static implicit operator UniScalar(float fraction)
        {
            return new UniScalar(fraction);
        }

        public static implicit operator UniScalar(int offset)
        {
            return new UniScalar(offset);
        }

        public static UniScalar operator +(UniScalar first, UniScalar second)
        {
            return new UniScalar(
              first.Fraction + second.Fraction,
              first.Offset + second.Offset
            );
        }

        public static UniScalar operator -(UniScalar first, UniScalar second)
        {
            return new UniScalar(
              first.Fraction - second.Fraction,
              first.Offset - second.Offset
            );
        }

        public static bool operator !=(UniScalar first, UniScalar second)
        {
            return !first.Equals(second);
        }

        public static bool operator ==(UniScalar first, UniScalar second)
        {
            return first.Equals(second);
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            return other is UniScalar && Equals((UniScalar)other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Fraction.GetHashCode() * 397) ^ Offset;
            }
        }

        public bool Equals(UniScalar other)
        {
            return Fraction.Equals(other.Fraction) && Offset == other.Offset;
        }

        public int ToOffset(int parentSize)
        {
            return (int)(Fraction * parentSize + Offset);
        }

        public override string ToString()
        {
            return string.Format("{{{0}% {1}{2}}}", Fraction * 100.0f, (Offset >= 0.0f) ? "+" : string.Empty, Offset);
        }
    }
}