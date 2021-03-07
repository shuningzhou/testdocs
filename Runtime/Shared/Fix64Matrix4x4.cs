using System;
using UnityEngine;

namespace Parallel
{
    [Serializable]
    //column-major order.
    public struct Fix64Matrix4X4
    {
        //https://github.com/microsoft/referencesource/blob/master/System.Numerics/System/Numerics/Matrix4x4.cs
        public Fix64 m00;
        public Fix64 m10;
        public Fix64 m20;
        public Fix64 m30;
        public Fix64 m01;
        public Fix64 m11;
        public Fix64 m21;
        public Fix64 m31;
        public Fix64 m02;
        public Fix64 m12;
        public Fix64 m22;
        public Fix64 m32;
        public Fix64 m03;
        public Fix64 m13;
        public Fix64 m23;
        public Fix64 m33;

        //https://referencesource.microsoft.com/#System.Numerics/System/Numerics/Matrix4x4.cs
        // Value at row 1, column 1 of the matrix.
        Fix64 M11
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
        Fix64 M12
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
        Fix64 M13
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
        Fix64 M14
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
        Fix64 M21
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
        Fix64 M22
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
        Fix64 M23
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
        Fix64 M24
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
        Fix64 M31
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
        Fix64 M32
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
        Fix64 M33
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
        Fix64 M34
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
        Fix64 M41
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
        Fix64 M42
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
        Fix64 M43
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
        Fix64 M44
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

        public Fix64 this[int row, int column]
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

        public Fix64 this[int index]
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

        public static Fix64Matrix4X4 zero
        {
            get
            {
                return new Fix64Matrix4X4() { m00 = Fix64.zero, m01 = Fix64.zero, m02 = Fix64.zero, m03 = Fix64.zero, m10 = Fix64.zero, m11 = Fix64.zero, m12 = Fix64.zero, m13 = Fix64.zero, m20 = Fix64.zero, m21 = Fix64.zero, m22 = Fix64.zero, m23 = Fix64.zero, m30 = Fix64.zero, m31 = Fix64.zero, m32 = Fix64.zero, m33 = Fix64.zero };
            }
        }

        public static Fix64Matrix4X4 identity
        {
            get
            {
                return new Fix64Matrix4X4() { m00 = Fix64.one, m01 = Fix64.zero, m02 = Fix64.zero, m03 = Fix64.zero, m10 = Fix64.zero, m11 = Fix64.one, m12 = Fix64.zero, m13 = Fix64.zero, m20 = Fix64.zero, m21 = Fix64.zero, m22 = Fix64.one, m23 = Fix64.zero, m30 = Fix64.zero, m31 = Fix64.zero, m32 = Fix64.zero, m33 = Fix64.one };
            }
        }

        public static Fix64Matrix4X4 operator *(Fix64Matrix4X4 lhs, Fix64Matrix4X4 rhs)
        {
            return new Fix64Matrix4X4() {
                m00 = (lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30),
                m01 = (lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31),
                m02 = (lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32),
                m03 = (lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33),
                m10 = (lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30),
                m11 = (lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31),
                m12 = (lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32),
                m13 = (lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33),
                m20 = (lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30),
                m21 = (lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31),
                m22 = (lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32),
                m23 = (lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33),
                m30 = (lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30),
                m31 = (lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31),
                m32 = (lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32),
                m33 = (lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33)
            };
        }

        public static Fix64Vec4 operator *(Fix64Matrix4X4 lhs, Fix64Vec4 v)
        {
            Fix64Vec4 vector4 = Fix64Vec4.zero;
            vector4.x = (lhs.m00 * v.x + lhs.m01 * v.y + lhs.m02 * v.z + lhs.m03 * v.w);
            vector4.y = (lhs.m10 * v.x + lhs.m11 * v.y + lhs.m12 * v.z + lhs.m13 * v.w);
            vector4.z = (lhs.m20 * v.x + lhs.m21 * v.y + lhs.m22 * v.z + lhs.m23 * v.w);
            vector4.w = (lhs.m30 * v.x + lhs.m31 * v.y + lhs.m32 * v.z + lhs.m33 * v.w);
            return vector4;
        }


        public static Fix64Matrix4X4 Inverse(Fix64Matrix4X4 m)
        {
            Fix64Matrix4X4 result;
            Invert1(m, out result);

            return result;
        }

        public static Fix64Matrix4X4 Transpose(Fix64Matrix4X4 matrix)
        {
            Fix64Matrix4X4 result = Fix64Matrix4X4.zero;

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

        internal static bool Invert1(Fix64Matrix4X4 m, out Fix64Matrix4X4 result)
        {
            Fix64Matrix4X4 inv = Fix64Matrix4X4.zero;

            inv[0] = m[5]  * m[10] * m[15] - 
                     m[5]  * m[11] * m[14] - 
                     m[9]  * m[6]  * m[15] + 
                     m[9]  * m[7]  * m[14] +
                     m[13] * m[6]  * m[11] - 
                     m[13] * m[7]  * m[10];

            inv[4] = -m[4]  * m[10] * m[15] + 
                      m[4]  * m[11] * m[14] + 
                      m[8]  * m[6]  * m[15] - 
                      m[8]  * m[7]  * m[14] - 
                      m[12] * m[6]  * m[11] + 
                      m[12] * m[7]  * m[10];

            inv[8] = m[4]  * m[9] * m[15] - 
                     m[4]  * m[11] * m[13] - 
                     m[8]  * m[5] * m[15] + 
                     m[8]  * m[7] * m[13] + 
                     m[12] * m[5] * m[11] - 
                     m[12] * m[7] * m[9];

            inv[12] = -m[4]  * m[9] * m[14] + 
                       m[4]  * m[10] * m[13] +
                       m[8]  * m[5] * m[14] - 
                       m[8]  * m[6] * m[13] - 
                       m[12] * m[5] * m[10] + 
                       m[12] * m[6] * m[9];

            inv[1] = -m[1]  * m[10] * m[15] + 
                      m[1]  * m[11] * m[14] + 
                      m[9]  * m[2] * m[15] - 
                      m[9]  * m[3] * m[14] - 
                      m[13] * m[2] * m[11] + 
                      m[13] * m[3] * m[10];

            inv[5] = m[0]  * m[10] * m[15] - 
                     m[0]  * m[11] * m[14] - 
                     m[8]  * m[2] * m[15] + 
                     m[8]  * m[3] * m[14] + 
                     m[12] * m[2] * m[11] - 
                     m[12] * m[3] * m[10];

            inv[9] = -m[0]  * m[9] * m[15] + 
                      m[0]  * m[11] * m[13] + 
                      m[8]  * m[1] * m[15] - 
                      m[8]  * m[3] * m[13] - 
                      m[12] * m[1] * m[11] + 
                      m[12] * m[3] * m[9];

            inv[13] = m[0]  * m[9] * m[14] - 
                      m[0]  * m[10] * m[13] - 
                      m[8]  * m[1] * m[14] + 
                      m[8]  * m[2] * m[13] + 
                      m[12] * m[1] * m[10] - 
                      m[12] * m[2] * m[9];

            inv[2] = m[1]  * m[6] * m[15] - 
                     m[1]  * m[7] * m[14] - 
                     m[5]  * m[2] * m[15] + 
                     m[5]  * m[3] * m[14] + 
                     m[13] * m[2] * m[7] - 
                     m[13] * m[3] * m[6];

            inv[6] = -m[0]  * m[6] * m[15] + 
                      m[0]  * m[7] * m[14] + 
                      m[4]  * m[2] * m[15] - 
                      m[4]  * m[3] * m[14] - 
                      m[12] * m[2] * m[7] + 
                      m[12] * m[3] * m[6];

            inv[10] = m[0]  * m[5] * m[15] - 
                      m[0]  * m[7] * m[13] - 
                      m[4]  * m[1] * m[15] + 
                      m[4]  * m[3] * m[13] + 
                      m[12] * m[1] * m[7] - 
                      m[12] * m[3] * m[5];

            inv[14] = -m[0]  * m[5] * m[14] + 
                       m[0]  * m[6] * m[13] + 
                       m[4]  * m[1] * m[14] - 
                       m[4]  * m[2] * m[13] - 
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

            Fix64 det = m[0] * inv[0] + m[1] * inv[4] + m[2] * inv[8] + m[3] * inv[12];

            result = Fix64Matrix4X4.zero;

            if (det == Fix64.zero)
                return false;

            det = Fix64.one / det;

            for (int i = 0; i< 16; i++)
                result[i] = inv[i] * det;

            return true;
        }
    internal static bool Invert(Fix64Matrix4X4 matrix, out Fix64Matrix4X4 result)
        {
            Fix64 a = matrix.M11, b = matrix.M12, c = matrix.M13, d = matrix.M14;
            Fix64 e = matrix.M21, f = matrix.M22, g = matrix.M23, h = matrix.M24;
            Fix64 i = matrix.M31, j = matrix.M32, k = matrix.M33, l = matrix.M34;
            Fix64 m = matrix.M41, n = matrix.M42, o = matrix.M43, p = matrix.M44;

            Fix64 kp_lo = k * p - l * o;
            Fix64 jp_ln = j * p - l * n;
            Fix64 jo_kn = j * o - k * n;
            Fix64 ip_lm = i * p - l * m;
            Fix64 io_km = i * o - k * m;
            Fix64 in_jm = i * n - j * m;

            Fix64 a11 = (f * kp_lo - g * jp_ln + h * jo_kn);
            Fix64 a12 = -(e * kp_lo - g * ip_lm + h * io_km);
            Fix64 a13 = (e * jp_ln - f * ip_lm + h * in_jm);
            Fix64 a14 = -(e * jo_kn - f * io_km + g * in_jm);

            Fix64 det = a * a11 + b * a12 + c * a13 + d * a14;

            if(det == Fix64.zero)
            {
                result = Fix64Matrix4X4.zero;
                return false;
            }


            Fix64 invDet = Fix64.one / det;

            result = Fix64Matrix4X4.zero;

            result.M11 = a11 * invDet;
            result.M21 = a12 * invDet;
            result.M31 = a13 * invDet;
            result.M41 = a14 * invDet;

            result.M12 = -(b * kp_lo - c * jp_ln + d * jo_kn) * invDet;
            result.M22 = (a * kp_lo - c * ip_lm + d * io_km) * invDet;
            result.M32 = -(a * jp_ln - b * ip_lm + d * in_jm) * invDet;
            result.M42 = (a * jo_kn - b * io_km + c * in_jm) * invDet;

            Fix64 gp_ho = g * p - h * o;
            Fix64 fp_hn = f * p - h * n;
            Fix64 fo_gn = f * o - g * n;
            Fix64 ep_hm = e * p - h * m;
            Fix64 eo_gm = e * o - g * m;
            Fix64 en_fm = e * n - f * m;

            result.M13 = (b * gp_ho - c * fp_hn + d * fo_gn) * invDet;
            result.M23 = -(a * gp_ho - c * ep_hm + d * eo_gm) * invDet;
            result.M33 = (a * fp_hn - b * ep_hm + d * en_fm) * invDet;
            result.M43 = -(a * fo_gn - b * eo_gm + c * en_fm) * invDet;

            Fix64 gl_hk = g * l - h * k;
            Fix64 fl_hj = f * l - h * j;
            Fix64 fk_gj = f * k - g * j;
            Fix64 el_hi = e * l - h * i;
            Fix64 ek_gi = e * k - g * i;
            Fix64 ej_fi = e * j - f * i;

            result.M14 = -(b * gl_hk - c * fl_hj + d * fk_gj) * invDet;
            result.M24 = (a * gl_hk - c * el_hi + d * ek_gi) * invDet;
            result.M34 = -(a * fl_hj - b * el_hi + d * ej_fi) * invDet;
            result.M44 = (a * fk_gj - b * ek_gi + c * ej_fi) * invDet;

            return true;
        }
        
        public Fix64 Determinant()
        {
            Fix64 a = M11, b = M12, c = M13, d = M14;
            Fix64 e = M21, f = M22, g = M23, h = M24;
            Fix64 i = M31, j = M32, k = M33, l = M34;
            Fix64 m = M41, n = M42, o = M43, p = M44;

            Fix64 kp_lo = k * p - l * o;
            Fix64 jp_ln = j * p - l * n;
            Fix64 jo_kn = j * o - k * n;
            Fix64 ip_lm = i * p - l * m;
            Fix64 io_km = i * o - k * m;
            Fix64 in_jm = i * n - j * m;

            return a * (f * kp_lo - g * jp_ln + h * jo_kn) -
                   b * (e * kp_lo - g * ip_lm + h * io_km) +
                   c * (e * jp_ln - f * ip_lm + h * in_jm) -
                   d * (e * jo_kn - f * io_km + g * in_jm);
        }

        public Fix64Vec4 GetColumn(int i)
        {
            return new Fix64Vec4(this[0, i], this[1, i], this[2, i], this[3, i]);
        }

        public void SetColumn(int i, Fix64Vec4 v)
        {
            this[0, i] = v.x;
            this[1, i] = v.y;
            this[2, i] = v.z;
            this[3, i] = v.w;
        }

        public Fix64Vec4 GetRow(int i)
        {
            return new Fix64Vec4(this[i, 0], this[i, 1], this[i, 2], this[i, 3]);
        }

        public void SetRow(int i, Fix64Vec4 v)
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
        public Fix64Vec3 MultiplyPoint(Fix64Vec3 v)
        {
            Fix64Vec3 vector3 = Fix64Vec3.zero;
            vector3.x = (this.m00 * v.x + this.m01 * v.y + this.m02 * v.z) + this.m03;
            vector3.y = (this.m10 * v.x + this.m11 * v.y + this.m12 * v.z) + this.m13;
            vector3.z = (this.m20 * v.x + this.m21 * v.y + this.m22 * v.z) + this.m23;
            Fix64 num = Fix64.one / ((this.m30 * v.x + this.m31 * v.y + this.m32 * v.z) + this.m33);
            vector3.x *= num;
            vector3.y *= num;
            vector3.z *= num;
            return vector3;
        }

        /// <summary>
        ///   <para>Transforms a position by this matrix (fast).</para>
        /// </summary>
        /// <param name="v"></param>
        public Fix64Vec3 MultiplyPoint3x4(Fix64Vec3 v)
        {
            Fix64Vec3 vector3 = Fix64Vec3.zero;
            vector3.x = (this.m00 * v.x + this.m01 * v.y + this.m02 * v.z) + this.m03;
            vector3.y = (this.m10 * v.x + this.m11 * v.y + this.m12 * v.z) + this.m13;
            vector3.z = (this.m20 * v.x + this.m21 * v.y + this.m22 * v.z) + this.m23;
            return vector3;
        }

        /// <summary>
        ///   <para>Transforms a direction by this matrix.</para>
        /// </summary>
        /// <param name="v"></param>
        public Fix64Vec3 MultiplyVector(Fix64Vec3 v)
        {
            Fix64Vec3 vector3 = Fix64Vec3.zero;
            vector3.x = (this.m00 * v.x + this.m01 * v.y + this.m02 * v.z);
            vector3.y = (this.m10 * v.x + this.m11 * v.y + this.m12 * v.z);
            vector3.z = (this.m20 * v.x + this.m21 * v.y + this.m22 * v.z);
            return vector3;
        }

        /// <summary>
        ///   <para>Creates a scaling matrix.</para>
        /// </summary>
        /// <param name="v"></param>
        public static Fix64Matrix4X4 Scale(Fix64Vec3 v)
        {
            return new Fix64Matrix4X4() { m00 = v.x, m01 = Fix64.zero, m02 = Fix64.zero, m03 = Fix64.zero, m10 = Fix64.zero, m11 = v.y, m12 = Fix64.zero, m13 = Fix64.zero, m20 = Fix64.zero, m21 = Fix64.zero, m22 = v.z, m23 = Fix64.zero, m30 = Fix64.zero, m31 = Fix64.zero, m32 = Fix64.zero, m33 = Fix64.one };
        }

        /// <summary>
        ///   <para>Sets this matrix to a translation, rotation and scaling matrix.</para>
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="q"></param>
        /// <param name="s"></param>
        public void SetTRS(Fix64Vec3 pos, Fix64Quat q, Fix64Vec3 s)
        {
            this = Fix64Matrix4X4.TRS(pos, q, s);
        }

        /// <summary>
        ///   <para>Creates a translation, rotation and scaling matrix.</para>
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="q"></param>
        /// <param name="s"></param>
        public static Fix64Matrix4X4 TRS(Fix64Vec3 pos, Fix64Quat q, Fix64Vec3 s)
        {
            Fix64Matrix4X4 result = Fix64Matrix4X4.zero;

            Fix64 x1 = pos.x;
            Fix64 y1 = pos.y;
            Fix64 z1 = pos.z;
            Fix64 x2 = s.x;
            Fix64 y2 = s.y;
            Fix64 z2 = s.z;

            Fix64Mat3x3 rot = new Fix64Mat3x3(q);
            Fix64 a11 = rot.x.x;
            Fix64 a21 = rot.x.y;
            Fix64 a31 = rot.x.z;
            Fix64 a12 = rot.y.x;
            Fix64 a22 = rot.y.y;
            Fix64 a32 = rot.y.z;
            Fix64 a13 = rot.z.x;
            Fix64 a23 = rot.z.y;
            Fix64 a33 = rot.z.z;

            result.M11 = x2 * a11;
            result.M12 = x2 * a21;
            result.M13 = x2 * a31;
            result.M14 = Fix64.zero;
            result.M21 = y2 * a12;
            result.M22 = y2 * a22;
            result.M23 = y2 * a32;
            result.M24 = Fix64.zero;
            result.M31 = z2 * a13;
            result.M32 = z2 * a23;
            result.M33 = z2 * a33;
            result.M34 = Fix64.zero;
            result.M41 = x1;
            result.M42 = y1;
            result.M43 = z1;
            result.M44 = Fix64.one;

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
        public static Fix64Matrix4X4 Ortho(Fix64 width, Fix64 height, Fix64 zNearPlane, Fix64 zFarPlane)
        {
            Fix64Matrix4X4 result = Fix64Matrix4X4.zero;

            result.M11 = Fix64.two / width;
            result.M12 = result.M13 = result.M14 = Fix64.zero;

            result.M22 = Fix64.two / height;
            result.M21 = result.M23 = result.M24 = Fix64.zero;

            result.M33 = Fix64.one / (zNearPlane - zFarPlane);
            result.M31 = result.M32 = result.M34 = Fix64.zero;

            result.M41 = result.M42 = Fix64.zero;
            result.M43 = zNearPlane / (zNearPlane - zFarPlane);
            result.M44 = Fix64.one;

            return result;
        }
        
        /// <summary>
        ///   <para>Creates a perspective projection matrix.</para>
        /// </summary>
        /// <param name="fov"></param>
        /// <param name="aspect"></param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        public static Fix64Matrix4X4 Perspective(Fix64 fieldOfView, Fix64 aspectRatio, Fix64 nearPlaneDistance, Fix64 farPlaneDistance)
        {
            Fix64Matrix4X4 result = Fix64Matrix4X4.zero;

            Fix64 rad = fieldOfView * Fix64Math.DegreeToRad;

            Fix64 yScale = Fix64.one / Fix64Math.Tan(rad * Fix64.half);
            Fix64 xScale = yScale / aspectRatio;

            result.M11 = xScale;
            result.M12 = result.M13 = result.M14 = Fix64.zero;

            result.M22 = yScale;
            result.M21 = result.M23 = result.M24 = Fix64.zero;

            result.M31 = result.M32 = Fix64.zero;
            result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.M34 = Fix64.negOne;

            result.M41 = result.M42 = result.M44 = Fix64.zero;
            result.M43 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);

            return result;
        }

        public static explicit operator Matrix4x4(Fix64Matrix4X4 value)
        {
            return new Matrix4x4(
                            (Vector4)value.GetColumn(0),
                            (Vector4)value.GetColumn(1),
                            (Vector4)value.GetColumn(2),
                            (Vector4)value.GetColumn(3));
        }

    }
}
