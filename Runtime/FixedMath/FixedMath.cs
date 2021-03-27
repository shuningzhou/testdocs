using UnityEngine;
using System;
using System.Runtime.InteropServices;
using AOT;

namespace Parallel
{
    internal class NativeFixedMath
    {
        // Name of the plugin when using [DllImport]
#if !UNITY_EDITOR && UNITY_IOS
		const string PLUGIN_NAME = "__Internal";
#else
        const string PLUGIN_NAME = "fixedMath";
#endif
        //fixed point
        [DllImport(PLUGIN_NAME)]
        internal static extern long Rcp64(long a);

        [DllImport(PLUGIN_NAME)]
        internal static extern long Sign64(long a);

        [DllImport(PLUGIN_NAME)]
        internal static extern long Sqrt64(long a);

        [DllImport(PLUGIN_NAME)]
        internal static extern long Abs64(long a);

        [DllImport(PLUGIN_NAME)]
        internal static extern long Mul64(long a, long b);

        [DllImport(PLUGIN_NAME)]
        internal static extern long Div64(long a, long b);

        [DllImport(PLUGIN_NAME)]
        internal static extern long Pow64(long a, long b);

        [DllImport(PLUGIN_NAME)]
        internal static extern long Log264(long a);

        [DllImport(PLUGIN_NAME)]
        internal static extern long Log64(long a);

        [DllImport(PLUGIN_NAME)]
        internal static extern long Mod64(long a, long b);

        [DllImport(PLUGIN_NAME)]
        internal static extern long Tan64(long a);

        [DllImport(PLUGIN_NAME)]
        internal static extern long Atan64(long a);

        [DllImport(PLUGIN_NAME)]
        internal static extern long Atan264(long a, long b);

        [DllImport(PLUGIN_NAME)]
        internal static extern long Asin64(long a);

        [DllImport(PLUGIN_NAME)]
        internal static extern long Sin64(long a);

        [DllImport(PLUGIN_NAME)]
        internal static extern long Cos64(long a);

        [DllImport(PLUGIN_NAME)]
        internal static extern long Acos64(long a);

        //vec3
        [DllImport(PLUGIN_NAME)]
        internal static extern void Vec3_Dot(FVector3 a, FVector3 b, out FFloat result);
        [DllImport(PLUGIN_NAME)]
        internal static extern void Vec3_Cross(FVector3 a, FVector3 b, out FVector3 result);

        [DllImport(PLUGIN_NAME)]
        internal static extern void Vec3_ProjectOnPlane(FVector3 a, FVector3 planeNormal, out FVector3 result);

        [DllImport(PLUGIN_NAME)]
        internal static extern void Vec3_Project(FVector3 a, FVector3 b, out FVector3 result);

        //quaternion
        [DllImport(PLUGIN_NAME)]
        internal static extern void Quat_Mul_Vec3(FQuaternion rotation, FVector3 point, ref FVector3 result);

        //matrix4x4
        [DllImport(PLUGIN_NAME)]
        internal static extern void Mat4x4_Mul_Point_3x4(FMatrix4x4 mat, FVector3 v, ref FVector3 result);

        [DllImport(PLUGIN_NAME)]
        internal static extern void Mat4x4_Mul(FMatrix4x4 lhs, FMatrix4x4 rhs, out FMatrix4x4 result);

        [DllImport(PLUGIN_NAME)]
        internal static extern bool Mat4x4_Invert(FMatrix4x4 m, out FMatrix4x4 result);

        [DllImport(PLUGIN_NAME)]
        internal static extern void Mat4x4_TRS(FVector3 pos, FQuaternion q, FVector3 s, out FMatrix4x4 result);
    }
}