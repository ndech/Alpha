using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator.Toolkit.Math
{
    public class Vector3 : IComparable, IComparable<Vector3>, IEquatable<Vector3>, IFormattable
    {
        public static readonly Vector3 Origin = new Vector3(0,0,0);
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double this [int index]
        {
            get
            {
                if (index == 0)
                    return X;
                else if (index == 1)
                    return Y;
                else if (index == 2)
                    return Z;
                throw new ArgumentOutOfRangeException("index must be less than three", "index");
            }
            set
            {
                if (index == 0)
                    X = value;
                else if (index == 1)
                    Y = value;
                else if (index == 2)
                    Z = value;
                throw new ArgumentOutOfRangeException("index must be less than three", "index");
            }
        }
        public double Magnitude 
        {
            get
            {
                return System.Math.Sqrt(SumOfSquaredComponents);
            }
            set
            {
                if(value<0)
                    throw new ArgumentOutOfRangeException("Magnitude must be positive", "Magnitude");
                if(this == Origin)
                    throw new DivideByZeroException("Can not set the magnitude on the origin vector");
                this = this * (value/Magnitude);

            }
        }
        public double SumOfSquaredComponents
        {
            get
            {
                return X*X+Y*Y+Z*Z;
            }
        }
        public Vector3(double value)
        {
            X = value;
            Y = value;
            Z = value;
        }
        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return
            (
               new Vector3
               (
                  v1.X + v2.X,
                  v1.Y + v2.Y,
                  v1.Z + v2.Z
               )
            );
        }
        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return
            (
               new Vector3
               (
                  v1.X - v2.X,
                  v1.Y - v2.Y,
                  v1.Z - v2.Z
               )
            );
        }
        public static Vector3 operator -(Vector3 v1)
        {
            return
            (
               new Vector3
               (
                  -v1.X,
                  -v1.Y,
                  -v1.Z
               )
            );
        }
        public static bool operator <(Vector3 v1, Vector3 v2)
        {
            return v1.Magnitude < v2.Magnitude;
        }
        public static bool operator <=(Vector3 v1, Vector3 v2)
        {
            return v1.Magnitude <= v2.Magnitude;
        }
        public static bool operator >(Vector3 v1, Vector3 v2)
        {
            return v1.Magnitude > v2.Magnitude;
        }
        public static bool operator >=(Vector3 v1, Vector3 v2)
        {
            return v1.Magnitude >= v2.Magnitude;
        }
        public static bool operator ==(Vector3 v1, Vector3 v2)
        {
            return (v1.X == v2.X) && (v1.Y == v2.Y) && (v1.Z == v2.Z);
        }
        public static bool operator !=(Vector3 v1, Vector3 v2)
        {
            return !(v1 == v2);
        }
        public static Vector3 operator /(Vector3 v1, double s2)
        {
            return new Vector3 (v1.X / s2, v1.Y / s2, v1.Z / s2);
        }
        public static Vector3 operator *(Vector3 v1, double s2)
        {
            return new Vector3 (v1.X * s2, v1.Y * s2, v1.Z * s2);
        }
        public static Vector3 operator *(double s2, Vector3 v1)
        {
            return v1 * s2;
        }
        public static bool IsUnit(Vector3 v1)
        {
            return v1.Magnitude == 1;
        }
        public bool IsUnit()
        {
            return IsUnit(this);
        }
        public static Vector3 Cross(Vector3 left, Vector3 right)
        {
            return new Vector3(
                (left.Y * right.Z) - (left.Z * right.Y),
                (left.Z * right.X) - (left.X * right.Z),
                (left.X * right.Y) - (left.Y * right.X));
        }
        public static double Dot(Vector3 left, Vector3 right)
        {
            return (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z);
        }

    }
}
