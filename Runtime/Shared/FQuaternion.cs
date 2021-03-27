using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Parallel
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 32)]
    public struct FQuaternion
    {
        //quaternion
        public static FQuaternion identity { get { return FromRaw(0L, 0L, 0L, 1L << FixedConstants64.SHIFT); } }

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
                        throw new IndexOutOfRangeException("Invalid FQuaternion index!");
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
                        throw new IndexOutOfRangeException("Invalid FQuaternion index!");
                }
            }
        }


        public FVector3 eulerAngles
        {
            get
            {
                return FQuaternion.ToEularRad(this) * FMath.Rad2Deg;
            }
            set
            {
                this = FQuaternion.FromEulerRad(value * FMath.Deg2Rad);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FQuaternion FromRaw(long rawValueX, long rawValueY, long rawValueZ, long rawValueW)
        {
            FQuaternion r;
            r.RawX = rawValueX;
            r.RawY = rawValueY;
            r.RawZ = rawValueZ;
            r.RawW = rawValueW;
            return r;
        }

        public FQuaternion(FFloat x, FFloat y, FFloat z, FFloat w)
        {
            RawX = x.Raw;
            RawY = y.Raw;
            RawZ = z.Raw;
            RawW = w.Raw;
        }

        public static explicit operator FQuaternion(Quaternion value)
        {
            FQuaternion r = FQuaternion.identity;

            r.x = (FFloat)value.x;
            r.y = (FFloat)value.y;
            r.z = (FFloat)value.z;
            r.w = (FFloat)value.w;

            return r;
        }

        public static explicit operator Quaternion(FQuaternion value)
        {
            return new Quaternion((float)value.x, (float)value.y, (float)value.z, (float)value.w);
        }

        public static FQuaternion Euler(FFloat x, FFloat y, FFloat z)
        {
            return FQuaternion.FromEulerRad(new FVector3(x, y, z) * FMath.Deg2Rad);
        }
        /// <summary>
        ///   <para>Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis (in that order).</para>
        /// </summary>
        /// <param name="euler"></param>
        public static FQuaternion Euler(FVector3 euler)
        {
            return FQuaternion.FromEulerRad(euler * FMath.Deg2Rad);
        }

        //https://gist.github.com/aeroson/043001ca12fe29ee911e
        public static FQuaternion FromEulerRad(FVector3 euler)
        {
            FFloat yaw = euler.x;
            FFloat pitch = euler.y;
            FFloat roll = euler.z;
            FFloat rollOver2 = roll * FFloat.half;
            FFloat sinRollOver2 = FMath.Sin(rollOver2);
            FFloat cosRollOver2 = FMath.Cos(rollOver2);
            FFloat pitchOver2 = pitch * FFloat.half;
            FFloat sinPitchOver2 = FMath.Sin(pitchOver2);
            FFloat cosPitchOver2 = FMath.Cos(pitchOver2);
            FFloat yawOver2 = yaw * FFloat.half;
            FFloat sinYawOver2 = FMath.Sin(yawOver2);
            FFloat cosYawOver2 = FMath.Cos(yawOver2);
            FQuaternion result = FQuaternion.identity;
            result.x = sinYawOver2 * cosPitchOver2 * cosRollOver2 + cosYawOver2 * sinPitchOver2 * sinRollOver2; // confirmed (scc+css)
            result.y = cosYawOver2 * sinPitchOver2 * cosRollOver2 - sinYawOver2 * cosPitchOver2 * sinRollOver2; // confirmed (csc-scs)
            result.z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2; // confirmed (ccs-ssc)
            result.w = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2; // confirmed (ccc+sss)
            return result;
        }

        // from http://stackoverflow.com/questions/12088610/conversion-between-euler-quaternion-like-in-unity3d-engine
        public static FVector3 ToEularRad(FQuaternion rotation)
        {
            FFloat sqw = rotation.w * rotation.w;
            FFloat sqx = rotation.x * rotation.x;
            FFloat sqy = rotation.y * rotation.y;
            FFloat sqz = rotation.z * rotation.z;
            FFloat unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            FFloat test = rotation.x * rotation.w - rotation.y * rotation.z;
            FVector3 v = FVector3.zero;

            if (test > FFloat.s0_4995 * unit)
            { // singularity at north pole
                v.y = FFloat.two * FMath.Atan2(rotation.y, rotation.x);
                v.x = FMath.PI / FFloat.two;
                v.z = FFloat.zero;
                return NormalizeAngles(v * FMath.Rad2Deg);
            }
            if (test < -FFloat.s0_4995 * unit)
            { // singularity at south pole
                v.y = -FFloat.two * FMath.Atan2(rotation.y, rotation.x);
                v.x = -FMath.PI / FFloat.two;
                v.z = FFloat.zero;
                return NormalizeAngles(v * FMath.Rad2Deg);
            }
            FQuaternion q = new FQuaternion(rotation.w, rotation.z, rotation.x, rotation.y);
            v.y = FMath.Atan2(FFloat.two * q.x * q.w + FFloat.two * q.y * q.z, FFloat.one - FFloat.two * (q.z * q.z + q.w * q.w));     // Yaw
            v.x = FMath.Asin(FFloat.two * (q.x * q.z - q.w * q.y));                             // Pitch
            v.z = FMath.Atan2(FFloat.two * q.x * q.y + FFloat.two * q.z * q.w, FFloat.one - FFloat.two * (q.y * q.y + q.z * q.z));      // Roll
            return v;
        }

        public static FVector3 NormalizeAngles(FVector3 angles)
        {
            angles.x = NormalizeAngle(angles.x);
            angles.y = NormalizeAngle(angles.y);
            angles.z = NormalizeAngle(angles.z);
            return angles;
        }
        private static FFloat NormalizeAngle(FFloat angle)
        {
            while (angle > FFloat.s360)
                angle -= FFloat.s360;
            while (angle < FFloat.zero)
                angle += FFloat.s360;
            return angle;
        }

        public static FQuaternion operator *(FQuaternion a, FQuaternion b)
        {
            FFloat q1x = a.x;
            FFloat q1y = a.y;
            FFloat q1z = a.z;
            FFloat q1w = a.w;

            FFloat q2x = b.x;
            FFloat q2y = b.y;
            FFloat q2z = b.z;
            FFloat q2w = b.w;

            // cross(av, bv)
            FFloat cx = q1y * q2z - q1z * q2y;
            FFloat cy = q1z * q2x - q1x * q2z;
            FFloat cz = q1x * q2y - q1y * q2x;

            FFloat dot = q1x * q2x + q1y * q2y + q1z * q2z;

            return new FQuaternion(
                q1x * q2w + q2x * q1w + cx,
                q1y * q2w + q2y * q1w + cy,
                q1z * q2w + q2z * q1w + cz,
                q1w * q2w - dot);
        }

        public static FVector3 operator *(FQuaternion rotation, FVector3 point)
        {
            FVector3 result = FVector3.zero;

            NativeFixedMath.Quat_Mul_Vec3(rotation, point, ref result);

            return result;
        }
        public static bool operator ==(FQuaternion a, FQuaternion b)
        {
            return a.RawX == b.RawX && a.RawY == b.RawY && a.RawZ == b.RawZ && a.RawW == b.RawW;
        }

        public static bool operator !=(FQuaternion a, FQuaternion b)
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

        public static FFloat Dot(FQuaternion a, FQuaternion b)
        {
            return (a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w);
        }

        //rad
        public static FQuaternion AngleAxis(FFloat angle, FVector3 axis)
        {
            if (axis.sqrMagnitude == FFloat.zero)
            {
                return identity;
            }

            var result = identity;
            angle *= FMath.Deg2Rad;
            angle *= FFloat.half;
            axis.Normalize();
            FVector3 v = axis * FMath.Sin(angle);
            result.x = v.x;
            result.y = v.y;
            result.z = v.z;
            result.w = FMath.Cos(angle);

            return Normalize(result);
        }

        public void ToAngleAxis(out FFloat angle, out FVector3 axis)
        {
            FQuaternion.ToAxisAngleRad(this, out axis, out angle);
            angle = angle * FMath.Rad2Deg;
        }

        private static void ToAxisAngleRad(FQuaternion q, out FVector3 axis, out FFloat angle)
        {
            if (FMath.Abs(q.w) > FFloat.one)
            {
                q.Normalize();
            }
                
            angle = FFloat.two * FMath.Acos(q.w); // angle
            FFloat den = FMath.Sqrt(FFloat.one - q.w * q.w);
            if (den > FFloat.err)
            {
                axis = new FVector3(q.x / den, q.y / den, q.z / den);
            }
            else
            {
                // This occurs when the angle is zero. 
                // Not a problem: just set an arbitrary normalized axis.
                axis = new FVector3(FFloat.one, FFloat.zero, FFloat.zero);
            }
        }

        public static FQuaternion FromToRotation(FVector3 fromDirection, FVector3 toDirection)
        {
            return FromTwoVectors(fromDirection, toDirection);
        }

        public void SetFromToRotation(FVector3 fromDirection, FVector3 toDirection)
        {
            this = FQuaternion.FromToRotation(fromDirection, toDirection);
        }

        public static FQuaternion LookRotation(FVector3 forward)
        {
            FVector3 up = FVector3.up;
            return FQuaternion.LookRotation(forward, up);
        }

        public void SetLookRotation(FVector3 view)
        {
            FVector3 up = FVector3.up;
            this.SetLookRotation(view, up);
        }

        public void SetLookRotation(FVector3 view, FVector3 up)
        {
            this = FQuaternion.LookRotation(view, up);
        }

        public FFloat GetXAngle()
        {
            //FFloat r21 = (FFloat)2 * (y * z + w * x);
            //return FFloat.Asin(r21);

            FFloat sinr_cosp = (FFloat)2 * (w * x + y * z);
            FFloat cosr_cosp = (FFloat)1 - (FFloat)2 * (x * x + y * y);
            return FMath.Atan2(sinr_cosp, cosr_cosp);
        }

        public FFloat GetYAngle()
        {
            //FFloat r31 = -(FFloat)2 * (x * z - w * y);
            //FFloat r32 = w * w - x * x - y * y + z * z;
            //return FFloat.Atan2(r31, r32);
            FFloat sinp = (FFloat)2 * (w * y - z * x);

            //if (FFloat.Abs(sinp) >= FFloat.one)
            //{
            //    FFloat sign = FFloat.Sign(sinp);
            //    return sign * FFloat.halfPi;
            //}

            return FMath.Asin(sinp);
        }

        public FFloat GetZAngle()
        {
            //FFloat r11 = -(FFloat)2 * (x * y - w * z);
            //FFloat r12 = w * w - x * x + y * y - z * z;
            //return FFloat.Atan2(r11, r12);
            FFloat siny_cosp = (FFloat)2 * (w * z + x * y);
            FFloat cosy_cosp = (FFloat)1 - (FFloat)2 * (y * y + z * z);
            return FMath.Atan2(siny_cosp, cosy_cosp);
        }

        //public FVector3 EulerAngles()
        //{
        //    FFloat xDegree = FFloat.RadToDeg(GetXAngle());
        //    FFloat yDegree = FFloat.RadToDeg(GetYAngle());
        //    FFloat zDegree = FFloat.RadToDeg(GetZAngle());

        //    return new FVector3(xDegree, yDegree, zDegree);
        //}

        //https://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToEuler/
        //public FVector3 EulerAngles1()
        //{
        //    FFloat sqw = w * w;
        //    FFloat sqx = x * x;
        //    FFloat sqy = y * y;
        //    FFloat sqz = z * z;
        //    FFloat unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
        //    FFloat test = x * y + z * w;

        //    FFloat _x = FFloat.zero;
        //    FFloat _y = FFloat.zero;
        //    FFloat _z = FFloat.zero;

        //    FFloat cutoff = FFloat.s0_4995 * unit;

        //    _z = FMath.Atan2((FFloat)2 * y * w - (FFloat)2 * x * z, sqx - sqy - sqz + sqw);
        //    _y = FMath.Asin((FFloat)2 * test / unit);
        //    _x = FMath.Atan2((FFloat)2 * x * w - (FFloat)2 * y * z, -sqx + sqy - sqz + sqw);

        //    //if (test >= cutoff)
        //    //{ // singularity at north pole
        //    //    _z = (FFloat)2 * FFloat.Atan2(x, w);
        //    //    _y = FFloat.halfPi;
        //    //    _x = FFloat.zero;
        //    //}
        //    //else if (test < -cutoff)
        //    //{ // singularity at south pole
        //    //    _z = -(FFloat)2 * FFloat.Atan2(x, w);
        //    //    _y = -FFloat.halfPi;
        //    //    _x = FFloat.zero;
        //    //}
        //    //else
        //    //{
        //    //    _z = FFloat.Atan2((FFloat)2 * y * w - (FFloat)2 * x * z, sqx - sqy - sqz + sqw);
        //    //    _y = FFloat.Asin((FFloat)2 * test / unit);
        //    //    _x = FFloat.Atan2((FFloat)2 * x * w - (FFloat)2 * y * z, -sqx + sqy - sqz + sqw);
        //    //}
        //    FFloat xDegree = FFloat.RadToDeg(_x);
        //    FFloat yDegree = FFloat.RadToDeg(_y);
        //    FFloat zDegree = FFloat.RadToDeg(_z);

        //    return new FVector3(xDegree, zDegree, yDegree);
        //}




        //public static FQuaternion FromEulerAngles(FVector3 value)
        //{
        //    FFloat yaw_y = FFloat.DegToRad(value.y);
        //    FFloat pitch_x = FFloat.DegToRad(value.x);
        //    FFloat roll_z = FFloat.DegToRad(value.z);

        //    return FQuaternion.FromYawPitchRoll(yaw_y, pitch_x, roll_z);
        //}


        //https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
        //public static FQuaternion FromYawPitchRoll(FFloat yaw_y, FFloat pitch_x, FFloat roll_z)
        //{
        //    //FFloat half_yaw = FFloat.Div2(yaw_y);
        //    //FFloat cy = FFloat.Cos(half_yaw);
        //    //FFloat sy = FFloat.Sin(half_yaw);

        //    //FFloat half_pitch = FFloat.Div2(pitch_x);
        //    //FFloat cp = FFloat.Cos(half_pitch);
        //    //FFloat sp = FFloat.Sin(half_pitch);

        //    //FFloat half_roll = FFloat.Div2(roll_z);
        //    //FFloat cr = FFloat.Cos(half_roll);
        //    //FFloat sr = FFloat.Sin(half_roll);

        //    //FFloat w = cr * cp * cy + sr * sp * sy;
        //    //FFloat x = sr * cp * cy - cr * sp * sy;
        //    //FFloat y = cr * sp * cy + sr * cp * sy;
        //    //FFloat z = cr * cp * sy - sr * sp * cy;

        //    //return new FQuaternion(x, y, z, w);

        //    FFloat half_roll = FMath.Div2(roll_z);
        //    FFloat sr = FMath.Sin(half_roll);
        //    FFloat cr = FMath.Cos(half_roll);

        //    FFloat half_pitch = FMath.Div2(pitch_x);
        //    FFloat sp = FMath.Sin(half_pitch);
        //    FFloat cp = FMath.Cos(half_pitch);

        //    FFloat half_yaw = FMath.Div2(yaw_y);
        //    FFloat sy = FMath.Sin(half_yaw);
        //    FFloat cy = FMath.Cos(half_yaw);

        //    return new FQuaternion(
        //        cy * sp * cr + sy * cp * sr,
        //        sy * cp * cr - cy * sp * sr,
        //        cy * cp * sr - sy * sp * cr,
        //        cy * cp * cr + sy * sp * sr);
        //}

        public FFloat Length
        {
            get
            {
                return FMath.Sqrt(x * x + y * y + z * z + w * w);
            }
        }

        public FFloat LengthSquared
        {
            get
            {
                return x * x + y * y + z * z + w * w;
            }
        }

        public void Normalize()
        {
            var scale = FFloat.one / Length;
            x *= scale;
            y *= scale;
            z *= scale;
            w *= scale;
        }

        public static FQuaternion Normalize(FQuaternion q)
        {
            FQuaternion result = FQuaternion.identity;
            FFloat scale = FFloat.one / q.Length;

            return new FQuaternion(q.x * scale, q.y * scale, q.z * scale, q.w * scale);
        }

        public static FFloat Angle(FQuaternion a, FQuaternion b)
        {
            return FMath.Acos(FMath.Min(FMath.Abs(FQuaternion.Dot(a, b)), FFloat.one)) * FFloat.two * FMath.Rad2Deg;
        }

        //public static FQuaternion Normalize(FQuaternion a)
        //{
        //    long inv_norm = NativeFixedMath.Rcp64(LengthFastest(a).Raw);
        //    FFloat x = FFloat.FromRaw(NativeFixedMath.Mul64(a.RawX, inv_norm));
        //    FFloat y = FFloat.FromRaw(NativeFixedMath.Mul64(a.RawY, inv_norm));
        //    FFloat z = FFloat.FromRaw(NativeFixedMath.Mul64(a.RawZ, inv_norm));
        //    FFloat w = FFloat.FromRaw(NativeFixedMath.Mul64(a.RawW, inv_norm));

        //    return new FQuaternion(x, y, z, w);
        //}

        public static FFloat LengthSqr(FQuaternion a)
        {
            return FFloat.FromRaw(NativeFixedMath.Mul64(a.RawX, a.RawX) 
                + NativeFixedMath.Mul64(a.RawY, a.RawY) 
                + NativeFixedMath.Mul64(a.RawZ, a.RawZ) 
                + NativeFixedMath.Mul64(a.RawW, a.RawW));
        }

        public static FFloat LengthFastest(FQuaternion a)
        {
            return FFloat.FromRaw(NativeFixedMath.Sqrt64(LengthSqr(a).Raw));
        }

        public static FQuaternion Inverse(FQuaternion a)
        {
            long inv_norm = NativeFixedMath.Rcp64(LengthSqr(a).Raw);
            return FromRaw(
                -NativeFixedMath.Mul64(a.RawX, inv_norm),
                -NativeFixedMath.Mul64(a.RawY, inv_norm),
                -NativeFixedMath.Mul64(a.RawZ, inv_norm),
                NativeFixedMath.Mul64(a.RawW, inv_norm));

            //FFloat ls = a.x * a.x + a.y * a.y + a.z * a.z + a.w * a.w;
            //FFloat invNorm = FFloat.one / ls;

            //return new FQuaternion(
            //    -a.x * invNorm,
            //    -a.y * invNorm,
            //    -a.z * invNorm,
            //    a.w * invNorm);
        }

        public static FQuaternion FromTwoVectors(FVector3 a, FVector3 b)
        { // From: http://lolengine.net/blog/2014/02/24/quaternion-from-two-vectors-final

            FFloat norm_a_norm_b = FMath.Sqrt(FVector3.Dot(a, a) * FVector3.Dot(b, b));
            FFloat real_part = norm_a_norm_b + FVector3.Dot(a, b);

            FVector3 v = FVector3.zero;

            if (real_part < (FFloat.err * norm_a_norm_b))
            {
                /* If u and v are exactly opposite, rotate 180 degrees
                 * around an arbitrary orthogonal axis. Axis normalization
                 * can happen later, when we normalize the quaternion. */
                real_part = FFloat.zero;

                v = FMath.Abs(a.x) > FMath.Abs(a.z) ? new FVector3(-a.y, a.x, FFloat.zero)
                                                    : new FVector3(FFloat.zero, -a.z, a.y);
            }
            else
            {
                /* Otherwise, build quaternion the standard way. */
                v = FVector3.Cross(a, b);
            }

            return Normalize(new FQuaternion(v.x, v.y, v.z, real_part));
        }

        //normalize dir
        public static FQuaternion LookRotation(FVector3 dir, FVector3 up)
        { // From: https://answers.unity.com/questions/819699/calculate-quaternionlookrotation-manually.html
            if (dir == FVector3.zero)
                return identity;

            if (up != dir)
            {
                FVector3 v = dir + up * -FVector3.Dot(up, dir);
                FQuaternion q = FromTwoVectors(FVector3.forward, v);
                return FromTwoVectors(v, dir) * q;
            }
            else
                return FromTwoVectors(FVector3.forward, dir);
        }

        public static FQuaternion Slerp(FQuaternion q1, FQuaternion q2, FFloat t)
        {
            t = FMath.Clamp01(t);
            return SlerpUnclamped(q1, q2, t);
        }
        public static FQuaternion SlerpUnclamped(FQuaternion q1, FQuaternion q2, FFloat t)
        {
            FFloat epsilon = FFloat.err;
            FFloat cos_omega = q1.x * q2.x + q1.y * q2.y + q1.z * q2.z + q1.w * q2.w;

            bool flip = false;

            if (cos_omega < FFloat.zero)
            {
                flip = true;
                cos_omega = -cos_omega;
            }

            FFloat s1, s2;
            if (cos_omega > (FFloat.one - epsilon))
            {
                // Too close, do straight linear interpolation.
                s1 = FFloat.one - t;
                s2 = (flip) ? -t : t;
            }
            else
            {
                FFloat omega = FFloat.FromRaw(NativeFixedMath.Acos64(cos_omega.Raw));

                FFloat inv_sin_omega = FFloat.one / FFloat.FromRaw(NativeFixedMath.Sin64(omega.Raw));

                FFloat v1 = (FFloat.one - t) * omega;
                FFloat v2 = t * omega;

                s1 = FFloat.FromRaw(NativeFixedMath.Sin64(v1.Raw)) * inv_sin_omega;
                s2 = FFloat.FromRaw(NativeFixedMath.Sin64(v2.Raw)) * inv_sin_omega;
                s2 = (flip) ? -s2 : s2;
            }

            return new FQuaternion(
                s1 * q1.x + s2 * q2.x,
                s1 * q1.y + s2 * q2.y,
                s1 * q1.z + s2 * q2.z,
                s1 * q1.w + s2 * q2.w);
        }

        public static FQuaternion Lerp(FQuaternion q1, FQuaternion q2, FFloat t)
        {
            t = FMath.Clamp01(t);
            return LerpUnclamped(q1, q2, t);
        }
        
        //https://github.com/shuningzhou/referencesource/blob/master/System.Numerics/System/Numerics/Quaternion.cs
        public static FQuaternion LerpUnclamped(FQuaternion q1, FQuaternion q2, FFloat t)
        {
            FFloat t1 = FFloat.one - t;

            FQuaternion r = FQuaternion.identity;

            FFloat dot = q1.x * q2.x + q1.y * q2.y +
                        q1.z * q2.z + q1.w * q2.w;

            if (dot >= FFloat.zero)
            {
                r.x = t1 * q1.x + t * q2.x;
                r.y = t1 * q1.y + t * q2.y;
                r.z = t1 * q1.z + t * q2.z;
                r.w = t1 * q1.w + t * q2.w;
            }
            else
            {
                r.x = t1 * q1.x - t * q2.x;
                r.y = t1 * q1.y - t * q2.y;
                r.z = t1 * q1.z - t * q2.z;
                r.w = t1 * q1.w - t * q2.w;
            }

            // Normalize it.
            FFloat ls = r.x * r.x + r.y * r.y + r.z * r.z + r.w * r.w;
            FFloat invNorm = FFloat.one / FMath.Sqrt(ls);

            r.x *= invNorm;
            r.y *= invNorm;
            r.z *= invNorm;
            r.w *= invNorm;

            return r;
        }

        public static FQuaternion RotateTowards(FQuaternion from, FQuaternion to, FFloat maxDegreesDelta)
        {
            FFloat num = FQuaternion.Angle(from, to);
            if (num == FFloat.zero)
                return to;
            FFloat t =FMath.Min(FFloat.one, maxDegreesDelta / num);
            return FQuaternion.SlerpUnclamped(from, to, t);
        }

        public override string ToString()
        {
            return $"Fix64Quat({x}, {y}, {z}, {w})";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FQuaternion))
                return false;
            return ((FQuaternion)obj) == this;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() * 7919) ^ (z.GetHashCode() * 4513) ^ (w.GetHashCode() * 8923);
        }
    }
}