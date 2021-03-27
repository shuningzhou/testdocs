using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Parallel
{
    public struct FVector4
    {
        public static FVector4 zero { get { return new FVector4(FFloat.zero, FFloat.zero, FFloat.zero, FFloat.zero); } }
        public static FVector4 one { get { return new FVector4(FFloat.one, FFloat.one, FFloat.one, FFloat.one); } }

        public long RawX;
        public long RawY;
        public long RawZ;
        public long RawW;

        public FFloat x { get { return FFloat.FromRaw(RawX); } set { RawX = value.Raw; } }
        public FFloat y { get { return FFloat.FromRaw(RawY); } set { RawY = value.Raw; } }
        public FFloat z { get { return FFloat.FromRaw(RawZ); } set { RawZ = value.Raw; } }
        public FFloat w { get { return FFloat.FromRaw(RawW); } set { RawW = value.Raw; } }

        public FFloat this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;
                    case 1:
                        return this.y;
                    case 2:
                        return this.z;
                    case 3:
                        return this.w;
                    default:
                        throw new IndexOutOfRangeException("Invalid FVector4 index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    case 2:
                        this.z = value;
                        break;
                    case 3:
                        this.w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid FVector4 index!");
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FVector4 FromRaw(long rawValueX, long rawValueY, long rawValueZ, long rawValueW)
        {
            FVector4 r;
            r.RawX = rawValueX;
            r.RawY = rawValueY;
            r.RawZ = rawValueZ;
            r.RawW = rawValueW;
            return r;
        }

        public FVector4(FFloat x, FFloat y, FFloat z, FFloat w)
        {
            RawX = x.Raw;
            RawY = y.Raw;
            RawZ = z.Raw;
            RawW = w.Raw;
        }

        public FVector4(FFloat x, FFloat y, FFloat z)
        {
            RawX = x.Raw;
            RawY = y.Raw;
            RawZ = z.Raw;
            RawW = 0;
        }

        public FVector4(FFloat x, FFloat y)
        {
            RawX = x.Raw;
            RawY = y.Raw;
            RawZ = 0;
            RawW = 0;
        }

        public FVector4 normalized
        {
            get
            {
                return FVector4.Normalize(this);
            }
        }

        /// <summary>
        ///   <para>Returns the length of this vector (Read Only).</para>
        /// </summary>
        public FFloat magnitude
        {
            get
            {
                return FMath.Sqrt(FVector4.Dot(this, this));
            }
        }

        /// <summary>
        ///   <para>Returns the squared length of this vector (Read Only).</para>
        /// </summary>
        public FFloat sqrMagnitude
        {
            get
            {
                return FVector4.Dot(this, this);
            }
        }


        public static explicit operator Vector4(FVector4 value)
        {
            return new Vector4((float)value.x, (float)value.y, (float)value.z, (float)value.w);
        }


        public static FVector4 operator -(FVector4 a)
        {
            return FromRaw(-a.RawX, -a.RawY, -a.RawZ, -a.RawW);
        }

        public static FVector4 operator +(FVector4 a, FVector4 b)
        {
            return FromRaw(a.RawX + b.RawX, a.RawY + b.RawY, a.RawZ + b.RawZ, a.RawW + b.RawW);
        }

        public static FVector4 operator -(FVector4 a, FVector4 b)
        {
            return FromRaw(a.RawX - b.RawX, a.RawY - b.RawY, a.RawZ - b.RawZ, a.RawW - b.RawW);
        }

        public static FVector4 operator *(FVector4 a, FVector4 b)
        {
            return FromRaw(NativeFixedMath.Mul64(a.RawX, b.RawX), NativeFixedMath.Mul64(a.RawY, b.RawY), NativeFixedMath.Mul64(a.RawZ, b.RawZ), NativeFixedMath.Mul64(a.RawW, b.RawW));
        }

        public static FVector4 operator /(FVector4 a, FVector4 b)
        {
            return FromRaw(NativeFixedMath.Div64(a.RawX, b.RawX), NativeFixedMath.Div64(a.RawY, b.RawY), NativeFixedMath.Div64(a.RawZ, b.RawZ), NativeFixedMath.Div64(a.RawW, b.RawW));
        }

        public static FVector4 operator +(FFloat a, FVector4 b)
        {
            return FromRaw(a.Raw + b.RawX, a.Raw + b.RawY, a.Raw + b.RawZ, a.Raw + b.RawW);
        }

        public static FVector4 operator +(FVector4 a, FFloat b)
        {
            return FromRaw(a.RawX + b.Raw, a.RawY + b.Raw, a.RawZ + b.Raw, a.RawW + b.Raw);
        }

        public static FVector4 operator -(FFloat a, FVector4 b)
        {
            return FromRaw(a.Raw - b.RawX, a.Raw - b.RawY, a.Raw - b.RawZ, a.Raw - b.RawW);
        }

        public static FVector4 operator -(FVector4 a, FFloat b)
        {
            return FromRaw(a.RawX - b.Raw, a.RawY - b.Raw, a.RawZ - b.Raw, a.RawW - b.Raw);
        }

        public static FVector4 operator *(FFloat a, FVector4 b)
        {
            return FromRaw(NativeFixedMath.Mul64(a.Raw, b.RawX), NativeFixedMath.Mul64(a.Raw, b.RawY), NativeFixedMath.Mul64(a.Raw, b.RawZ), NativeFixedMath.Mul64(a.Raw, b.RawW));
        }

        public static FVector4 operator *(FVector4 a, FFloat b)
        {
            return FromRaw(NativeFixedMath.Mul64(a.RawX, b.Raw), NativeFixedMath.Mul64(a.RawY, b.Raw), NativeFixedMath.Mul64(a.RawZ, b.Raw), NativeFixedMath.Mul64(a.RawW, b.Raw));
        }

        public static FVector4 operator /(FFloat a, FVector4 b)
        {
            return FromRaw(NativeFixedMath.Div64(a.Raw, b.RawX), NativeFixedMath.Div64(a.Raw, b.RawY), NativeFixedMath.Div64(a.Raw, b.RawZ), NativeFixedMath.Div64(a.Raw, b.RawW));
        }

        public static FVector4 operator /(FVector4 a, FFloat b)
        {
            return FromRaw(NativeFixedMath.Div64(a.RawX, b.Raw), NativeFixedMath.Div64(a.RawY, b.Raw), NativeFixedMath.Div64(a.RawZ, b.Raw), NativeFixedMath.Div64(a.RawW, b.Raw));
        }

        public static bool operator ==(FVector4 a, FVector4 b)
        {
            return a.RawX == b.RawX && a.RawY == b.RawY && a.RawZ == b.RawZ && a.RawW == b.RawW;
        }

        public static bool operator !=(FVector4 a, FVector4 b)
        {
            return a.RawX != b.RawX || a.RawY != b.RawY || a.RawZ != b.RawZ || a.RawW != b.RawW;
        }

        public void Set(FFloat new_x, FFloat new_y, FFloat new_z, FFloat new_w)
        {
            this.x = new_x;
            this.y = new_y;
            this.z = new_z;
            this.w = new_w;
        }

        public override string ToString()
        {
            return $"Fix64Vec4({x}, {y}, {z}, {w})";
        }

        public static FVector4 Lerp(FVector4 a, FVector4 b, FFloat t)
        {
            t = FMath.Clamp01(t);
            return new FVector4(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);
        }

        public static FVector4 LerpUnclamped(FVector4 a, FVector4 b, FFloat t)
        {
            return new FVector4(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);
        }

        public static FVector4 MoveTowards(FVector4 current, FVector4 target, FFloat maxDistanceDelta)
        {
            FVector4 vector4 = target - current;
            FFloat magnitude = vector4.magnitude;
            if (magnitude <= maxDistanceDelta || magnitude == FFloat.zero)
                return target;
            return current + vector4 / magnitude * maxDistanceDelta;
        }

        public static FVector4 Scale(FVector4 a, FVector4 b)
        {
            return new FVector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        public void Scale(FVector4 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
            this.w *= scale.w;
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;
        }


        public override bool Equals(object other)
        {
            if (!(other is FVector4))
                return false;
            FVector4 vector4 = (FVector4)other;
            if (this == vector4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static FVector4 Normalize(FVector4 a)
        {
            FFloat num = FVector4.Magnitude(a);
            if (num > FFloat.err)
                return a / num;
            return FVector4.zero;
        }

        public void Normalize()
        {
            FFloat num = FVector4.Magnitude(this);
            if (num > FFloat.err)
                this = this / num;
            else
                this = FVector4.zero;
        }

        public static FFloat Dot(FVector4 a, FVector4 b)
        {
            return (a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w);
        }

        public static FVector4 Project(FVector4 a, FVector4 b)
        {
            return b * FVector4.Dot(a, b) / FVector4.Dot(b, b);
        }

        public static FFloat Distance(FVector4 a, FVector4 b)
        {
            return FVector4.Magnitude(a - b);
        }

        public static FFloat Magnitude(FVector4 a)
        {
            return FMath.Sqrt(FVector4.Dot(a, a));
        }

        public static FFloat SqrMagnitude(FVector4 a)
        {
            return FVector4.Dot(a, a);
        }

        public FFloat SqrMagnitude()
        {
            return FVector4.Dot(this, this);
        }

        public static FVector4 Min(FVector4 lhs, FVector4 rhs)
        {
            return new FVector4(FMath.Min(lhs.x, rhs.x), FMath.Min(lhs.y, rhs.y), FMath.Min(lhs.z, rhs.z), FMath.Min(lhs.w, rhs.w));
        }

        public static FVector4 Max(FVector4 lhs, FVector4 rhs)
        {
            return new FVector4(FMath.Max(lhs.x, rhs.x), FMath.Max(lhs.y, rhs.y), FMath.Max(lhs.z, rhs.z), FMath.Max(lhs.w, rhs.w));
        }
    }
}
