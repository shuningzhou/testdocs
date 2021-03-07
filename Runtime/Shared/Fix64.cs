using System;
using System.Runtime.CompilerServices;

namespace Parallel
{
    public static class FixedConstants64
    {
        public const int SHIFT = 20;
        public const double FLOAT_ONE = 1048576.0;
        public const int ONE = 1048576;
        public const int NINETY = 94371840;
        public const int HALF = 524288;
        public const int QUARTER = 262144;
        public const int NEG_ONE = -1048576;
        public const int TWO = 2097152;
        public const int NEG_TWO = -2097152;
        public const long PI = 3294198;
        public const long PI_TWO = 6588397;
        public const long PI_HALF = 1647099;
        public const long E = 2850325;
        public const long MIN = -9223372036854775808;
        public const long MAX = 9223372036854775807;
        public const long RAD_TO_DEGREE = 60078991;
        public const long DEGREE_TO_RAD = 18301;
    }

    [Serializable]
    public struct Fix64 : IComparable<Fix64>, IEquatable<Fix64>
    {
        public static Fix64 zero    { get { return FromRaw(0L); } }
        public static Fix64 one     { get { return FromRaw(1L << FixedConstants64.SHIFT); } }
        public static Fix64 two     { get { return FromRaw(FixedConstants64.TWO); } }
        public static Fix64 ninety { get { return FromRaw(FixedConstants64.NINETY); } }
        public static Fix64 negOne  { get { return FromRaw(FixedConstants64.NEG_ONE); } }
        public static Fix64 negTwo { get { return FromRaw(FixedConstants64.NEG_TWO); } }

        public static Fix64 half { get { return FromRaw(FixedConstants64.HALF); } }
        public static Fix64 quarter { get { return FromRaw(FixedConstants64.QUARTER); } }

        public static Fix64 pi { get { return FromRaw(FixedConstants64.PI); } }
        public static Fix64 halfPi { get { return FromRaw(FixedConstants64.PI_HALF); } }
        public static Fix64 twoPi { get { return FromRaw(FixedConstants64.PI_TWO); } }

        public static Fix64 e { get { return FromRaw(FixedConstants64.E); } }

        public static Fix64 max { get { return FromRaw(FixedConstants64.MIN); } }
        public static Fix64 min { get { return FromRaw(FixedConstants64.MIN); } }

        public static Fix64 spv { get { return FromRaw(1); } }

        public long Raw;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 FromRaw(long rawValue)
        {
            Fix64 r;
            r.Raw = rawValue;
            return r;
        }

        public static Fix64 FromDivision(int value, UInt32 divisor)
        {
            if (divisor <= 1)
            {
                divisor = 1;
            }
            
            if (divisor > 100000)
            {
                divisor = 100000;
            }

            return Fix64.FromRaw(((long)value << FixedConstants64.SHIFT) / divisor);
        }

        public int CompareTo(Fix64 other)
        {
            if (Raw < other.Raw) return -1;
            if (Raw > other.Raw) return +1;
            return 0;
        }

        public bool Equals(Fix64 other)
        {
            return (Raw == other.Raw);
        }
        
        //casting
        public static explicit operator Fix64(int value)
        {
            return Fix64.FromRaw((long)value << FixedConstants64.SHIFT);
        }

        public static explicit operator int(Fix64 value)
        {
            return (int)((value.Raw + FixedConstants64.HALF) >> FixedConstants64.SHIFT);
        }

        public static explicit operator Fix64(float value)
        {
            return Fix64.FromRaw((long)(value * FixedConstants64.FLOAT_ONE));
        }

        public static explicit operator float(Fix64 value)
        {
            return (float)(value.Raw * (1.0f / FixedConstants64.FLOAT_ONE));
        }

        //operators
        public static Fix64 operator -(Fix64 v1)
        {
            return FromRaw(-v1.Raw);
        }


        public static bool operator ==(Fix64 a, Fix64 b) { return a.Raw == b.Raw; }
        public static bool operator !=(Fix64 a, Fix64 b) { return a.Raw != b.Raw; }

        public static bool operator <(Fix64 a, Fix64 b) { return a.Raw < b.Raw; }
        public static bool operator <=(Fix64 a, Fix64 b) { return a.Raw <= b.Raw; }
        public static bool operator >(Fix64 a, Fix64 b) { return a.Raw > b.Raw; }
        public static bool operator >=(Fix64 a, Fix64 b) { return a.Raw >= b.Raw; }

        public static Fix64 RadToDeg(Fix64 a)
        {
            return FromRaw(NativeFixedMath.Mul64(a.Raw, FixedConstants64.RAD_TO_DEGREE));
        } 
        public static Fix64 DegToRad(Fix64 a)
        {
            return FromRaw(NativeFixedMath.Mul64(a.Raw, FixedConstants64.DEGREE_TO_RAD));
        }

        public static Fix64 operator +(Fix64 a, Fix64 b) { return FromRaw(a.Raw + b.Raw); }
        public static Fix64 operator -(Fix64 a, Fix64 b) { return FromRaw(a.Raw - b.Raw); }
        public static Fix64 operator /(Fix64 a, Fix64 b) { return FromRaw(NativeFixedMath.Div64(a.Raw, b.Raw)); }
        public static Fix64 operator *(Fix64 a, Fix64 b) { return FromRaw(NativeFixedMath.Mul64(a.Raw, b.Raw)); }
        public static Fix64 operator %(Fix64 a, Fix64 b) { return FromRaw(NativeFixedMath.Mod64(a.Raw, b.Raw)); }

        public override string ToString()
        {
            float f = (float)this;
            return f.ToString("0.000") + "(" + Raw.ToString() + ")";
        }

        public override int GetHashCode()
        {
            return (int)Raw | (int)(Raw >> 32);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Fix64))
                return false;
            return ((Fix64)obj).Raw == Raw;
        }
    }
}
