using System;
using System.Globalization;
using SharpDX;
using SystemMath = System.Math;

namespace Alpha.Toolkit.Math
{
    public struct Vector3D : IEquatable<Vector3D>, IFormattable
    {
        public static readonly Vector3D Origin = new Vector3D(0, 0, 0);
        public double X;
        public double Y;
        public double Z;

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;                     
                    case 1: return Y;
                    case 2: return Z;
                }
                throw new ArgumentOutOfRangeException("index", "index must be less than three");
            }
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                }
                throw new ArgumentOutOfRangeException("index", "index must be less than three");
            }
        }

        public double Magnitude
        {
            get { return System.Math.Sqrt(SumOfSquaredComponents); }
        }

        public double SumOfSquaredComponents
        {
            get { return X*X + Y*Y + Z*Z; }
        }

        public Vector3D(double value)
        {
            X = value;
            Y = value;
            Z = value;
        }

        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return
                (
                    new Vector3D
                        (
                        v1.X + v2.X,
                        v1.Y + v2.Y,
                        v1.Z + v2.Z
                        )
                    );
        }

        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return
                (
                    new Vector3D
                        (
                        v1.X - v2.X,
                        v1.Y - v2.Y,
                        v1.Z - v2.Z
                        )
                    );
        }

        public static Vector3D operator -(Vector3D v1)
        {
            return
                (
                    new Vector3D
                        (
                        -v1.X,
                        -v1.Y,
                        -v1.Z
                        )
                    );
        }

        public static bool operator <(Vector3D v1, Vector3D v2)
        {
            return v1.Magnitude < v2.Magnitude;
        }

        public static bool operator <=(Vector3D v1, Vector3D v2)
        {
            return v1.Magnitude <= v2.Magnitude;
        }

        public static bool operator >(Vector3D v1, Vector3D v2)
        {
            return v1.Magnitude > v2.Magnitude;
        }

        public static bool operator >=(Vector3D v1, Vector3D v2)
        {
            return v1.Magnitude >= v2.Magnitude;
        }

        public static bool operator ==(Vector3D v1, Vector3D v2)
        {
            return (v1.X == v2.X) && (v1.Y == v2.Y) && (v1.Z == v2.Z);
        }

        public static bool operator !=(Vector3D v1, Vector3D v2)
        {
            return !(v1 == v2);
        }

        public static Vector3D operator /(Vector3D v1, double s2)
        {
            return new Vector3D(v1.X/s2, v1.Y/s2, v1.Z/s2);
        }

        public static Vector3D operator *(Vector3D v1, double s2)
        {
            return new Vector3D(v1.X*s2, v1.Y*s2, v1.Z*s2);
        }

        public static Vector3D operator *(double s2, Vector3D v1)
        {
            return v1*s2;
        }

        public static bool IsUnit(Vector3D v1)
        {
            return v1.Magnitude == 1;
        }

        public bool IsUnit()
        {
            return IsUnit(this);
        }

        public static Vector3D Cross(Vector3D left, Vector3D right)
        {
            return new Vector3D(
                (left.Y*right.Z) - (left.Z*right.Y),
                (left.Z*right.X) - (left.X*right.Z),
                (left.X*right.Y) - (left.Y*right.X));
        }

        public static double Dot(Vector3D left, Vector3D right)
        {
            return (left.X*right.X) + (left.Y*right.Y) + (left.Z*right.Z);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "[X:{0} Y:{1} Z:{2}]", X, Y, Z);
        }

        public string ToString(string format)
        {
            if (format == null)
                return ToString();

            return string.Format(format, CultureInfo.CurrentCulture, "[X:{0} Y:{1} Z:{2}]",
                X.ToString(format, CultureInfo.CurrentCulture), Y.ToString(format, CultureInfo.CurrentCulture),
                Z.ToString(format, CultureInfo.CurrentCulture));
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "[X:{0} Y:{1} Z:{2}]",
                X.ToString(formatProvider), Y.ToString(formatProvider), Z.ToString(formatProvider));
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString(formatProvider);

            return string.Format(format, formatProvider, "[X:{0} Y:{1} Z:{2}]",
                X.ToString(format, formatProvider), Y.ToString(format, formatProvider),
                Z.ToString(format, formatProvider));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((X.GetHashCode()*397) ^ Y.GetHashCode()*397) ^ Z.GetHashCode();
            }
        }

        public static bool Equals(Vector3D a, Vector3D b)
        {
            return
                MathUtil.NearEqual(a.X, b.X) &&
                MathUtil.NearEqual(a.Y, b.Y) &&
                MathUtil.NearEqual(a.Z, b.Z);
        }

        public bool Equals(Vector3D other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object value)
        {
            if (value == null)
                return false;
            return ReferenceEquals(value.GetType(), typeof (Vector3D)) && Equals((Vector3D) value);
        }

        public static explicit operator SharpDX.Vector3(Vector3D vector)
        {
            return new Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
        }

        public static double Distance(Vector3D value1, Vector3D value2)
        {
            double num1 = value1.X - value2.X;
            double num2 = value1.Y - value2.Y;
            double num3 = value1.Z - value2.Z;
            return SystemMath.Sqrt(num1 * num1 + num2 * num2 + num3 * num3);
        }
    }
}