using System;
using UnityEngine;

namespace Parallel
{
    [Serializable]
    //column-major order.
    public struct FMatrix4x4
    {
        //https://github.com/microsoft/referencesource/blob/master/System.Numerics/System/Numerics/Matrix4x4.cs
        public FFloat m00;
        public FFloat m10;
        public FFloat m20;
        public FFloat m30;
        public FFloat m01;
        public FFloat m11;
        public FFloat m21;
        public FFloat m31;
        public FFloat m02;
        public FFloat m12;
        public FFloat m22;
        public FFloat m32;
        public FFloat m03;
        public FFloat m13;
        public FFloat m23;
        public FFloat m33;

        //https://referencesource.microsoft.com/#System.Numerics/System/Numerics/Matrix4x4.cs
        // Value at row 1, column 1 of the matrix.
        FFloat M11
        {
            get
            {
                return m00;
            }
            set
            {
                m00 = value;
            }
        }

        // Value at row 1, column 2 of the matrix.
        FFloat M12
        {
            get
            {
                return m10;
            }
            set
            {
                m10 = value;
            }
        }

        // Value at row 1, column 3 of the matrix.
        FFloat M13
        {
            get
            {
                return m20;
            }
            set
            {
                m20 = value;
            }
        }

        // Value at row 1, column 4 of the matrix.
        FFloat M14
        {
            get
            {
                return m30;
            }
            set
            {
                m30 = value;
            }
        }

        // Value at row 2, column 1 of the matrix.
        FFloat M21
        {
            get
            {
                return m01;
            }
            set
            {
                m01 = value;
            }
        }

        // Value at row 2, column 2 of the matrix.
        FFloat M22
        {
            get
            {
                return m11;
            }
            set
            {
                m11 = value;
            }
        }

        // Value at row 2, column 3 of the matrix.
        FFloat M23
        {
            get
            {
                return m21;
            }
            set
            {
                m21 = value;
            }
        }

        // Value at row 2, column 4 of the matrix.
        FFloat M24
        {
            get
            {
                return m31;
            }
            set
            {
                m31 = value;
            }
        }

        // Value at row 3, column 1 of the matrix.
        FFloat M31
        {
            get
            {
                return m02;
            }
            set
            {
                m02 = value;
            }
        }

        // Value at row 3, column 2 of the matrix.
        FFloat M32
        {
            get
            {
                return m12;
            }
            set
            {
                m12 = value;
            }
        }

        // Value at row 3, column 3 of the matrix.
        FFloat M33
        {
            get
            {
                return m22;
            }
            set
            {
                m22 = value;
            }
        }

        // Value at row 3, column 4 of the matrix.
        FFloat M34
        {
            get
            {
                return m32;
            }
            set
            {
                m32 = value;
            }
        }

        // Value at row 4, column 1 of the matrix.
        FFloat M41
        {
            get
            {
                return m03;
            }
            set
            {
                m03 = value;
            }
        }

        // Value at row 4, column 2 of the matrix.
        FFloat M42
        {
            get
            {
                return m13;
            }
            set
            {
                m13 = value;
            }
        }

        // Value at row 4, column 3 of the matrix.
        FFloat M43
        {
            get
            {
                return m23;
            }
            set
            {
                m23 = value;
            }
        }

        // Value at row 4, column 4 of the matrix.
        FFloat M44
        {
            get
            {
                return m33;
            }
            set
            {
                m33 = value;
            }
        }

        public FFloat this[int row, int column]
        {
            get
            {
                return this[row + column * 4];
            }
            set
            {
                this[row + column * 4] = value;
            }
        }

        public FFloat this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.m00;
                    case 1:
                        return this.m10;
                    case 2:
                        return this.m20;
                    case 3:
                        return this.m30;
                    case 4:
                        return this.m01;
                    case 5:
                        return this.m11;
                    case 6:
                        return this.m21;
                    case 7:
                        return this.m31;
                    case 8:
                        return this.m02;
                    case 9:
                        return this.m12;
                    case 10:
                        return this.m22;
                    case 11:
                        return this.m32;
                    case 12:
                        return this.m03;
                    case 13:
                        return this.m13;
                    case 14:
                        return this.m23;
                    case 15:
                        return this.m33;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.m00 = value;
                        break;
                    case 1:
                        this.m10 = value;
                        break;
                    case 2:
                        this.m20 = value;
                        break;
                    case 3:
                        this.m30 = value;
                        break;
                    case 4:
                        this.m01 = value;
                        break;
                    case 5:
                        this.m11 = value;
                        break;
                    case 6:
                        this.m21 = value;
                        break;
                    case 7:
                        this.m31 = value;
                        break;
                    case 8:
                        this.m02 = value;
                        break;
                    case 9:
                        this.m12 = value;
                        break;
                    case 10:
                        this.m22 = value;
                        break;
                    case 11:
                        this.m32 = value;
                        break;
                    case 12:
                        this.m03 = value;
                        break;
                    case 13:
                        this.m13 = value;
                        break;
                    case 14:
                        this.m23 = value;
                        break;
                    case 15:
                        this.m33 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
        }

        public FMatrix4x4 inverse
        {
            get
            {
                return FMatrix4x4.Inverse(this);
            }
        }

        /// <summary>
        ///   <para>Returns the transpose of this matrix (Read Only).</para>
        /// </summary>
        public FMatrix4x4 transpose
        {
            get
            {
                return FMatrix4x4.Transpose(this);
            }
        }

        public FFloat determinant
        {
            get
            {
                return FMatrix4x4.Determinant(this);
            }
        }

        static FMatrix4x4 _SZERO = new FMatrix4x4() { m00 = FFloat.zero, m01 = FFloat.zero, m02 = FFloat.zero, m03 = FFloat.zero, m10 = FFloat.zero, m11 = FFloat.zero, m12 = FFloat.zero, m13 = FFloat.zero, m20 = FFloat.zero, m21 = FFloat.zero, m22 = FFloat.zero, m23 = FFloat.zero, m30 = FFloat.zero, m31 = FFloat.zero, m32 = FFloat.zero, m33 = FFloat.zero };

        public static FMatrix4x4 zero
        {
            get
            {
                return _SZERO;
            }
        }

        public static FMatrix4x4 identity
        {
            get
            {
                return new FMatrix4x4() { m00 = FFloat.one, m01 = FFloat.zero, m02 = FFloat.zero, m03 = FFloat.zero, m10 = FFloat.zero, m11 = FFloat.one, m12 = FFloat.zero, m13 = FFloat.zero, m20 = FFloat.zero, m21 = FFloat.zero, m22 = FFloat.one, m23 = FFloat.zero, m30 = FFloat.zero, m31 = FFloat.zero, m32 = FFloat.zero, m33 = FFloat.one };
            }
        }

        public bool isIdentity
        {
            get
            {
                return M11 == FFloat.one && M22 == FFloat.one && M33 == FFloat.one && M44 == FFloat.one && // Check diagonal element first for early out.
                       M12 == FFloat.zero && M13 == FFloat.zero && M14 == FFloat.zero &&
                       M21 == FFloat.zero && M23 == FFloat.zero && M24 == FFloat.zero &&
                       M31 == FFloat.zero && M32 == FFloat.zero && M34 == FFloat.zero &&
                       M41 == FFloat.zero && M42 == FFloat.zero && M43 == FFloat.zero;
            }
        }

        public static FMatrix4x4 operator *(FMatrix4x4 lhs, FMatrix4x4 rhs)
        {

            FMatrix4x4 result;
            NativeFixedMath.Mat4x4_Mul(lhs, rhs, out result);
            return result;
        }

        public static FVector4 operator *(FMatrix4x4 lhs, FVector4 v)
        {
            FVector4 vector4 = FVector4.zero;
            vector4.x = (lhs.m00 * v.x + lhs.m01 * v.y + lhs.m02 * v.z + lhs.m03 * v.w);
            vector4.y = (lhs.m10 * v.x + lhs.m11 * v.y + lhs.m12 * v.z + lhs.m13 * v.w);
            vector4.z = (lhs.m20 * v.x + lhs.m21 * v.y + lhs.m22 * v.z + lhs.m23 * v.w);
            vector4.w = (lhs.m30 * v.x + lhs.m31 * v.y + lhs.m32 * v.z + lhs.m33 * v.w);
            return vector4;
        }


        public static FMatrix4x4 Inverse(FMatrix4x4 m)
        {
            FMatrix4x4 result;

            NativeFixedMath.Mat4x4_Invert(m, out result);

            return result;
        }

        public static FMatrix4x4 Transpose(FMatrix4x4 matrix)
        {
            FMatrix4x4 result = FMatrix4x4.zero;

            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M14 = matrix.M41;
            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M24 = matrix.M42;
            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
            result.M34 = matrix.M43;
            result.M41 = matrix.M14;
            result.M42 = matrix.M24;
            result.M43 = matrix.M34;
            result.M44 = matrix.M44;

            return result;
        }

        //https://stackoverflow.com/questions/1148309/inverting-a-4x4-matrix/1148405
        internal static bool Invert1(FMatrix4x4 m, out FMatrix4x4 result)
        {
            FMatrix4x4 inv = FMatrix4x4.zero;

            inv[0] = m[5] * m[10] * m[15] -
                  m[5] * m[11] * m[14] -
                  m[9] * m[6] * m[15] +
                  m[9] * m[7] * m[14] +
                  m[13] * m[6] * m[11] -
                  m[13] * m[7] * m[10];

            inv[4] = -m[4] * m[10] * m[15] +
                      m[4] * m[11] * m[14] +
                      m[8] * m[6] * m[15] -
                      m[8] * m[7] * m[14] -
                      m[12] * m[6] * m[11] +
                      m[12] * m[7] * m[10];

            inv[8] = m[4] * m[9] * m[15] -
                     m[4] * m[11] * m[13] -
                     m[8] * m[5] * m[15] +
                     m[8] * m[7] * m[13] +
                     m[12] * m[5] * m[11] -
                     m[12] * m[7] * m[9];

            inv[12] = -m[4] * m[9] * m[14] +
                       m[4] * m[10] * m[13] +
                       m[8] * m[5] * m[14] -
                       m[8] * m[6] * m[13] -
                       m[12] * m[5] * m[10] +
                       m[12] * m[6] * m[9];

            inv[1] = -m[1] * m[10] * m[15] +
                      m[1] * m[11] * m[14] +
                      m[9] * m[2] * m[15] -
                      m[9] * m[3] * m[14] -
                      m[13] * m[2] * m[11] +
                      m[13] * m[3] * m[10];

            inv[5] = m[0] * m[10] * m[15] -
                     m[0] * m[11] * m[14] -
                     m[8] * m[2] * m[15] +
                     m[8] * m[3] * m[14] +
                     m[12] * m[2] * m[11] -
                     m[12] * m[3] * m[10];

            inv[9] = -m[0] * m[9] * m[15] +
                      m[0] * m[11] * m[13] +
                      m[8] * m[1] * m[15] -
                      m[8] * m[3] * m[13] -
                      m[12] * m[1] * m[11] +
                      m[12] * m[3] * m[9];

            inv[13] = m[0] * m[9] * m[14] -
                      m[0] * m[10] * m[13] -
                      m[8] * m[1] * m[14] +
                      m[8] * m[2] * m[13] +
                      m[12] * m[1] * m[10] -
                      m[12] * m[2] * m[9];

            inv[2] = m[1] * m[6] * m[15] -
                     m[1] * m[7] * m[14] -
                     m[5] * m[2] * m[15] +
                     m[5] * m[3] * m[14] +
                     m[13] * m[2] * m[7] -
                     m[13] * m[3] * m[6];

            inv[6] = -m[0] * m[6] * m[15] +
                      m[0] * m[7] * m[14] +
                      m[4] * m[2] * m[15] -
                      m[4] * m[3] * m[14] -
                      m[12] * m[2] * m[7] +
                      m[12] * m[3] * m[6];

            inv[10] = m[0] * m[5] * m[15] -
                      m[0] * m[7] * m[13] -
                      m[4] * m[1] * m[15] +
                      m[4] * m[3] * m[13] +
                      m[12] * m[1] * m[7] -
                      m[12] * m[3] * m[5];

            inv[14] = -m[0] * m[5] * m[14] +
                       m[0] * m[6] * m[13] +
                       m[4] * m[1] * m[14] -
                       m[4] * m[2] * m[13] -
                       m[12] * m[1] * m[6] +
                       m[12] * m[2] * m[5];

            inv[3] = -m[1] * m[6] * m[11] +
                      m[1] * m[7] * m[10] +
                      m[5] * m[2] * m[11] -
                      m[5] * m[3] * m[10] -
                      m[9] * m[2] * m[7] +
                      m[9] * m[3] * m[6];

            inv[7] = m[0] * m[6] * m[11] -
                     m[0] * m[7] * m[10] -
                     m[4] * m[2] * m[11] +
                     m[4] * m[3] * m[10] +
                     m[8] * m[2] * m[7] -
                     m[8] * m[3] * m[6];

            inv[11] = -m[0] * m[5] * m[11] +
                       m[0] * m[7] * m[9] +
                       m[4] * m[1] * m[11] -
                       m[4] * m[3] * m[9] -
                       m[8] * m[1] * m[7] +
                       m[8] * m[3] * m[5];

            inv[15] = m[0] * m[5] * m[10] -
                      m[0] * m[6] * m[9] -
                      m[4] * m[1] * m[10] +
                      m[4] * m[2] * m[9] +
                      m[8] * m[1] * m[6] -
                      m[8] * m[2] * m[5];

            FFloat det = m[0] * inv[0] + m[1] * inv[4] + m[2] * inv[8] + m[3] * inv[12];

            result = FMatrix4x4.zero;

            if (det == FFloat.zero)
                return false;

            det = FFloat.one / det;

            for (int i = 0; i< 16; i++)
                result[i] = inv[i] * det;

            return true;
        }
    internal static bool Invert(FMatrix4x4 matrix, out FMatrix4x4 result)
        {
            FFloat a = matrix.M11, b = matrix.M12, c = matrix.M13, d = matrix.M14;
            FFloat e = matrix.M21, f = matrix.M22, g = matrix.M23, h = matrix.M24;
            FFloat i = matrix.M31, j = matrix.M32, k = matrix.M33, l = matrix.M34;
            FFloat m = matrix.M41, n = matrix.M42, o = matrix.M43, p = matrix.M44;

            FFloat kp_lo = k * p - l * o;
            FFloat jp_ln = j * p - l * n;
            FFloat jo_kn = j * o - k * n;
            FFloat ip_lm = i * p - l * m;
            FFloat io_km = i * o - k * m;
            FFloat in_jm = i * n - j * m;

            FFloat a11 = (f * kp_lo - g * jp_ln + h * jo_kn);
            FFloat a12 = -(e * kp_lo - g * ip_lm + h * io_km);
            FFloat a13 = (e * jp_ln - f * ip_lm + h * in_jm);
            FFloat a14 = -(e * jo_kn - f * io_km + g * in_jm);

            FFloat det = a * a11 + b * a12 + c * a13 + d * a14;

            if(det == FFloat.zero)
            {
                result = FMatrix4x4.zero;
                return false;
            }


            FFloat invDet = FFloat.one / det;

            result = FMatrix4x4.zero;

            result.M11 = a11 * invDet;
            result.M21 = a12 * invDet;
            result.M31 = a13 * invDet;
            result.M41 = a14 * invDet;

            result.M12 = -(b * kp_lo - c * jp_ln + d * jo_kn) * invDet;
            result.M22 = (a * kp_lo - c * ip_lm + d * io_km) * invDet;
            result.M32 = -(a * jp_ln - b * ip_lm + d * in_jm) * invDet;
            result.M42 = (a * jo_kn - b * io_km + c * in_jm) * invDet;

            FFloat gp_ho = g * p - h * o;
            FFloat fp_hn = f * p - h * n;
            FFloat fo_gn = f * o - g * n;
            FFloat ep_hm = e * p - h * m;
            FFloat eo_gm = e * o - g * m;
            FFloat en_fm = e * n - f * m;

            result.M13 = (b * gp_ho - c * fp_hn + d * fo_gn) * invDet;
            result.M23 = -(a * gp_ho - c * ep_hm + d * eo_gm) * invDet;
            result.M33 = (a * fp_hn - b * ep_hm + d * en_fm) * invDet;
            result.M43 = -(a * fo_gn - b * eo_gm + c * en_fm) * invDet;

            FFloat gl_hk = g * l - h * k;
            FFloat fl_hj = f * l - h * j;
            FFloat fk_gj = f * k - g * j;
            FFloat el_hi = e * l - h * i;
            FFloat ek_gi = e * k - g * i;
            FFloat ej_fi = e * j - f * i;

            result.M14 = -(b * gl_hk - c * fl_hj + d * fk_gj) * invDet;
            result.M24 = (a * gl_hk - c * el_hi + d * ek_gi) * invDet;
            result.M34 = -(a * fl_hj - b * el_hi + d * ej_fi) * invDet;
            result.M44 = (a * fk_gj - b * ek_gi + c * ej_fi) * invDet;

            return true;
        }
        
        public static FFloat Determinant(FMatrix4x4 matrix)
        {
            FFloat a = matrix.M11, b = matrix.M12, c = matrix.M13, d = matrix.M14;
            FFloat e = matrix.M21, f = matrix.M22, g = matrix.M23, h = matrix.M24;
            FFloat i = matrix.M31, j = matrix.M32, k = matrix.M33, l = matrix.M34;
            FFloat m = matrix.M41, n = matrix.M42, o = matrix.M43, p = matrix.M44;

            FFloat kp_lo = k * p - l * o;
            FFloat jp_ln = j * p - l * n;
            FFloat jo_kn = j * o - k * n;
            FFloat ip_lm = i * p - l * m;
            FFloat io_km = i * o - k * m;
            FFloat in_jm = i * n - j * m;

            return a * (f * kp_lo - g * jp_ln + h * jo_kn) -
                   b * (e * kp_lo - g * ip_lm + h * io_km) +
                   c * (e * jp_ln - f * ip_lm + h * in_jm) -
                   d * (e * jo_kn - f * io_km + g * in_jm);
        }

        public FVector4 GetColumn(int i)
        {
            return new FVector4(this[0, i], this[1, i], this[2, i], this[3, i]);
        }

        public void SetColumn(int i, FVector4 v)
        {
            this[0, i] = v.x;
            this[1, i] = v.y;
            this[2, i] = v.z;
            this[3, i] = v.w;
        }

        public FVector4 GetRow(int i)
        {
            return new FVector4(this[i, 0], this[i, 1], this[i, 2], this[i, 3]);
        }

        public void SetRow(int i, FVector4 v)
        {
            this[i, 0] = v.x;
            this[i, 1] = v.y;
            this[i, 2] = v.z;
            this[i, 3] = v.w;
        }

        /// <summary>
        ///   <para>Transforms a position by this matrix (generic).</para>
        /// </summary>
        /// <param name="v"></param>
        public FVector3 MultiplyPoint(FVector3 v)
        {
            FVector3 vector3 = FVector3.zero;
            vector3.x = (this.m00 * v.x + this.m01 * v.y + this.m02 * v.z) + this.m03;
            vector3.y = (this.m10 * v.x + this.m11 * v.y + this.m12 * v.z) + this.m13;
            vector3.z = (this.m20 * v.x + this.m21 * v.y + this.m22 * v.z) + this.m23;
            FFloat num = FFloat.one / ((this.m30 * v.x + this.m31 * v.y + this.m32 * v.z) + this.m33);
            vector3.x *= num;
            vector3.y *= num;
            vector3.z *= num;
            return vector3;
        }

        /// <summary>
        ///   <para>Transforms a position by this matrix (fast).</para>
        /// </summary>
        /// <param name="v"></param>
        public FVector3 MultiplyPoint3x4(FVector3 v)
        {
            FVector3 vector3 = FVector3.zero;
            NativeFixedMath.Mat4x4_Mul_Point_3x4(this, v, ref vector3);
            return vector3;
        }

        /// <summary>
        ///   <para>Transforms a direction by this matrix.</para>
        /// </summary>
        /// <param name="v"></param>
        public FVector3 MultiplyVector(FVector3 v)
        {
            FVector3 vector3 = FVector3.zero;
            vector3.x = (this.m00 * v.x + this.m01 * v.y + this.m02 * v.z);
            vector3.y = (this.m10 * v.x + this.m11 * v.y + this.m12 * v.z);
            vector3.z = (this.m20 * v.x + this.m21 * v.y + this.m22 * v.z);
            return vector3;
        }

        /// <summary>
        ///   <para>Creates a scaling matrix.</para>
        /// </summary>
        /// <param name="v"></param>
        public static FMatrix4x4 Scale(FVector3 v)
        {
            return new FMatrix4x4() { m00 = v.x, m01 = FFloat.zero, m02 = FFloat.zero, m03 = FFloat.zero, m10 = FFloat.zero, m11 = v.y, m12 = FFloat.zero, m13 = FFloat.zero, m20 = FFloat.zero, m21 = FFloat.zero, m22 = v.z, m23 = FFloat.zero, m30 = FFloat.zero, m31 = FFloat.zero, m32 = FFloat.zero, m33 = FFloat.one };
        }

        /// <summary>
        ///   <para>Sets this matrix to a translation, rotation and scaling matrix.</para>
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="q"></param>
        /// <param name="s"></param>
        public void SetTRS(FVector3 pos, FQuaternion q, FVector3 s)
        {
            this = FMatrix4x4.TRS(pos, q, s);
        }

        /// <summary>
        ///   <para>Creates a translation, rotation and scaling matrix.</para>
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="q"></param>
        /// <param name="s"></param>
        public static FMatrix4x4 TRS(FVector3 pos, FQuaternion q, FVector3 s)
        {
            FMatrix4x4 result;

            NativeFixedMath.Mat4x4_TRS(pos, q, s, out result);

            return result;
        }

        /// <summary>
        ///   <para>Returns a nicely formatted string for this matrix.</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString()
        {
            return $"{m00}\t{m01}\t{m02}\t{m03}\n{m10}\t{m11}\t{m12}\t{m13}\n{m20}\t{m21}\t{m22}\t{m23}\n{m30}\t{m31}\t{m32}\t{m33}\n";
        }
        
        /// <summary>
        ///   <para>Creates an orthogonal projection matrix.</para>
        /// </summary>
        /// <param name="width">Width of the view volume.</param>
        /// <param name="height">Height of the view volume.</param>
        /// <param name="zNearPlane">Minimum Z-value of the view volume.</param>
        /// <param name="zFarPlane">Maximum Z-value of the view volume.</param>
        /// <returns>The orthographic projection matrix.</returns>
        public static FMatrix4x4 Ortho(FFloat width, FFloat height, FFloat zNearPlane, FFloat zFarPlane)
        {
            FMatrix4x4 result = FMatrix4x4.zero;

            result.M11 = FFloat.two / width;
            result.M12 = result.M13 = result.M14 = FFloat.zero;

            result.M22 = FFloat.two / height;
            result.M21 = result.M23 = result.M24 = FFloat.zero;

            result.M33 = FFloat.one / (zNearPlane - zFarPlane);
            result.M31 = result.M32 = result.M34 = FFloat.zero;

            result.M41 = result.M42 = FFloat.zero;
            result.M43 = zNearPlane / (zNearPlane - zFarPlane);
            result.M44 = FFloat.one;

            return result;
        }
        
        /// <summary>
        ///   <para>Creates a perspective projection matrix.</para>
        /// </summary>
        /// <param name="fov"></param>
        /// <param name="aspect"></param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        public static FMatrix4x4 Perspective(FFloat fieldOfView, FFloat aspectRatio, FFloat nearPlaneDistance, FFloat farPlaneDistance)
        {
            FMatrix4x4 result = FMatrix4x4.zero;

            FFloat rad = fieldOfView * FMath.Deg2Rad;

            FFloat yScale = FFloat.one / FMath.Tan(rad * FFloat.half);
            FFloat xScale = yScale / aspectRatio;

            result.M11 = xScale;
            result.M12 = result.M13 = result.M14 = FFloat.zero;

            result.M22 = yScale;
            result.M21 = result.M23 = result.M24 = FFloat.zero;

            result.M31 = result.M32 = FFloat.zero;
            result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.M34 = FFloat.negOne;

            result.M41 = result.M42 = result.M44 = FFloat.zero;
            result.M43 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);

            return result;
        }

        public static explicit operator Matrix4x4(FMatrix4x4 value)
        {
            return new Matrix4x4(
                            (Vector4)value.GetColumn(0),
                            (Vector4)value.GetColumn(1),
                            (Vector4)value.GetColumn(2),
                            (Vector4)value.GetColumn(3));
        }


        public static bool operator ==(FMatrix4x4 lhs, FMatrix4x4 rhs)
        {
            if (lhs.GetColumn(0) == rhs.GetColumn(0) && lhs.GetColumn(1) == rhs.GetColumn(1) && lhs.GetColumn(2) == rhs.GetColumn(2))
                return lhs.GetColumn(3) == rhs.GetColumn(3);
            return false;
        }

        public static bool operator !=(FMatrix4x4 lhs, FMatrix4x4 rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return this.GetColumn(0).GetHashCode() ^ this.GetColumn(1).GetHashCode() << 2 ^ this.GetColumn(2).GetHashCode() >> 2 ^ this.GetColumn(3).GetHashCode() >> 1;
        }

        public override bool Equals(object other)
        {
            if (!(other is FMatrix4x4))
                return false;
            FMatrix4x4 matrix4x4 = (FMatrix4x4)other;
            if (this == matrix4x4)
            {
                return true;
            }
            return false;
        }

    }
}
