using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator.Toolkit.Math
{
    internal class Matrix3x3 : IEquatable<Matrix3x3>, IFormattable
    {
        public static readonly Matrix3x3 Zero = new Matrix3x3(0);
        public static readonly Matrix3x3 Identity = new Matrix3x3(1.0, 0, 0, 0, 1.0, 0, 0, 0, 1.0);
        public double M11 { get; set; }
        public double M12 { get; set; }
        public double M13 { get; set; }
        public double M21 { get; set; }
        public double M22 { get; set; }
        public double M23 { get; set; }
        public double M31 { get; set; }
        public double M32 { get; set; }
        public double M33 { get; set; }

        public Matrix3x3(double value)
        {
            M11 = M12 = M13 =
                M21 = M22 = M23 =
                    M31 = M32 = M33 = value;
        }

        public Matrix3x3(double M11, double M12, double M13,
            double M21, double M22, double M23,
            double M31, double M32, double M33)
        {
            this.M11 = M11;
            this.M12 = M12;
            this.M13 = M13;
            this.M21 = M21;
            this.M22 = M22;
            this.M23 = M23;
            this.M31 = M31;
            this.M32 = M32;
            this.M33 = M33;
        }

        public Matrix3x3(double[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 9)
                throw new ArgumentOutOfRangeException("values",
                    "There must be nine and only nine input values for Matrix3x3.");

            M11 = values[0];
            M12 = values[1];
            M13 = values[2];

            M21 = values[3];
            M22 = values[4];
            M23 = values[5];

            M31 = values[6];
            M32 = values[7];
            M33 = values[8];
        }

        public Vector3 Row1
        {
            get { return new Vector3(M11, M12, M13); }
            set
            {
                M11 = value.X;
                M12 = value.Y;
                M13 = value.Z;
            }
        }

        public Vector3 Row2
        {
            get { return new Vector3(M21, M22, M23); }
            set
            {
                M21 = value.X;
                M22 = value.Y;
                M23 = value.Z;
            }
        }

        public Vector3 Row3
        {
            get { return new Vector3(M31, M32, M33); }
            set
            {
                M31 = value.X;
                M32 = value.Y;
                M33 = value.Z;
            }
        }

        public Vector3 Column1
        {
            get { return new Vector3(M11, M21, M31); }
            set
            {
                M11 = value.X;
                M21 = value.Y;
                M31 = value.Z;
            }
        }

        public Vector3 Column2
        {
            get { return new Vector3(M12, M22, M32); }
            set
            {
                M12 = value.X;
                M22 = value.Y;
                M32 = value.Z;
            }
        }

        public Vector3 Column3
        {
            get { return new Vector3(M13, M23, M33); }
            set
            {
                M13 = value.X;
                M23 = value.Y;
                M33 = value.Z;
            }
        }

        public Vector3 ScaleVector
        {
            get { return new Vector3(M11, M22, M33); }
            set
            {
                M11 = value.X;
                M22 = value.Y;
                M33 = value.Z;
            }
        }

        public bool IsIdentity()
        {
            return this.Equals(Identity);
        }

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return M11;
                    case 1:
                        return M12;
                    case 2:
                        return M13;
                    case 3:
                        return M21;
                    case 4:
                        return M22;
                    case 5:
                        return M23;
                    case 6:
                        return M31;
                    case 7:
                        return M32;
                    case 8:
                        return M33;
                }
                throw new ArgumentOutOfRangeException("index", "Indices for Matrix3x3 run from 0 to 8, inclusive.");
            }

            set
            {
                switch (index)
                {
                    case 0:
                        M11 = value;
                        break;
                    case 1:
                        M12 = value;
                        break;
                    case 2:
                        M13 = value;
                        break;
                    case 3:
                        M21 = value;
                        break;
                    case 4:
                        M22 = value;
                        break;
                    case 5:
                        M23 = value;
                        break;
                    case 6:
                        M31 = value;
                        break;
                    case 7:
                        M32 = value;
                        break;
                    case 8:
                        M33 = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("index",
                            "Indices for Matrix3x3 run from 0 to 8, inclusive.");
                }
            }
        }

        public double this[int row, int column]
        {
            get
            {
                if (row < 0 || row > 2)
                    throw new ArgumentOutOfRangeException("row",
                        "Rows and columns for matrices run from 0 to 2, inclusive.");
                if (column < 0 || column > 2)
                    throw new ArgumentOutOfRangeException("column",
                        "Rows and columns for matrices run from 0 to 2, inclusive.");

                return this[(row*3) + column];
            }

            set
            {
                if (row < 0 || row > 2)
                    throw new ArgumentOutOfRangeException("row",
                        "Rows and columns for matrices run from 0 to 2, inclusive.");
                if (column < 0 || column > 2)
                    throw new ArgumentOutOfRangeException("column",
                        "Rows and columns for matrices run from 0 to 2, inclusive.");

                this[(row*3) + column] = value;
            }
        }

        public double[] ToArray()
        {
            return new[] {M11, M12, M13, M21, M22, M23, M31, M32, M33};
        }

        public double Determinant()
        {
            return M11*M22*M33 + M12*M23*M31 + M13*M21*M32 - M13*M22*M31 - M12*M21*M33 - M11*M23*M32;
        }

        public static Matrix3x3 Add(Matrix3x3 left, Matrix3x3 right)
        {
            return new Matrix3x3(
                left.M11 + right.M11,
                left.M12 + right.M12,
                left.M13 + right.M13,
                left.M21 + right.M21,
                left.M22 + right.M22,
                left.M23 + right.M23,
                left.M31 + right.M31,
                left.M32 + right.M32,
                left.M33 + right.M33
                );
        }

        public static Matrix3x3 Substract(Matrix3x3 left, Matrix3x3 right)
        {
            return new Matrix3x3(
                left.M11 - right.M11,
                left.M12 - right.M12,
                left.M13 - right.M13,
                left.M21 - right.M21,
                left.M22 - right.M22,
                left.M23 - right.M23,
                left.M31 - right.M31,
                left.M32 - right.M32,
                left.M33 - right.M33
                );
        }

        public static Matrix3x3 Multiply(Matrix3x3 left, double multiplicator)
        {
            return new Matrix3x3(
                left.M11*multiplicator,
                left.M12*multiplicator,
                left.M13*multiplicator,
                left.M21*multiplicator,
                left.M22*multiplicator,
                left.M23*multiplicator,
                left.M31*multiplicator,
                left.M32*multiplicator,
                left.M33*multiplicator
                );
        }

        public static Matrix3x3 Multiply(Matrix3x3 left, Matrix3x3 right)
        {
            return new Matrix3x3(
                (left.M11*right.M11) + (left.M12*right.M21) + (left.M13*right.M31),
                (left.M11*right.M12) + (left.M12*right.M22) + (left.M13*right.M32),
                (left.M11*right.M13) + (left.M12*right.M23) + (left.M13*right.M33),
                (left.M21*right.M11) + (left.M22*right.M21) + (left.M23*right.M31),
                (left.M21*right.M12) + (left.M22*right.M22) + (left.M23*right.M32),
                (left.M21*right.M13) + (left.M22*right.M23) + (left.M23*right.M33),
                (left.M31*right.M11) + (left.M32*right.M21) + (left.M33*right.M31),
                (left.M31*right.M12) + (left.M32*right.M22) + (left.M33*right.M32),
                (left.M31*right.M13) + (left.M32*right.M23) + (left.M33*right.M33)
                );
        }

        public static Vector3 Multiply(Matrix3x3 left, Vector3 right)
        {
            return new Vector3(
                (left.M11*right.X) + (left.M12*right.Y) + (left.M13*right.Z),
                (left.M11*right.X) + (left.M12*right.Y) + (left.M13*right.Z),
                (left.M11*right.X) + (left.M12*right.Y) + (left.M13*right.Z)
                );
        }

        public static Matrix3x3 Divide(Matrix3x3 left, double right)
        {
            if (right == 0.0)
                throw new DivideByZeroException();
            return Multiply(left, 1.0/right);
        }

        public static Matrix3x3 Negate(Matrix3x3 value)
        {
            return new Matrix3x3(
                -value.M11,
                -value.M12,
                -value.M13,
                -value.M21,
                -value.M22,
                -value.M23,
                -value.M31,
                -value.M32,
                -value.M33);
        }

        public static Matrix3x3 Transpose(Matrix3x3 value)
        {
            return new Matrix3x3(
                value.M11,
                value.M21,
                value.M31,
                value.M12,
                value.M22,
                value.M32,
                value.M13,
                value.M23,
                value.M33
                );
        }

        public Matrix3x3 Transpose()
        {
            return Transpose(this);
        }

        public static Matrix3x3 Invert(Matrix3x3 value)
        {
            double determinant = value.Determinant();
            if (determinant == 0.0)
                throw new DivideByZeroException("Matrix not inversible");

            determinant = 1.0/determinant;

            double d11 = (value.M22*value.M33) - (value.M23*value.M32);
            double d12 = (value.M21*value.M33) - (value.M23*value.M31);
            double d13 = (value.M21*value.M32) - (value.M22*value.M31);

            double d21 = (value.M12*value.M33) - (value.M13*value.M32);
            double d22 = (value.M11*value.M33) - (value.M13*value.M31);
            double d23 = (value.M11*value.M32) - (value.M12*value.M31);

            double d31 = (value.M12*value.M23) - (value.M13*value.M22);
            double d32 = (value.M11*value.M23) - (value.M13*value.M21);
            double d33 = (value.M11*value.M22) - (value.M12*value.M21);

            return new Matrix3x3(
                d11*determinant,
                -d21*determinant,
                d31*determinant,
                -d12*determinant,
                d22*determinant,
                -d32*determinant,
                d13*determinant,
                -d23*determinant,
                d33*determinant
                );
        }

        public Matrix3x3 Invert()
        {
            return Invert(this);
        }

        public static Matrix3x3 operator +(Matrix3x3 left, Matrix3x3 right)
        {
            return Add(left, right);
        }

        public static Matrix3x3 operator +(Matrix3x3 value)
        {
            return value;
        }

        public static Matrix3x3 operator -(Matrix3x3 left, Matrix3x3 right)
        {
            return Substract(left, right);
        }

        public static Matrix3x3 operator -(Matrix3x3 value)
        {
            return Negate(value);
        }

        public static Matrix3x3 operator *(double left, Matrix3x3 right)
        {
            return Multiply(right, left);
        }

        public static Matrix3x3 operator *(Matrix3x3 left, double right)
        {
            return Multiply(left, right);
        }

        public static Matrix3x3 operator *(Matrix3x3 left, Matrix3x3 right)
        {
            return Multiply(left, right);
        }

        public static Vector3 operator *(Matrix3x3 left, Vector3 right)
        {
            return Multiply(left, right);
        }

        public static Matrix3x3 operator /(Matrix3x3 left, double right)
        {
            return Divide(left, right);
        }

        public static bool operator ==(Matrix3x3 left, Matrix3x3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Matrix3x3 left, Matrix3x3 right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "[M11:{0} M12:{1} M13:{2}] [M21:{3} M22:{4} M23:{5}] [M31:{6} M32:{7} M33:{8}]",
                M11, M12, M13, M21, M22, M23, M31, M32, M33);
        }

        public string ToString(string format)
        {
            if (format == null)
                return ToString();

            return string.Format(format, CultureInfo.CurrentCulture,
                "[M11:{0} M12:{1} M13:{2}] [M21:{3} M22:{4} M23:{5}] [M31:{6} M32:{7} M33:{8}]",
                M11.ToString(format, CultureInfo.CurrentCulture), M12.ToString(format, CultureInfo.CurrentCulture),
                M13.ToString(format, CultureInfo.CurrentCulture),
                M21.ToString(format, CultureInfo.CurrentCulture), M22.ToString(format, CultureInfo.CurrentCulture),
                M23.ToString(format, CultureInfo.CurrentCulture),
                M31.ToString(format, CultureInfo.CurrentCulture), M32.ToString(format, CultureInfo.CurrentCulture),
                M33.ToString(format, CultureInfo.CurrentCulture));
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider,
                "[M11:{0} M12:{1} M13:{2}] [M21:{3} M22:{4} M23:{5}] [M31:{6} M32:{7} M33:{8}]",
                M11.ToString(formatProvider), M12.ToString(formatProvider), M13.ToString(formatProvider),
                M21.ToString(formatProvider), M22.ToString(formatProvider), M23.ToString(formatProvider),
                M31.ToString(formatProvider), M32.ToString(formatProvider), M33.ToString(formatProvider));
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString(formatProvider);

            return string.Format(format, formatProvider,
                "[M11:{0} M12:{1} M13:{2}] [M21:{3} M22:{4} M23:{5}] [M31:{6} M32:{7} M33:{8}]",
                M11.ToString(format, formatProvider), M12.ToString(format, formatProvider),
                M13.ToString(format, formatProvider),
                M21.ToString(format, formatProvider), M22.ToString(format, formatProvider),
                M23.ToString(format, formatProvider),
                M31.ToString(format, formatProvider), M32.ToString(format, formatProvider),
                M33.ToString(format, formatProvider));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = M11.GetHashCode();
                hashCode = (hashCode*397) ^ M12.GetHashCode();
                hashCode = (hashCode*397) ^ M13.GetHashCode();
                hashCode = (hashCode*397) ^ M21.GetHashCode();
                hashCode = (hashCode*397) ^ M22.GetHashCode();
                hashCode = (hashCode*397) ^ M23.GetHashCode();
                hashCode = (hashCode*397) ^ M31.GetHashCode();
                hashCode = (hashCode*397) ^ M32.GetHashCode();
                hashCode = (hashCode*397) ^ M33.GetHashCode();
                return hashCode;
            }
        }

        public static bool Equals(Matrix3x3 a, Matrix3x3 b)
        {
            return
                MathUtil.NearEqual(a.M11, b.M11) &&
                MathUtil.NearEqual(a.M12, b.M12) &&
                MathUtil.NearEqual(a.M13, b.M13) &&
                MathUtil.NearEqual(a.M21, b.M21) &&
                MathUtil.NearEqual(a.M22, b.M22) &&
                MathUtil.NearEqual(a.M23, b.M23) &&
                MathUtil.NearEqual(a.M31, b.M31) &&
                MathUtil.NearEqual(a.M32, b.M32) &&
                MathUtil.NearEqual(a.M33, b.M33)
                ;
        }

        public bool Equals(Matrix3x3 other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object value)
        {
            if (value == null)
                return false;
            if (!ReferenceEquals(value.GetType(), typeof (Matrix3x3)))
                return false;
            return Equals((Matrix3x3) value);
        }
    }
}