using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator.Toolkit.Math
{
    class Matrix3x3 : IEquatable<Matrix3x3>, IFormattable
    {
        public static readonly Matrix3x3 Zero = new Matrix3x3(0);
        public static readonly Matrix3x3 Identity = new Matrix3x3(1.0,0,0,0,1.0,0,0,0,1.0);
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
            this.M11 = M11; this.M12 = M12; this.M13 = M13;
            this.M21 = M21; this.M22 = M22; this.M23 = M23;
            this.M31 = M31; this.M32 = M32; this.M33 = M33;
        }

        public Matrix3x3(double[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 9)
                throw new ArgumentOutOfRangeException("values", "There must be nine and only nine input values for Matrix3x3.");

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
            set { M11 = value.X; M12 = value.Y; M13 = value.Z; }
        }

        public Vector3 Row2
        {
            get { return new Vector3(M21, M22, M23); }
            set { M21 = value.X; M22 = value.Y; M23 = value.Z; }
        }

        public Vector3 Row3
        {
            get { return new Vector3(M31, M32, M33); }
            set { M31 = value.X; M32 = value.Y; M33 = value.Z; }
        }

        public Vector3 Column1
        {
            get { return new Vector3(M11, M21, M31); }
            set { M11 = value.X; M21 = value.Y; M31 = value.Z; }
        }

        public Vector3 Column2
        {
            get { return new Vector3(M12, M22, M32); }
            set { M12 = value.X; M22 = value.Y; M32 = value.Z; }
        }

        public Vector3 Column3
        {
            get { return new Vector3(M13, M23, M33); }
            set { M13 = value.X; M23 = value.Y; M33 = value.Z; }
        }        

        public Vector3 ScaleVector
        {
            get { return new Vector3(M11, M22, M33); }
            set { M11 = value.X; M22 = value.Y; M33 = value.Z; }
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
                    case 0: return M11;
                    case 1: return M12;
                    case 2: return M13;
                    case 3: return M21;
                    case 4: return M22;
                    case 5: return M23;
                    case 6: return M31;
                    case 7: return M32;
                    case 8: return M33;
                }
                throw new ArgumentOutOfRangeException("index", "Indices for Matrix3x3 run from 0 to 8, inclusive.");
            }

            set
            {
                switch (index)
                {
                    case 0: M11 = value; break;
                    case 1: M12 = value; break;
                    case 2: M13 = value; break;
                    case 3: M21 = value; break;
                    case 4: M22 = value; break;
                    case 5: M23 = value; break;
                    case 6: M31 = value; break;
                    case 7: M32 = value; break;
                    case 8: M33 = value; break;
                    default: throw new ArgumentOutOfRangeException("index", "Indices for Matrix3x3 run from 0 to 8, inclusive.");
                }
            }
        }
        public double this[int row, int column]
        {
            get
            {
                if (row < 0 || row > 2)
                    throw new ArgumentOutOfRangeException("row", "Rows and columns for matrices run from 0 to 2, inclusive.");
                if (column < 0 || column > 2)
                    throw new ArgumentOutOfRangeException("column", "Rows and columns for matrices run from 0 to 2, inclusive.");

                return this[(row * 3) + column];
            }

            set
            {
                if (row < 0 || row > 2)
                    throw new ArgumentOutOfRangeException("row", "Rows and columns for matrices run from 0 to 2, inclusive.");
                if (column < 0 || column > 2)
                    throw new ArgumentOutOfRangeException("column", "Rows and columns for matrices run from 0 to 2, inclusive.");

                this[(row * 3) + column] = value;
            }
        }
        public double[] ToArray()
        {
            return new[] { M11, M12, M13, M21, M22, M23, M31, M32, M33 };
        }
        public double Determinant()
        {
            return M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32 - M13 * M22 * M31 - M12 * M21 * M33 - M11 * M23 * M32;
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
        
        /// <summary>
        /// Scales a Matrix3x3 by the given value.
        /// </summary>
        /// <param name="left">The Matrix3x3 to scale.</param>
        /// <param name="right">The amount by which to scale.</param>
        /// <param name="result">When the method completes, contains the scaled Matrix3x3.</param>
        public static void Multiply(ref Matrix3x3 left, double right, out Matrix3x3 result)
        {
            result.M11 = left.M11 * right;
            result.M12 = left.M12 * right;
            result.M13 = left.M13 * right;
            result.M21 = left.M21 * right;
            result.M22 = left.M22 * right;
            result.M23 = left.M23 * right;
            result.M31 = left.M31 * right;
            result.M32 = left.M32 * right;
            result.M33 = left.M33 * right;
        }

        /// <summary>
        /// Scales a Matrix3x3 by the given value.
        /// </summary>
        /// <param name="left">The Matrix3x3 to scale.</param>
        /// <param name="right">The amount by which to scale.</param>
        /// <returns>The scaled Matrix3x3.</returns>
        public static Matrix3x3 Multiply(Matrix3x3 left, double right)
        {
            Matrix3x3 result;
            Multiply(ref left, right, out result);
            return result;
        }

        /// <summary>
        /// Determines the product of two matrices.
        /// </summary>
        /// <param name="left">The first Matrix3x3 to multiply.</param>
        /// <param name="right">The second Matrix3x3 to multiply.</param>
        /// <param name="result">The product of the two matrices.</param>
        public static void Multiply(ref Matrix3x3 left, ref Matrix3x3 right, out Matrix3x3 result)
        {
            Matrix3x3 temp = new Matrix3x3();
            temp.M11 = (left.M11 * right.M11) + (left.M12 * right.M21) + (left.M13 * right.M31);
            temp.M12 = (left.M11 * right.M12) + (left.M12 * right.M22) + (left.M13 * right.M32);
            temp.M13 = (left.M11 * right.M13) + (left.M12 * right.M23) + (left.M13 * right.M33);
            temp.M21 = (left.M21 * right.M11) + (left.M22 * right.M21) + (left.M23 * right.M31);
            temp.M22 = (left.M21 * right.M12) + (left.M22 * right.M22) + (left.M23 * right.M32);
            temp.M23 = (left.M21 * right.M13) + (left.M22 * right.M23) + (left.M23 * right.M33);
            temp.M31 = (left.M31 * right.M11) + (left.M32 * right.M21) + (left.M33 * right.M31);
            temp.M32 = (left.M31 * right.M12) + (left.M32 * right.M22) + (left.M33 * right.M32);
            temp.M33 = (left.M31 * right.M13) + (left.M32 * right.M23) + (left.M33 * right.M33);
            result = temp;
        }

        /// <summary>
        /// Determines the product of two matrices.
        /// </summary>
        /// <param name="left">The first Matrix3x3 to multiply.</param>
        /// <param name="right">The second Matrix3x3 to multiply.</param>
        /// <returns>The product of the two matrices.</returns>
        public static Matrix3x3 Multiply(Matrix3x3 left, Matrix3x3 right)
        {
            Matrix3x3 result;
            Multiply(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Scales a Matrix3x3 by the given value.
        /// </summary>
        /// <param name="left">The Matrix3x3 to scale.</param>
        /// <param name="right">The amount by which to scale.</param>
        /// <param name="result">When the method completes, contains the scaled Matrix3x3.</param>
        public static void Divide(ref Matrix3x3 left, double right, out Matrix3x3 result)
        {
            double inv = 1.0f / right;

            result.M11 = left.M11 * inv;
            result.M12 = left.M12 * inv;
            result.M13 = left.M13 * inv;
            result.M21 = left.M21 * inv;
            result.M22 = left.M22 * inv;
            result.M23 = left.M23 * inv;
            result.M31 = left.M31 * inv;
            result.M32 = left.M32 * inv;
            result.M33 = left.M33 * inv;
        }

        /// <summary>
        /// Scales a Matrix3x3 by the given value.
        /// </summary>
        /// <param name="left">The Matrix3x3 to scale.</param>
        /// <param name="right">The amount by which to scale.</param>
        /// <returns>The scaled Matrix3x3.</returns>
        public static Matrix3x3 Divide(Matrix3x3 left, double right)
        {
            Matrix3x3 result;
            Divide(ref left, right, out result);
            return result;
        }
        
        public static void Negate(ref Matrix3x3 value, out Matrix3x3 result)
        {
            result.M11 = -value.M11;
            result.M12 = -value.M12;
            result.M13 = -value.M13;
            result.M21 = -value.M21;
            result.M22 = -value.M22;
            result.M23 = -value.M23;
            result.M31 = -value.M31;
            result.M32 = -value.M32;
            result.M33 = -value.M33;
        }

        /// <summary>
        /// Negates a Matrix3x3.
        /// </summary>
        /// <param name="value">The Matrix3x3 to be negated.</param>
        /// <returns>The negated Matrix3x3.</returns>
        public static Matrix3x3 Negate(Matrix3x3 value)
        {
            Matrix3x3 result;
            Negate(ref value, out result);
            return result;
        }

        public static void Transpose(ref Matrix3x3 value, out Matrix3x3 result)
        {
            Matrix3x3 temp = new Matrix3x3();
            temp.M11 = value.M11;
            temp.M12 = value.M21;
            temp.M13 = value.M31;
            temp.M21 = value.M12;
            temp.M22 = value.M22;
            temp.M23 = value.M32;
            temp.M31 = value.M13;
            temp.M32 = value.M23;
            temp.M33 = value.M33;

            result = temp;
        }


        /// <summary>
        /// Calculates the transpose of the specified Matrix3x3.
        /// </summary>
        /// <param name="value">The Matrix3x3 whose transpose is to be calculated.</param>
        /// <returns>The transpose of the specified Matrix3x3.</returns>
        public static Matrix3x3 Transpose(Matrix3x3 value)
        {
            Matrix3x3 result;
            Transpose(ref value, out result);
            return result;
        }

        /// <summary>
        /// Calculates the inverse of the specified Matrix3x3.
        /// </summary>
        /// <param name="value">The Matrix3x3 whose inverse is to be calculated.</param>
        /// <param name="result">When the method completes, contains the inverse of the specified Matrix3x3.</param>
        public static void Invert(ref Matrix3x3 value, out Matrix3x3 result)
        {
            double d11 = value.M22 * value.M33 + value.M23 * -value.M32;
            double d12 = value.M21 * value.M33 + value.M23 * -value.M31;
            double d13 = value.M21 * value.M32 + value.M22 * -value.M31;

            double det = value.M11 * d11 - value.M12 * d12 + value.M13 * d13;
            if (Math.Abs(det) == 0.0f)
            {
                result = Matrix3x3.Zero;
                return;
            }

            det = 1f / det;

            double d21 = value.M12 * value.M33 + value.M13 * -value.M32;
            double d22 = value.M11 * value.M33 + value.M13 * -value.M31;
            double d23 = value.M11 * value.M32 + value.M12 * -value.M31;

            double d31 = (value.M12 * value.M23) - (value.M13 * value.M22);
            double d32 = (value.M11 * value.M23) - (value.M13 * value.M21);
            double d33 = (value.M11 * value.M22) - (value.M12 * value.M21);

            result.M11 = +d11 * det; result.M12 = -d21 * det; result.M13 = +d31 * det;
            result.M21 = -d12 * det; result.M22 = +d22 * det; result.M23 = -d32 * det;
            result.M31 = +d13 * det; result.M32 = -d23 * det; result.M33 = +d33 * det;
        }

        /// <summary>
        /// Calculates the inverse of the specified Matrix3x3.
        /// </summary>
        /// <param name="value">The Matrix3x3 whose inverse is to be calculated.</param>
        /// <returns>The inverse of the specified Matrix3x3.</returns>
        public static Matrix3x3 Invert(Matrix3x3 value)
        {
            value.Invert();
            return value;
        }

        public static Matrix3x3 operator +(Matrix3x3 left, Matrix3x3 right)
        {
            Matrix3x3 result;
            Add(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Assert a Matrix3x3 (return it unchanged).
        /// </summary>
        /// <param name="value">The Matrix3x3 to assert (unchanged).</param>
        /// <returns>The asserted (unchanged) Matrix3x3.</returns>
        public static Matrix3x3 operator +(Matrix3x3 value)
        {
            return value;
        }

        /// <summary>
        /// Subtracts two matrices.
        /// </summary>
        /// <param name="left">The first Matrix3x3 to subtract.</param>
        /// <param name="right">The second Matrix3x3 to subtract.</param>
        /// <returns>The difference between the two matrices.</returns>
        public static Matrix3x3 operator -(Matrix3x3 left, Matrix3x3 right)
        {
            Matrix3x3 result;
            Subtract(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Negates a Matrix3x3.
        /// </summary>
        /// <param name="value">The Matrix3x3 to negate.</param>
        /// <returns>The negated Matrix3x3.</returns>
        public static Matrix3x3 operator -(Matrix3x3 value)
        {
            Matrix3x3 result;
            Negate(ref value, out result);
            return result;
        }

        /// <summary>
        /// Scales a Matrix3x3 by a given value.
        /// </summary>
        /// <param name="right">The Matrix3x3 to scale.</param>
        /// <param name="left">The amount by which to scale.</param>
        /// <returns>The scaled Matrix3x3.</returns>
        public static Matrix3x3 operator *(double left, Matrix3x3 right)
        {
            Matrix3x3 result;
            Multiply(ref right, left, out result);
            return result;
        }

        /// <summary>
        /// Scales a Matrix3x3 by a given value.
        /// </summary>
        /// <param name="left">The Matrix3x3 to scale.</param>
        /// <param name="right">The amount by which to scale.</param>
        /// <returns>The scaled Matrix3x3.</returns>
        public static Matrix3x3 operator *(Matrix3x3 left, double right)
        {
            Matrix3x3 result;
            Multiply(ref left, right, out result);
            return result;
        }

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <param name="left">The first Matrix3x3 to multiply.</param>
        /// <param name="right">The second Matrix3x3 to multiply.</param>
        /// <returns>The product of the two matrices.</returns>
        public static Matrix3x3 operator *(Matrix3x3 left, Matrix3x3 right)
        {
            Matrix3x3 result;
            Multiply(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Scales a Matrix3x3 by a given value.
        /// </summary>
        /// <param name="left">The Matrix3x3 to scale.</param>
        /// <param name="right">The amount by which to scale.</param>
        /// <returns>The scaled Matrix3x3.</returns>
        public static Matrix3x3 operator /(Matrix3x3 left, double right)
        {
            Matrix3x3 result;
            Divide(ref left, right, out result);
            return result;
        }

        /// <summary>
        /// Divides two matrices.
        /// </summary>
        /// <param name="left">The first Matrix3x3 to divide.</param>
        /// <param name="right">The second Matrix3x3 to divide.</param>
        /// <returns>The quotient of the two matrices.</returns>
        public static Matrix3x3 operator /(Matrix3x3 left, Matrix3x3 right)
        {
            Matrix3x3 result;
            Divide(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Tests for equality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Matrix3x3 left, Matrix3x3 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests for inequality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Matrix3x3 left, Matrix3x3 right)
        {
            return !left.Equals(right);
        }
        
        /// <summary>
        /// Convert the 3x3 Matrix to a 4x4 Matrix.
        /// </summary>
        /// <returns>A 4x4 Matrix with zero translation and M44=1</returns>
        public static explicit operator Matrix(Matrix3x3 Value)
        {
            return new Matrix(
                Value.M11, Value.M12, Value.M13 , 0 ,
                Value.M21, Value.M22, Value.M23 , 0 ,
                Value.M31, Value.M32, Value.M33 , 0 ,
                0, 0, 0 , 1
                );
        }

        /// <summary>
        /// Convert the 4x4 Matrix to a 3x3 Matrix.
        /// </summary>
        /// <returns>A 3x3 Matrix</returns>
        public static explicit operator Matrix3x3(Matrix Value)
        {
            return new Matrix3x3(
                Value.M11, Value.M12, Value.M13,
                Value.M21, Value.M22, Value.M23,
                Value.M31, Value.M32, Value.M33
                );
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "[M11:{0} M12:{1} M13:{2}] [M21:{3} M22:{4} M23:{5}] [M31:{6} M32:{7} M33:{8}]",
                M11, M12, M13, M21, M22, M23, M31, M32, M33);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format)
        {
            if (format == null)
                return ToString();

            return string.Format(format, CultureInfo.CurrentCulture, "[M11:{0} M12:{1} M13:{2}] [M21:{3} M22:{4} M23:{5}] [M31:{6} M32:{7} M33:{8}]",
                M11.ToString(format, CultureInfo.CurrentCulture), M12.ToString(format, CultureInfo.CurrentCulture), M13.ToString(format, CultureInfo.CurrentCulture),
                M21.ToString(format, CultureInfo.CurrentCulture), M22.ToString(format, CultureInfo.CurrentCulture), M23.ToString(format, CultureInfo.CurrentCulture),
                M31.ToString(format, CultureInfo.CurrentCulture), M32.ToString(format, CultureInfo.CurrentCulture), M33.ToString(format, CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "[M11:{0} M12:{1} M13:{2}] [M21:{3} M22:{4} M23:{5}] [M31:{6} M32:{7} M33:{8}]",
                M11.ToString(formatProvider), M12.ToString(formatProvider), M13.ToString(formatProvider),
                M21.ToString(formatProvider), M22.ToString(formatProvider), M23.ToString(formatProvider),
                M31.ToString(formatProvider), M32.ToString(formatProvider), M33.ToString(formatProvider));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString(formatProvider);

            return string.Format(format, formatProvider, "[M11:{0} M12:{1} M13:{2}] [M21:{3} M22:{4} M23:{5}] [M31:{6} M32:{7} M33:{8}]",
                M11.ToString(format, formatProvider), M12.ToString(format, formatProvider), M13.ToString(format, formatProvider),
                M21.ToString(format, formatProvider), M22.ToString(format, formatProvider), M23.ToString(format, formatProvider),
                M31.ToString(format, formatProvider), M32.ToString(format, formatProvider), M33.ToString(format, formatProvider));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = M11.GetHashCode();
                hashCode = (hashCode * 397) ^ M12.GetHashCode();
                hashCode = (hashCode * 397) ^ M13.GetHashCode();
                hashCode = (hashCode * 397) ^ M21.GetHashCode();
                hashCode = (hashCode * 397) ^ M22.GetHashCode();
                hashCode = (hashCode * 397) ^ M23.GetHashCode();
                hashCode = (hashCode * 397) ^ M31.GetHashCode();
                hashCode = (hashCode * 397) ^ M32.GetHashCode();
                hashCode = (hashCode * 397) ^ M33.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="SharpDX.Matrix3x3"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SharpDX.Matrix3x3"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="SharpDX.Matrix3x3"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Matrix3x3 other)
        {
            return (MathUtil.NearEqual(other.M11, M11) &&
                MathUtil.NearEqual(other.M12, M12) &&
                MathUtil.NearEqual(other.M13, M13) &&
                MathUtil.NearEqual(other.M21, M21) &&
                MathUtil.NearEqual(other.M22, M22) &&
                MathUtil.NearEqual(other.M23, M23) &&
                MathUtil.NearEqual(other.M31, M31) &&
                MathUtil.NearEqual(other.M32, M32) &&
                MathUtil.NearEqual(other.M33, M33));
        }

        /// <summary>
        /// Determines whether the specified <see cref="SharpDX.Matrix3x3"/> are equal.
        /// </summary>
        public static bool Equals(ref Matrix3x3 a,ref Matrix3x3 b)
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

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object value)
        {
            if (value == null)
                return false;

            if (!ReferenceEquals(value.GetType(), typeof(Matrix3x3)))
                return false;

            return Equals((Matrix3x3)value);
        }
    }
}