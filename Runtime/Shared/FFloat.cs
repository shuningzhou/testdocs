using System;
using System.Runtime.CompilerServices;

namespace Parallel
{
    public static class FixedConstants64
    {
        public const int SHIFT = 20;
        public const float FLOAT_ONE = 1048576.0f;
        public const float ONE_OVER_FLOAT_ONE = 1.0f / FLOAT_ONE;
        public const int ONE = 1048576;
        public const int D_90 = 94371840;
        public const int D_180 = 188743680;
        public const int D_270 = 283115520;
        public const int D_360 = 377480360;
        public const int HALF = 524288;
        public const int QUARTER = 262144;
        public const int NEG_ONE = -1048576;
        public const int TWO = 2097152;
        public const int THREE = 3145728;
        public const int NEG_TWO = -2097152;
        public const long PI = 3294198;
        public const long PI_TWO = 6588397;
        public const long PI_HALF = 1647099;
        public const long E = 2850325;
        public const long MIN = -9223372036854775808;
        public const long MAX = 9223372036854775807;
        public const long RAD_TO_DEGREE = 60078979;
        public const long DEGREE_TO_RAD = 18301;
    }

    [Serializable]
    public struct FFloat : IComparable<FFloat>, IEquatable<FFloat>
    {
        public static FFloat zero    { get { return FromRaw(0L); } }
        public static FFloat one     { get { return FromRaw(1L << FixedConstants64.SHIFT); } }
        public static FFloat two     { get { return FromRaw(FixedConstants64.TWO); } }

        public static FFloat three { get { return FromRaw(FixedConstants64.THREE); } }
        public static FFloat ninety { get { return FromRaw(FixedConstants64.D_90); } }

        public static FFloat s90 { get { return FromRaw(FixedConstants64.D_90); } }

        public static FFloat s180 { get { return FromRaw(FixedConstants64.D_180); } }

        public static FFloat s270 { get { return FromRaw(FixedConstants64.D_270); } }

        public static FFloat s360 { get { return FromRaw(FixedConstants64.D_360); } }

        public static FFloat s0 { get { return zero; } }

        public static FFloat negOne  { get { return FromRaw(FixedConstants64.NEG_ONE); } }
        public static FFloat negTwo { get { return FromRaw(FixedConstants64.NEG_TWO); } }

        public static FFloat half { get { return FromRaw(FixedConstants64.HALF); } }
        public static FFloat quarter { get { return FromRaw(FixedConstants64.QUARTER); } }

        public static FFloat pi { get { return FromRaw(FixedConstants64.PI); } }
        public static FFloat halfPi { get { return FromRaw(FixedConstants64.PI_HALF); } }
        public static FFloat twoPi { get { return FromRaw(FixedConstants64.PI_TWO); } }

        public static FFloat e { get { return FromRaw(FixedConstants64.E); } }

        public static FFloat max { get { return FromRaw(FixedConstants64.MIN); } }
        public static FFloat min { get { return FromRaw(FixedConstants64.MIN); } }

        public static FFloat spv { get { return FromRaw(1); } }

        public static FFloat err { get { return FromRaw(210); } }

        internal static FFloat s0_4995 { get { return FromRaw(523763); } }
        internal static FFloat s0_0001 { get { return FromRaw(105); } }

        public long Raw;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FFloat FromRaw(long rawValue)
        {
            FFloat r;
            r.Raw = rawValue;
            return r;
        }

        public static FFloat FromDivision(int value, UInt32 divisor)
        {
            if (divisor <= 1)
            {
                divisor = 1;
            }
            
            if (divisor > 100000)
            {
                divisor = 100000;
            }

            return FFloat.FromRaw(((long)value << FixedConstants64.SHIFT) / divisor);
        }

        public int CompareTo(FFloat other)
        {
            if (Raw < other.Raw) return -1;
            if (Raw > other.Raw) return +1;
            return 0;
        }

        public bool Equals(FFloat other)
        {
            return (Raw == other.Raw);
        }
        
        //casting
        public static explicit operator FFloat(int value)
        {
            return FFloat.FromRaw((long)value << FixedConstants64.SHIFT);
        }

        public static explicit operator int(FFloat value)
        {
            return (int)((value.Raw) >> FixedConstants64.SHIFT);
        }

        public static explicit operator FFloat(float value)
        {
            return FFloat.FromRaw((long)(value * FixedConstants64.FLOAT_ONE));
        }

        public static explicit operator float(FFloat value)
        {
            return (float)(value.Raw * (FixedConstants64.ONE_OVER_FLOAT_ONE));
        }

        //operators
        public static FFloat operator -(FFloat v1)
        {
            return FromRaw(-v1.Raw);
        }


        public static bool operator ==(FFloat a, FFloat b) { return a.Raw == b.Raw; }
        public static bool operator !=(FFloat a, FFloat b) { return a.Raw != b.Raw; }

        public static bool operator <(FFloat a, FFloat b) { return a.Raw < b.Raw; }
        public static bool operator <=(FFloat a, FFloat b) { return a.Raw <= b.Raw; }
        public static bool operator >(FFloat a, FFloat b) { return a.Raw > b.Raw; }
        public static bool operator >=(FFloat a, FFloat b) { return a.Raw >= b.Raw; }

        public static FFloat RadToDeg(FFloat a)
        {
            return FromRaw(NativeFixedMath.Mul64(a.Raw, FixedConstants64.RAD_TO_DEGREE));
        } 
        public static FFloat DegToRad(FFloat a)
        {
            return FromRaw(NativeFixedMath.Mul64(a.Raw, FixedConstants64.DEGREE_TO_RAD));
        }

        public static FFloat operator +(FFloat a, FFloat b) { return FromRaw(a.Raw + b.Raw); }
        public static FFloat operator -(FFloat a, FFloat b) { return FromRaw(a.Raw - b.Raw); }
        public static FFloat operator /(FFloat a, FFloat b) { return FromRaw(NativeFixedMath.Div64(a.Raw, b.Raw)); }
        public static FFloat operator *(FFloat a, FFloat b) { return FromRaw(NativeFixedMath.Mul64(a.Raw, b.Raw)); }
        public static FFloat operator %(FFloat a, FFloat b) { return FromRaw(NativeFixedMath.Mod64(a.Raw, b.Raw)); }

        public override string ToString()
        {
            float f = (float)this;
            return f.ToString("0.00000") + "(" + Raw.ToString() + ")";
        }

        public override int GetHashCode()
        {
            return (int)Raw | (int)(Raw >> 32);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FFloat))
                return false;
            return ((FFloat)obj).Raw == Raw;
        }
    }
}
