using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Parallel
{
    public static class Fix64Math
    {
        public static Fix64 pi { get { return Fix64.FromRaw(FixedConstants64.PI); } }
        public static Fix64 halfPi { get { return Fix64.FromRaw(FixedConstants64.PI_HALF); } }

        public static Fix64 RadToDegree { get { return Fix64.FromRaw(FixedConstants64.RAD_TO_DEGREE); } }
        public static Fix64 DegreeToRad { get { return Fix64.FromRaw(FixedConstants64.DEGREE_TO_RAD); } }

        //rounding
        public static int Ceiling(Fix64 value)
        {
            return (int)((value.Raw + (FixedConstants64.ONE - 1)) >> FixedConstants64.SHIFT);
        }

        public static int Floor(Fix64 value)
        {
            return (int)(value.Raw >> FixedConstants64.SHIFT);
        }

        //Fix64
        public static Fix64 Sqrt(Fix64 value)
        {
            long sqrtValue = NativeFixedMath.Sqrt64(value.Raw);
            return Fix64.FromRaw(sqrtValue);
        }

        public static Fix64 Div2(Fix64 a) { return Fix64.FromRaw(a.Raw >> 1); }
        public static Fix64 Abs(Fix64 a) { return Fix64.FromRaw(NativeFixedMath.Abs64(a.Raw)); }
        public static Fix64 Sign(Fix64 a) { return Fix64.FromRaw(NativeFixedMath.Sign64(a.Raw)); }

        public static Fix64 Sin(Fix64 a) { return Fix64.FromRaw(NativeFixedMath.Sin64(a.Raw)); }
        public static Fix64 Asin(Fix64 a) { return Fix64.FromRaw(NativeFixedMath.Asin64(a.Raw)); }

        public static Fix64 Cos(Fix64 a) { return Fix64.FromRaw(NativeFixedMath.Cos64(a.Raw)); }
        public static Fix64 ACos(Fix64 a) { return Fix64.FromRaw(NativeFixedMath.Acos64(a.Raw)); }

        public static Fix64 Tan(Fix64 a) { return Fix64.FromRaw(NativeFixedMath.Tan64(a.Raw)); }
        public static Fix64 Atan(Fix64 a) { return Fix64.FromRaw(NativeFixedMath.Atan64(a.Raw)); }
        public static Fix64 Atan2(Fix64 a, Fix64 b) { return Fix64.FromRaw(NativeFixedMath.Atan264(a.Raw, b.Raw)); }

        public static Fix64 Pow(Fix64 a, Fix64 b) { return Fix64.FromRaw(NativeFixedMath.Pow64(a.Raw, b.Raw)); }

        public static Fix64 Log(Fix64 a) { return Fix64.FromRaw(NativeFixedMath.Log64(a.Raw)); }
        public static Fix64 Log2(Fix64 a) { return Fix64.FromRaw(NativeFixedMath.Log264(a.Raw)); }

        public static Fix64 Max(Fix64 a, Fix64 b)
        {
            return a > b ? a : b;
        }

        public static Fix64 Max(Fix64 a, Fix64 b, Fix64 c)
        {
            return Max(a, Max(b, c));
        }

        public static Fix64 Min(Fix64 a, Fix64 b)
        {
            return a < b ? a : b;
        }

        public static Fix64 Clamp(Fix64 a, Fix64 low, Fix64 high)
        {
            return Max(low, Min(a, high));
        }

        public static Fix64 Lerp(Fix64 a, Fix64 b, Fix64 t)
        {
            t = Clamp(t, Fix64.zero, Fix64.one);
            return a + (b - a) * t;
        }

        public static Fix64 LerpUnClamped(Fix64 a, Fix64 b, Fix64 t)
        {
            return a + (b - a) * t;
        }

        //Fix64Vec2

        public static Fix64Vec2 FindNearestPointOnLine(Fix64Vec2 p, Fix64Vec2 a, Fix64Vec2 b)
        {
            Fix64Vec2 ba = b - a;

            Fix64Vec2 pa = p - a;
            Fix64 d = pa.Length();

            Fix64 angle = Fix64Vec2.Angle(ba, pa);
            if (angle > Fix64.FromDivision(90, 1))
            {
                angle = Fix64.FromDivision(90, 1);
            }
            d = d * Cos(angle * DegreeToRad);

            return a + ba.normalized * d;
        }

        public static bool InSpan(Fix64Vec2 v, Fix64Vec2 va, Fix64Vec2 vb)
        {
            Fix64 AXB = Fix64Vec2.Cross(va, vb);
            Fix64 BXA = Fix64Vec2.Cross(vb, va);

            Fix64 AXV = Fix64Vec2.Cross(va, v);
            Fix64 BXV = Fix64Vec2.Cross(vb, v);

            if (AXV * AXB >= Fix64.zero && BXV * BXA >= Fix64.zero)
            {
                return true;
            }

            return false;
        }

        //Fix64Vec3
        public static Fix64Vec3 Mul(Fix64Vec3 pos, Fix64Quat rot, Fix64Vec3 point)
        {
            Fix64Vec3 output = Fix64Vec3.zero;
            NativeParallel3D.Mul(pos, rot, point, ref output);
            return output;
        }

        public static Fix64Vec3 MulT(Fix64Vec3 pos, Fix64Quat rot, Fix64Vec3 point)
        {
            Fix64Vec3 output = Fix64Vec3.zero;
            NativeParallel3D.MulT(pos, rot, point, ref output);
            return output;
        }
    }
}
