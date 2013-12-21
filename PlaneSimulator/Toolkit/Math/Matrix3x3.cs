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
        public static readonly Matrix3x3 Identity = new Matrix3x3() { M11 = 1.0f, M22 = 1.0f, M33 = 1.0f };
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
                    case 1: M12 = value; br