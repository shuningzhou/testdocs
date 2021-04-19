using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Parallel
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 24)]
    public struct FVector3
    {
        static FVector3 _szero = new FVector3(FFloat.zero, FFloat.zero, FFloat.zero);
        public static FVector3 zero    { get { return _szero; } }
        public static FVector3 one     { get { return new FVector3(FFloat.one, FFloat.one, FFloat.one); } }
        public static FVector3 down    { get { return new FVector3(FFloat.zero, FFloat.negOne, FFloat.zero); } }
        public static FVector3 up      { get { return new FVector3(FFloat.zero, FFloat.one, FFloat.zero); } }
        public static FVector3 left    { get { return new FVector3(FFloat.negOne, FFloat.zero, FFloat.zero); } }
        public static FVector3 right   { get { return new FVector3(FFloat.one, FFloat.zero, FFloat.zero); } }
        public static FVector3 forward { get { return new FVector3(FFloat.zero, FFloat.zero, FFloat.one); } }
        public static FVector3 back    { get { return new FVector3(FFloat.zero, FFloat.zero, FFloat.negOne); } }
        public static FVector3 axisX   { get { return new FVector3(FFloat.one, FFloat.zero, FFloat.zero); } }
        public static FVector3 axisY   { get { return new FVector3(FFloat.zero, FFloat.one, FFloat.zero); } }
        public static FVector3 axisZ   { get { return new FVector3(FFloat.zero, FFloat.zero, FFloat.one); } }

        public long RawX;
        public long RawY;
        public long RawZ;

        public FFloat x { get { return FFloat.FromRaw(RawX); } set { RawX = value.Raw; } }
        public FFloat y { get { return FFloat.FromRaw(RawY); } set { RawY = value.Raw; } }
        public FFloat z { get { return FFloat.FromRaw(RawZ); } set { RawZ = value.Raw; } }

        public FFloat this[int index]
        {
            get
            {
                FFloat result;
                switch (index)
                {
                    case 0:
                        result = this.x;
                        break;
                    case 1:
                        result = this.y;
                        break;
                    case 2:
                        result = this.z;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid FVector3 index!");
                }
                return result;
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
                    default:
                        throw new IndexOutOfRangeException("Invalid FVector3 index!");
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FVector3 FromRaw(long rawValueX, long rawValueY, long rawValueZ)
        {
            FVector3 r;
            r.RawX = rawValueX;
            r.RawY = rawValueY;
            r.RawZ = rawValueZ;
            return r;
        }

        public FVector3(FFloat x, FFloat y, FFloat z)
        {
            RawX = x.Raw;
            RawY = y.Raw;
            RawZ = z.Raw;
        }

        public FVector3(FFloat x, FFloat y)
        {
            RawX = x.Raw;
            RawY = y.Raw;
            RawZ = 0;
        }

        public static explicit operator FVector3(Vector3 value)
        {
            FVector3 r = FVector3.zero;
            
            r.x = (FFloat)value.x;
            r.y = (FFloat)value.y;
            r.z = (FFloat)value.z;

            return r;
        }

        public static explicit operator Vector3(FVector3 value)
        {
            return new Vector3((float)value.x, (float)value.y, (float)value.z);
        }

        public static explicit operator FVector3(FVector2 value)
        {
            return FVector3.FromRaw(value.RawX, value.RawY, 0L);
        }

        public static FVector3 operator -(FVector3 a)
        {
            return FromRaw(-a.RawX, -a.RawY, -a.RawZ);
        }

        public static FVector3 operator +(FVector3 a, FVector3 b)
        {
            long x = a.RawX + b.RawX;
            long y = a.RawY + b.RawY;
            long z = a.RawZ + b.RawZ;
            FVector3 r = FromRaw(x, y, z);
            return r;
        }

        public static FVector3 operator -(FVector3 a, FVector3 b)
        {
            long x = a.RawX - b.RawX;
            long y = a.RawY - b.RawY;
            long z = a.RawZ - b.RawZ;
            FVector3 r = FromRaw(x, y, z);
            return r;
        }

        public static FVector3 operator *(FVector3 a, FVector3 b)
        {
            long x = NativeFixedMath.Mul64(a.RawX, b.RawX);
            long y = NativeFixedMath.Mul64(a.RawY, b.RawY);
            long z = NativeFixedMath.Mul64(a.RawZ, b.RawZ);
            FVector3 r = FromRaw(x, y, z);
            return r;
        }

        public static FVector3 operator /(FVector3 a, FVector3 b)
        {
            long x = NativeFixedMath.Div64(a.RawX, b.RawX);
            long y = NativeFixedMath.Div64(a.RawY, b.RawY);
            long z = NativeFixedMath.Div64(a.RawZ, b.RawZ);
            FVector3 r = FromRaw(x, y, z);
            return r;
        }

        public static FVector3 operator *(FFloat a, FVector3 b)
        {
            return FromRaw(NativeFixedMath.Mul64(a.Raw, b.RawX), NativeFixedMath.Mul64(a.Raw, b.RawY), NativeFixedMath.Mul64(a.Raw, b.RawZ));
        }

        public static FVector3 operator *(FVector3 a, FFloat b)
        {
            return FromRaw(NativeFixedMath.Mul64(a.RawX, b.Raw), NativeFixedMath.Mul64(a.RawY, b.Raw), NativeFixedMath.Mul64(a.RawZ, b.Raw));
        }

        public static FVector3 operator /(FFloat a, FVector3 b)
        {
            return FromRaw(NativeFixedMath.Div64(a.Raw, b.RawX), NativeFixedMath.Div64(a.Raw, b.RawY), NativeFixedMath.Div64(a.Raw, b.RawZ));
        }

        public static FVector3 operator /(FVector3 a, FFloat b)
        {
            return FromRaw(NativeFixedMath.Div64(a.RawX, b.Raw), NativeFixedMath.Div64(a.RawY, b.Raw), NativeFixedMath.Div64(a.RawZ, b.Raw));
        }

        public static bool operator ==(FVector3 a, FVector3 b)
        {
            return a.RawX == b.RawX && a.RawY == b.RawY && a.RawZ == b.RawZ;
        }

        public static bool operator !=(FVector3 a, FVector3 b)
        {
            return a.RawX != b.RawX || a.RawY != b.RawY || a.RawZ != b.RawZ;
        }
        public FVector3 normalized
        {
            get
            {
                FVector3 result = FVector3.zero;
                NativeParallel3D.Vec3Normalize64(this, ref result);
                return result;
            }
        }

        public FVector2 xz
        {
            get
            {
                return new FVector2(x, z);
            }
        }

        public FFloat magnitude
        {
            get
            {
                FFloat result = FFloat.zero;
                NativeParallel3D.Vec3Length64(this, ref result);
                return result;
            }
        }

        public FFloat sqrMagnitude
        {
            get
            {
                return x*x + y*y + z*z;
            }
        }

        //Projects vector a onto vector b
        public static FVector3 Project(FVector3 a, FVector3 b)
        {
            FVector3 v;
            NativeFixedMath.Vec3_Project(a, b, out v);
            return v;
        }

        //Projects vector a ontp a plane defined by a normal orthogonal to the plane.
        public static FVector3 ProjectOnPlane(FVector3 a, FVector3 planeNormal)
        {
            FVector3 v;
            NativeFixedMath.Vec3_ProjectOnPlane(a, planeNormal, out v);
            return v;
        }

        public override string ToString()
        {
            return $"Fix64Vec3({x}, {y}, {z})";
        }

        public static FVector3 ClampMagnitude(FVector3 a, FFloat max)
        {
            if(a.sqrMagnitude > (max * max))
            {
                return a.normalized * max;
            }

            return a;
        }

        //Distance between two points
        public static FFloat Distance(FVector3 a, FVector3 b)
        {
            return (a - b).magnitude;
        }

        //Dot product of two vectors
        public static FFloat Dot(FVector3 a, FVector3 b)
        {
            FFloat v;
            NativeFixedMath.Vec3_Dot(a, b, out v);
            return v;
        }

        //Cross product of two vectors
        public static FVector3 Cross(FVector3 a, FVector3 b)
        {
            FVector3 v;
            NativeFixedMath.Vec3_Cross(a, b, out v);
            return v;
        }

        //Angle in degrees between two normalized vectors
        public static FFloat Angle(FVector3 a, FVector3 b)
        {
            FFloat dot = FVector3.Dot(a.normalized, b.normalized);
            FFloat clamped = FMath.Clamp(dot, -FFloat.one, FFloat.one);
            FFloat rad = FFloat.FromRaw(NativeFixedMath.Acos64(clamped.Raw));
            return rad * FMath.Rad2Deg;
        }

        public static FFloat SignedAngle(FVector3 from, FVector3 to, FVector3 axis)
        {
            FVector3 normalized = from.normalized;
            FVector3 normalized2 = to.normalized;
            FFloat num = FMath.Acos(FMath.Clamp(FVector3.Dot(normalized, normalized2), FFloat.negOne, FFloat.one)) * FMath.Rad2Deg;
            FFloat num2 = FMath.Sign(FVector3.Dot(axis, FVector3.Cross(normalized, normalized2)));
            return num * num2;
        }


        public static FVector3 Lerp(FVector3 a, FVector3 b, FFloat t)
        {
            t = FMath.Clamp01(t);
            return LerpUnclamped(a, b, t);
        }

        public static FVector3 LerpUnclamped(FVector3 a, FVector3 b, FFloat t)
        {
            return new FVector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        public static FVector3 MoveTowards(FVector3 current, FVector3 target, FFloat maxDistanceDelta)
        {
            FVector3 a = target - current;
            FFloat magnitude = a.magnitude;
            FVector3 result;
            if (magnitude <= maxDistanceDelta || magnitude < FFloat.err)
            {
                result = target;
            }
            else
            {
                result = current + a / magnitude * maxDistanceDelta;
            }
            return result;
        }

        //https://keithmaggio.wordpress.com/2011/02/15/math-magician-lerp-slerp-and-nlerp/
        public static FVector3 Slerp(FVector3 a, FVector3 b, FFloat t)
        {
            t = FMath.Clamp01(t);
            return SlerpUnclamped(a, b, t);
        }

        public static FVector3 SlerpUnclamped(FVector3 a, FVector3 b, FFloat t)
        {
            // Dot product - the cosine of the angle between 2 vectors.
            FFloat dot = FVector3.Dot(a, b);
            // Clamp it to be in the range of Acos()
            // This may be unnecessary, but floating point
            // precision can be a fickle mistress.
            FMath.Clamp(dot, FFloat.negOne, FFloat.one);
            // Acos(dot) returns the angle between start and end,
            // And multiplying that by percent returns the angle between
            // start and the final result.
            FFloat theta = FMath.Acos(dot) * t;
            FVector3 RelativeVec = b - a * dot;
            RelativeVec = RelativeVec.normalized;
            // Orthonormal basis
            // The final result.
            return ((a * FMath.Cos(theta)) + (RelativeVec * FMath.Sin(theta)));
        }

        public static FVector3 Nlerp(FVector3 start, FVector3 end, FFloat percent)
        {
            return Lerp(start, end, percent).normalized;
        }

        //https://forum.unity.com/threads/need-to-write-my-own-vector3-rotatetowards.155024/
        public static FVector3 RotateTowards(FVector3 current, FVector3 target, FFloat maxRadiansDelta, FFloat maxMagnitudeDelta)
        {
            // replicates Unity Vector3.RotateTowards
            FFloat delta = FVector3.Angle(current, target) * FMath.Deg2Rad;
            FFloat magDiff = target.magnitude - current.magnitude;
            FFloat sign = FMath.Sign(magDiff);
            FFloat maxMagDelta = FMath.Min(maxMagnitudeDelta, FMath.Abs(magDiff));
            FFloat diff = FMath.Min(FFloat.one, maxRadiansDelta / delta);
            return FVector3.SlerpUnclamped(current.normalized, target.normalized, diff) *
            (current.magnitude + maxMagDelta * sign);
        }

        public static FVector3 SmoothDamp(FVector3 current, FVector3 target, ref FVector3 currentVelocity, FFloat smoothTime, FFloat maxSpeed, FFloat deltaTime)
        {
            smoothTime = FMath.Max(FFloat.FromDivision(1, 10000), smoothTime);
            FFloat num = FFloat.two / smoothTime;
            FFloat num2 = num * deltaTime;
            FFloat d = FFloat.one / (FFloat.one + num2 + FFloat.FromDivision(48, 100) * num2 * num2 + FFloat.FromDivision(235, 1000) * num2 * num2 * num2);
            FVector3 vector = current - target;
            FVector3 vector2 = target;
            FFloat maxLength = maxSpeed * smoothTime;
            vector = FVector3.ClampMagnitude(vector, maxLength);
            target = current - vector;
            FVector3 vector3 = (currentVelocity + num * vector) * deltaTime;
            currentVelocity = (currentVelocity - num * vector3) * d;
            FVector3 vector4 = target + (vector + vector3) * d;
            if (FVector3.Dot(vector2 - current, vector4 - vector2) > FFloat.zero)
            {
                vector4 = vector2;
                currentVelocity = (vector4 - vector2) / deltaTime;
            }
            return vector4;
        }

        public void Set(FFloat newX, FFloat newY, FFloat newZ)
        {
            this.x = newX;
            this.y = newY;
            this.z = newZ;
        }

        public static FVector3 Scale(FVector3 a, FVector3 b)
        {
            return new FVector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public void Scale(FVector3 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
        }

        public static FVector3 Reflect(FVector3 inDirection, FVector3 inNormal)
        {
            return -FFloat.two * FVector3.Dot(inNormal, inDirection) * inNormal + inDirection;
        }

        public static FVector3 Normalize(FVector3 value)
        {
            FFloat num = FVector3.Magnitude(value);
            FVector3 result;
            if (num > FFloat.err)
            {
                result = value / num;
            }
            else
            {
                result = FVector3.zero;
            }
            return result;
        }

        public void Normalize()
        {
            FFloat num = FVector3.Magnitude(this);
            if (num > FFloat.err)
            {
                this /= num;
            }
            else
            {
                this = FVector3.zero;
            }
        }

        public static FFloat Magnitude(FVector3 vector)
        {
            return vector.magnitude;
        }

        public static FFloat SqrMagnitude(FVector3 vector)
        {
            return vector.sqrMagnitude;
        }

        public static FVector3 Min(FVector3 lhs, FVector3 rhs)
        {
            return new FVector3(FMath.Min(lhs.x, rhs.x), FMath.Min(lhs.y, rhs.y), FMath.Min(lhs.z, rhs.z));
        }

        public static FVector3 Max(FVector3 lhs, FVector3 rhs)
        {
            return new FVector3(FMath.Max(lhs.x, rhs.x), FMath.Max(lhs.y, rhs.y), FMath.Max(lhs.z, rhs.z));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FVector3))
                return false;
            return ((FVector3)obj) == this;
        }

        public override int GetHashCode()
        {
            return RawX.GetHashCode() ^ RawY.GetHashCode() * 7919 ^ RawZ.GetHashCode() * 4513;
        }
    }
}
