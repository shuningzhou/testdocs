using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Parallel
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct FVector2
    {
        public static FVector2 zero { get { return FrameRaw(0L, 0L); } }
        public static FVector2 one { get { return FrameRaw(1L << FixedConstants64.SHIFT, 1L << FixedConstants64.SHIFT); } }

        public static FVector2 down { get { return new FVector2(FFloat.zero, FFloat.negOne); } }
        public static FVector2 up { get { return new FVector2(FFloat.zero, FFloat.one); } }
        public static FVector2 left { get { return new FVector2(FFloat.negOne, FFloat.zero); } }
        public static FVector2 right { get { return new FVector2(FFloat.one, FFloat.zero); } }

        public long RawX;
        public long RawY;

        public FFloat x { get { return FFloat.FromRaw(RawX); } set { RawX = value.Raw; } }
        public FFloat y { get { return FFloat.FromRaw(RawY); } set { RawY = value.Raw; } }

        public FFloat this[int index]
        {
            get
            {
                FFloat result;
                if (index != 0)
                {
                    if (index != 1)
                    {
                        throw new IndexOutOfRangeException("Invalid FVector2 index!");
                    }
                    result = this.y;
                }
                else
                {
                    result = this.x;
                }
                return result;
            }
            set
            {
                if (index != 0)
                {
                    if (index != 1)
                    {
                        throw new IndexOutOfRangeException("Invalid FVector2 index!");
                    }
                    this.y = value;
                }
                else
                {
                    this.x = value;
                }
            }
        }

        public FVector2 normalized
        {
            get
            {
                FVector2 result = new FVector2(this.x, this.y);
                result.Normalize();
                return result;
            }
        }

        public FFloat magnitude
        {
            get
            {
                return FMath.Sqrt(this.x * this.x + this.y * this.y);
            }
        }

        public FFloat sqrMagnitude
        {
            get
            {
                return this.x * this.x + this.y * this.y;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FVector2 FrameRaw(long rawValueX, long rawValueY)
        {
            FVector2 r;
            r.RawX = rawValueX;
            r.RawY = rawValueY;
            return r;
        }

        public FVector2(FFloat x, FFloat y)
        {
            RawX = x.Raw;
            RawY = y.Raw;
        }

        public void Set(FFloat newX, FFloat newY)
        {
            this.x = newX;
            this.y = newY;
        }

        //operators
        public static explicit operator FVector2(Vector2 value)
        {
            FVector2 r = FVector2.zero;

            r.x = (FFloat)value.x;
            r.y = (FFloat)value.y;

            return r;
        }

        public static explicit operator FVector2(Vector3 value)
        {
            FVector2 r = FVector2.zero;

            r.x = (FFloat)value.x;
            r.y = (FFloat)value.y;

            return r;
        }

        public static explicit operator Vector2(FVector2 value)
        {
            return new Vector2((float)value.x, (float)value.y);
        }

        public static explicit operator FVector2(FVector3 value)
        {
            return FVector2.FrameRaw(value.RawX, value.RawY);
        }

        public static FVector2 operator +(FVector2 a, FVector2 b)
        {
            long x = a.RawX + b.RawX;
            long y = a.RawY + b.RawY;

            FVector2 r = FrameRaw(x, y);
            return r;
        }

        public static FVector2 operator -(FVector2 a, FVector2 b)
        {
            long x = a.RawX - b.RawX;
            long y = a.RawY - b.RawY;

            FVector2 r = FrameRaw(x, y);
            return r;
        }

        public static FVector2 operator *(FVector2 a, FVector2 b)
        {
            long x = NativeFixedMath.Mul64(a.RawX, b.RawX);
            long y = NativeFixedMath.Mul64(a.RawY, b.RawY);

            FVector2 r = FrameRaw(x, y);
            return r;
        }

        public static FVector2 operator *(FFloat a, FVector2 b)
        {
            return FrameRaw(NativeFixedMath.Mul64(a.Raw, b.RawX), NativeFixedMath.Mul64(a.Raw, b.RawY));
        }

        public static FVector2 operator *(FVector2 a, FFloat b)
        {
            return FrameRaw(NativeFixedMath.Mul64(a.RawX, b.Raw), NativeFixedMath.Mul64(a.RawY, b.Raw));
        }

        public static FVector2 operator /(FFloat a, FVector2 b)
        {
            return FrameRaw(NativeFixedMath.Div64(a.Raw, b.RawX), NativeFixedMath.Div64(a.Raw, b.RawY));
        }

        public static FVector2 operator /(FVector2 a, FFloat b)
        {
            return FrameRaw(NativeFixedMath.Div64(a.RawX, b.Raw), NativeFixedMath.Div64(a.RawY, b.Raw));
        }


        public static bool operator ==(FVector2 a, FVector2 b) { return a.RawX == b.RawX && a.RawY == b.RawY; }
        public static bool operator !=(FVector2 a, FVector2 b) { return a.RawX != b.RawX || a.RawY != b.RawY; }

        public static FVector2 Lerp(FVector2 a, FVector2 b, FFloat t)
        {
            t = FMath.Clamp01(t);
            return new FVector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }

        public static FVector2 LerpUnclamped(FVector2 a, FVector2 b, FFloat t)
        {
            return new FVector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }

        public static FVector2 MoveTowards(FVector2 current, FVector2 target, FFloat maxDistanceDelta)
        {
            FVector2 a = target - current;
            FFloat magnitude = a.magnitude;
            FVector2 result;
            if (magnitude <= maxDistanceDelta || magnitude == FFloat.zero)
            {
                result = target;
            }
            else
            {
                result = current + a / magnitude * maxDistanceDelta;
            }
            return result;
        }

        public static FVector2 Perpendicular(FVector2 a)
        {
            return new FVector2(-a.y, a.x);
        }

        public static FVector2 Scale(FVector2 a, FVector2 b)
        {
            return new FVector2(a.x * b.x, a.y * b.y);
        }

        public void Scale(FVector2 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
        }

        public void Normalize()
        {
            FFloat magnitude = this.magnitude;
            if (magnitude > FFloat.err)
            {
                this /= magnitude;
            }
            else
            {
                this = FVector2.zero;
            }
        }

        public FFloat Length()
        {
            FFloat result = FFloat.zero;
            NativeParallel2D.Vec2Length64(this, ref result);
            return result;
        }

        //Distance between two points
        public static FFloat Distance(FVector2 a, FVector2 b)
        {
            return (a - b).magnitude;
        }

        public static FVector2 Reflect(FVector2 inDirection, FVector2 inNormal)
        {
            return -FFloat.two * FVector2.Dot(inNormal, inDirection) * inNormal + inDirection;
        }

        //Dot product of two vectors
        public static FFloat Dot(FVector2 a, FVector2 b)
        {
            return a.x * b.x + a.y * b.y;
        }


        //Cross product of two vectors
        public static FFloat Cross(FVector2 a, FVector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        public static FFloat Angle(FVector2 a, FVector2 b)
        {
            FFloat dot = FVector2.Dot(a.normalized, b.normalized);
            FFloat clamped = FMath.Clamp(dot, -FFloat.one, FFloat.one);
            FFloat rad = FFloat.FromRaw(NativeFixedMath.Acos64(clamped.Raw));
            return rad * FMath.Rad2Deg;
        }

        public static FFloat SignedAngle(FVector2 from, FVector2 to)
        {
            FVector2 normalized = from.normalized;
            FVector2 normalized2 = to.normalized;
            FFloat num = FMath.Acos(FMath.Clamp(FVector2.Dot(normalized, normalized2), FFloat.negOne, FFloat.one)) * FMath.Rad2Deg;
            FFloat num2 = FMath.Sign(normalized.x * normalized2.y - normalized.y * normalized2.x);
            return num * num2;
        }

        public static FVector2 ClampMagnitude(FVector2 vector, FFloat maxLength)
        {
            FVector2 result;
            if (vector.sqrMagnitude > maxLength * maxLength)
            {
                result = vector.normalized * maxLength;
            }
            else
            {
                result = vector;
            }
            return result;
        }

        public static FFloat SqrMagnitude(FVector2 a)
        {
            return a.x * a.x + a.y * a.y;
        }

        public FFloat SqrMagnitude()
        {
            return this.x * this.x + this.y * this.y;
        }

        public static FVector2 Min(FVector2 lhs, FVector2 rhs)
        {
            return new FVector2(FMath.Min(lhs.x, rhs.x), FMath.Min(lhs.y, rhs.y));
        }

        public static FVector2 Max(FVector2 lhs, FVector2 rhs)
        {
            return new FVector2(FMath.Max(lhs.x, rhs.x), FMath.Max(lhs.y, rhs.y));
        }

        public static FVector2 Intersection(FVector2 a1, FVector2 a2, FVector2 b1, FVector2 b2, out bool found)
        {
            FFloat tmp = (b2.x - b1.x) * (a2.y - a1.y) - (b2.y - b1.y) * (a2.x - a1.x);

            if (tmp == FFloat.zero)
            {
                // No solution!
                found = false;
                return FVector2.zero;
            }

            FFloat mu = ((a1.x - b1.x) * (a2.y - a1.y) - (a1.y - b1.y) * (a2.x - a1.x)) / tmp;

            found = true;

            return new FVector2(
                b1.x + (b2.x - b1.x) * mu,
                b1.y + (b2.y - b1.y) * mu
            );
        }

        public static FVector2 SmoothDamp(FVector2 current, FVector2 target, ref FVector2 currentVelocity, FFloat smoothTime, FFloat maxSpeed, FFloat deltaTime)
        {
            smoothTime = FMath.Max(FFloat.FromDivision(1, 10000), smoothTime);
            FFloat num = FFloat.two / smoothTime;
            FFloat num2 = num * deltaTime;
            FFloat d = FFloat.one / (FFloat.one + num2 + FFloat.FromDivision(48, 100) * num2 * num2 + FFloat.FromDivision(235, 1000) * num2 * num2 * num2);
            FVector2 vector = current - target;
            FVector2 vector2 = target;
            FFloat maxLength = maxSpeed * smoothTime;
            vector = FVector2.ClampMagnitude(vector, maxLength);
            target = current - vector;
            FVector2 vector3 = (currentVelocity + num * vector) * deltaTime;
            currentVelocity = (currentVelocity - num * vector3) * d;
            FVector2 vector4 = target + (vector + vector3) * d;
            if (FVector2.Dot(vector2 - current, vector4 - vector2) > FFloat.zero)
            {
                vector4 = vector2;
                currentVelocity = (vector4 - vector2) / deltaTime;
            }
            return vector4;
        }

        public static FVector2 Intersection(FVector2 a1, FVector2 v, FFloat range, FVector2 b1, FVector2 b2, out bool found)
        {
            FVector2 a2 = a1 + v.normalized * range;
            return Intersection(a1, a2, b1, b2, out found);
        }

        // Calculate the distance between
        // point pt and the segment p1 --> p2.
        //http://csharphelper.com/blog/2016/09/find-the-shortest-distance-between-a-point-and-a-line-segment-in-c/
        public static FFloat DistanceToSegment(
    FVector2 pt, FVector2 p1, FVector2 p2, out FVector2 closest)
        {
            FFloat dx = p2.x - p1.x;
            FFloat dy = p2.y - p1.y;
            if ((dx == FFloat.zero) && (dy == FFloat.zero))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.x - p1.x;
                dy = pt.y - p1.y;
                return FMath.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            FFloat t = ((pt.x - p1.x) * dx + (pt.y - p1.y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < FFloat.zero)
            {
                closest = p1;
                dx = pt.x - p1.x;
                dy = pt.y - p1.y;
            }
            else if (t > FFloat.one)
            {
                closest = p2;
                dx = pt.x - p2.x;
                dy = pt.y - p2.y;
            }
            else
            {
                closest = new FVector2(p1.x + t * dx, p1.y + t * dy);
                dx = pt.x - closest.x;
                dy = pt.y - closest.y;
            }

            return FMath.Sqrt(dx * dx + dy * dy);
        }

        public override string ToString()
        {
            return $"Fix64Vec2({x}, {y})";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FVector2))
                return false;
            return ((FVector2)obj) == this;
        }

        public override int GetHashCode()
        {
            return RawX.GetHashCode() ^ RawY.GetHashCode() * 7919;
        }
    }
}
