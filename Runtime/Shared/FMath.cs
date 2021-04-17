using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Parallel
{
    public static class FMath
    {
        public static FFloat PI { get { return FFloat.FromRaw(FixedConstants64.PI); } }
        public static FFloat HalfPI { get { return FFloat.FromRaw(FixedConstants64.PI_HALF); } }

        public static FFloat Rad2Deg { get { return FFloat.FromRaw(FixedConstants64.RAD_TO_DEGREE); } }
        public static FFloat Deg2Rad { get { return FFloat.FromRaw(FixedConstants64.DEGREE_TO_RAD); } }

        //FFloat
        public static FFloat Sqrt(FFloat value)
        {
            long sqrtValue = NativeFixedMath.Sqrt64(value.Raw);
            return FFloat.FromRaw(sqrtValue);
        }

        public static FFloat Div2(FFloat a) { return FFloat.FromRaw(a.Raw >> 1); }
        public static FFloat Abs(FFloat a) { return FFloat.FromRaw(NativeFixedMath.Abs64(a.Raw)); }
        public static FFloat Sign(FFloat a) { return FFloat.FromRaw(NativeFixedMath.Sign64(a.Raw)); }
        
        // rad
        public static FFloat Sin(FFloat a) { return FFloat.FromRaw(NativeFixedMath.Sin64(a.Raw)); }
        public static FFloat Asin(FFloat a) { return FFloat.FromRaw(NativeFixedMath.Asin64(a.Raw)); }

        public static FFloat Cos(FFloat a) { return FFloat.FromRaw(NativeFixedMath.Cos64(a.Raw)); }
        public static FFloat Acos(FFloat a) { return FFloat.FromRaw(NativeFixedMath.Acos64(a.Raw)); }

        public static FFloat Tan(FFloat a) { return FFloat.FromRaw(NativeFixedMath.Tan64(a.Raw)); }
        public static FFloat Atan(FFloat a) { return FFloat.FromRaw(NativeFixedMath.Atan64(a.Raw)); }
        public static FFloat Atan2(FFloat a, FFloat b) { return FFloat.FromRaw(NativeFixedMath.Atan264(a.Raw, b.Raw)); }

        public static FFloat Pow(FFloat a, FFloat b) { return FFloat.FromRaw(NativeFixedMath.Pow64(a.Raw, b.Raw)); }

        public static FFloat Exp(FFloat a) { return FFloat.FromRaw(NativeFixedMath.Pow64(FixedConstants64.E, a.Raw)); }

        public static FFloat Log10(FFloat a) { return FFloat.FromRaw(NativeFixedMath.Log64(a.Raw)); }
        public static FFloat Log(FFloat a) { return FFloat.FromRaw(NativeFixedMath.Log264(a.Raw)); }

        public static FFloat Max(FFloat a, FFloat b)
        {
            return a > b ? a : b;
        }

        public static FFloat Max(FFloat a, FFloat b, FFloat c)
        {
            return Max(a, Max(b, c));
        }

        public static FFloat Min(FFloat a, FFloat b)
        {
            return a < b ? a : b;
        }

        public static FFloat Min(FFloat a, FFloat b, FFloat c)
        {
            return Min(a, Min(b, c));
        }

        //rounding
        public static int CeilToInt(FFloat value)
        {
            return (int)((value.Raw + (FixedConstants64.ONE - 1)) >> FixedConstants64.SHIFT);
        }

        public static int FloorToInt(FFloat value)
        {
            return (int)((value.Raw) >> FixedConstants64.SHIFT);
        }

        public static int RoundToInt(FFloat value)
        {
            return (int)((value.Raw + FixedConstants64.HALF) >> FixedConstants64.SHIFT);
        }

        public static FFloat Ceil(FFloat value)
        {
            return (FFloat)CeilToInt(value);
        }

        public static FFloat Floor(FFloat value)
        {
            return (FFloat)FloorToInt(value);
        }
        
        public static FFloat Round(FFloat value)
        {
            return (FFloat)RoundToInt(value);
        }

        //clamp
        public static FFloat Clamp(FFloat a, FFloat low, FFloat high)
        {
            return Max(low, Min(a, high));
        }

        public static FFloat Clamp01(FFloat value)
        {
            FFloat result;
            if (value < FFloat.zero)
            {
                result = FFloat.zero;
            }
            else if (value > FFloat.one)
            {
                result = FFloat.one;
            }
            else
            {
                result = value;
            }
            return result;
        }

        //lerp
        public static FFloat Lerp(FFloat a, FFloat b, FFloat t)
        {
            t = Clamp(t, FFloat.zero, FFloat.one);
            return a + (b - a) * t;
        }

        public static FFloat LerpUnClamped(FFloat a, FFloat b, FFloat t)
        {
            return a + (b - a) * t;
        }

        public static FFloat LerpAngle(FFloat a, FFloat b, FFloat t)
        {
            FFloat num = FMath.Repeat(b - a, FFloat.s360);
            if (num > FFloat.s180)
            {
                num -= FFloat.s360;
            }
            return a + num * FMath.Clamp01(t);
        }

        public static FFloat MoveTowards(FFloat current, FFloat target, FFloat maxDelta)
        {
            FFloat result;
            if (FMath.Abs(target - current) <= maxDelta)
            {
                result = target;
            }
            else
            {
                result = current + FMath.Sign(target - current) * maxDelta;
            }
            return result;
        }

        public static FFloat MoveTowardsAngle(FFloat current, FFloat target, FFloat maxDelta)
        {
            FFloat num = FMath.DeltaAngle(current, target);
            FFloat result;
            if (-maxDelta < num && num < maxDelta)
            {
                result = target;
            }
            else
            {
                target = current + num;
                result = FMath.MoveTowards(current, target, maxDelta);
            }
            return result;
        }

        public static FFloat Gamma(FFloat value, FFloat absmax, FFloat gamma)
        {
            bool flag = false;
            if (value < FFloat.zero)
            {
                flag = true;
            }
            FFloat num = FMath.Abs(value);
            FFloat result;
            if (num > absmax)
            {
                result = ((!flag) ? num : (-num));
            }
            else
            {
                FFloat num2 = FMath.Pow(num / absmax, gamma) * absmax;
                result = ((!flag) ? num2 : (-num2));
            }
            return result;
        }

        public static bool Approximately(FFloat a, FFloat b)
        {
            return FMath.Abs(b - a) < FFloat.err;
        }

        public static FFloat SmoothStep(FFloat from, FFloat to, FFloat t)
        {
            t = FMath.Clamp01(t);
            t = -FFloat.two * t * t * t + FFloat.three * t * t;
            return to * t + from * (FFloat.one - t);
        }

        public static FFloat SmoothDamp(FFloat current, FFloat target, ref FFloat currentVelocity, FFloat smoothTime, FFloat maxSpeed, FFloat deltaTime)
        {
            smoothTime = FMath.Max(FFloat.FromDivision(1, 10000), smoothTime);
            FFloat num = FFloat.two / smoothTime;
            FFloat num2 = num * deltaTime;
            FFloat num3 = FFloat.one / (FFloat.one + num2 + FFloat.FromDivision(48, 100) * num2 * num2 + FFloat.FromDivision(235, 1000) * num2 * num2 * num2);
            FFloat num4 = current - target;
            FFloat num5 = target;
            FFloat num6 = maxSpeed * smoothTime;
            num4 = FMath.Clamp(num4, -num6, num6);
            target = current - num4;
            FFloat num7 = (currentVelocity + num * num4) * deltaTime;
            currentVelocity = (currentVelocity - num * num7) * num3;
            FFloat num8 = target + (num4 + num7) * num3;
            if (num5 - current > FFloat.zero == num8 > num5)
            {
                num8 = num5;
                currentVelocity = (num8 - num5) / deltaTime;
            }
            return num8;
        }

        public static FFloat SmoothDampAngle(FFloat current, FFloat target, ref FFloat currentVelocity, FFloat smoothTime, FFloat maxSpeed, FFloat deltaTime)
        {
            target = current + FMath.DeltaAngle(current, target);
            return FMath.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        //
        public static FFloat Repeat(FFloat t, FFloat length)
        {
            return FMath.Clamp(t - FMath.Floor(t / length) * length, FFloat.zero, length);
        }

        public static FFloat PingPong(FFloat t, FFloat length)
        {
            t = FMath.Repeat(t, length * FFloat.two);
            return length - FMath.Abs(t - length);
        }

        public static FFloat InverseLerp(FFloat a, FFloat b, FFloat value)
        {
            FFloat result;
            if (a != b)
            {
                result = FMath.Clamp01((value - a) / (b - a));
            }
            else
            {
                result = FFloat.zero;
            }
            return result;
        }

        public static FFloat DeltaAngle(FFloat current, FFloat target)
        {
            FFloat num = FMath.Repeat(target - current, FFloat.s360);
            if (num > FFloat.s180)
            {
                num -= FFloat.s360;
            }
            return num;
        }


        //FVector2

        public static FVector2 FindNearestPointOnLine(FVector2 p, FVector2 a, FVector2 b)
        {
            FVector2 ba = b - a;

            FVector2 pa = p - a;
            FFloat d = pa.Length();

            FFloat angle = FVector2.Angle(ba, pa);
            if (angle > FFloat.FromDivision(90, 1))
            {
                angle = FFloat.FromDivision(90, 1);
            }
            d = d * Cos(angle * Deg2Rad);

            return a + ba.normalized * d;
        }

        public static bool InSpan(FVector2 v, FVector2 va, FVector2 vb)
        {
            FFloat AXB = FVector2.Cross(va, vb);
            FFloat BXA = FVector2.Cross(vb, va);

            FFloat AXV = FVector2.Cross(va, v);
            FFloat BXV = FVector2.Cross(vb, v);

            if (AXV * AXB >= FFloat.zero && BXV * BXA >= FFloat.zero)
            {
                return true;
            }

            return false;
        }

        internal static bool LineIntersection(FVector2 p1, FVector2 p2, FVector2 p3, FVector2 p4, ref FVector2 result)
        {
            FFloat num = p2.x - p1.x;
            FFloat num2 = p2.y - p1.y;
            FFloat num3 = p4.x - p3.x;
            FFloat num4 = p4.y - p3.y;
            FFloat num5 = num * num4 - num2 * num3;
            bool result2;
            if (num5 == FFloat.zero)
            {
                result2 = false;
            }
            else
            {
                FFloat num6 = p3.x - p1.x;
                FFloat num7 = p3.y - p1.y;
                FFloat num8 = (num6 * num4 - num7 * num3) / num5;
                result = new FVector2(p1.x + num8 * num, p1.y + num8 * num2);
                result2 = true;
            }
            return result2;
        }

        internal static bool LineSegmentIntersection(FVector2 p1, FVector2 p2, FVector2 p3, FVector2 p4, ref FVector2 result)
        {
            FFloat num = p2.x - p1.x;
            FFloat num2 = p2.y - p1.y;
            FFloat num3 = p4.x - p3.x;
            FFloat num4 = p4.y - p3.y;
            FFloat num5 = num * num4 - num2 * num3;
            bool result2;
            if (num5 == FFloat.zero)
            {
                result2 = false;
            }
            else
            {
                FFloat num6 = p3.x - p1.x;
                FFloat num7 = p3.y - p1.y;
                FFloat num8 = (num6 * num4 - num7 * num3) / num5;
                if (num8 < FFloat.zero || num8 > FFloat.one)
                {
                    result2 = false;
                }
                else
                {
                    FFloat num9 = (num6 * num2 - num7 * num) / num5;
                    if (num9 < FFloat.zero || num9 > FFloat.one)
                    {
                        result2 = false;
                    }
                    else
                    {
                        result = new FVector2(p1.x + num8 * num, p1.y + num8 * num2);
                        result2 = true;
                    }
                }
            }
            return result2;
        }

        //FVector3
        public static FVector3 Mul(FVector3 pos, FQuaternion rot, FVector3 point)
        {
            FVector3 output = FVector3.zero;
            NativeParallel3D.Mul(pos, rot, point, ref output);
            return output;
        }

        public static FVector3 MulT(FVector3 pos, FQuaternion rot, FVector3 point)
        {
            FVector3 output = FVector3.zero;
            NativeParallel3D.MulT(pos, rot, point, ref output);
            return output;
        }
    }
}
