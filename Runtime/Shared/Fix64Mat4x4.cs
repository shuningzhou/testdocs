//using System;
//using System.Runtime.CompilerServices;
//using System.Runtime.InteropServices;
//using UnityEngine;

//namespace Parallel
//{
//    //column-major order.
//    public struct Fix64Mat4x4
//    {
//        public static Fix64Mat4x4 identity
//        {
//            get
//            {
//                FVector4 _x = new FVector4(FFloat.one, FFloat.zero, FFloat.zero, FFloat.zero);
//                FVector4 _y = new FVector4(FFloat.zero, FFloat.one, FFloat.zero, FFloat.zero);
//                FVector4 _z = new FVector4(FFloat.zero, FFloat.zero, FFloat.one, FFloat.zero);
//                FVector4 _w = new FVector4(FFloat.zero, FFloat.zero, FFloat.zero, FFloat.one);

//                return new Fix64Mat4x4(_x, _y, _z, _w);
//            }
//        }

//        public FVector4 x;
//        public FVector4 y;
//        public FVector4 z;
//        public FVector4 w;

//        public Fix64Mat4x4(FVector4 _x, FVector4 _y, FVector4 _z, FVector4 _w)
//        {
//            x = _x;
//            y = _y;
//            z = _z;
//            w = _w;
//        }

//        //https://stackoverflow.com/a/51586282/1368748
//        public static Fix64Mat4x4 FromTRS(FVector3 translation, FQuaternion rotation, FVector3 scale)
//        {
//            FFloat x1 = translation.x;
//            FFloat y1 = translation.y;
//            FFloat z1 = translation.z;
//            FFloat x2 = scale.x;
//            FFloat y2 = scale.y;
//            FFloat z2 = scale.z;

//            FMatrix3x3 rot = new FMatrix3x3(rotation);
//            FFloat a11 = rot.x.x;
//            FFloat a21 = rot.x.y;
//            FFloat a31 = rot.x.z;
//            FFloat a12 = rot.y.x;
//            FFloat a22 = rot.y.y;
//            FFloat a32 = rot.y.z;
//            FFloat a13 = rot.z.x;
//            FFloat a23 = rot.z.y;
//            FFloat a33 = rot.z.z;


//            FVector4 _x = new FVector4(x2 * a11, x2 * a21, x2 * a31, FFloat.zero);
//            FVector4 _y = new FVector4(y2 * a12, y2 * a22, y2 * a32, FFloat.zero);
//            FVector4 _z = new FVector4(z2 * a13, z2 * a23, z2 * a33, FFloat.zero);
//            FVector4 _w = new FVector4(x1, y1, z1, FFloat.one);

//            return new Fix64Mat4x4(_x, _y, _z, _w);
//        }

//        public static FVector4 operator *(Fix64Mat4x4 a, FVector4 v)
//        {
//            FVector4 r = v.x * a.x + v.y * a.y + v.z * a.z + v.w * a.w;
//            return r;
//        }

//        public static Fix64Mat4x4 operator *(Fix64Mat4x4 a, Fix64Mat4x4 b)
//        {
//            FVector4 _x = a * b.x;
//            FVector4 _y = a * b.y;
//            FVector4 _z = a * b.z;
//            FVector4 _w = a * b.w;

//            return new Fix64Mat4x4(_x, _y, _z, _w);
//        }


//        public FVector3 MultiplyVector(FVector3 v)
//        {
//            FFloat m00 = x.x, m01 = y.x, m02 = z.x;
//            FFloat m10 = x.y, m11 = y.y, m12 = z.y;
//            FFloat m20 = x.z, m21 = y.z, m22 = z.z;
//            //FFloat m30 = x.w, m31 = y.w, m32 = z.w;

//            FFloat _x = v.x * m00 + v.y * m01 + v.z * m02;
//            FFloat _y = v.x * m10 + v.y * m11 + v.z * m12;
//            FFloat _z = v.x * m20 + v.y * m21 + v.z * m22;

//            return new FVector3(_x, _y, _z);
//        }

//        public FVector3 MultiplyPoint3x4(FVector3 v)
//        {
//            FFloat m00 = x.x, m01 = y.x, m02 = z.x, m03 = w.x;
//            FFloat m10 = x.y, m11 = y.y, m12 = z.y, m13 = w.y;
//            FFloat m20 = x.z, m21 = y.z, m22 = z.z, m23 = w.z;
//            //FFloat m30 = x.w, m31 = y.w, m32 = z.w, m33 = w.w;

//            FFloat _x = v.x * m00 + v.y * m01 + v.z * m02 + m03;
//            FFloat _y = v.x * m10 + v.y * m11 + v.z * m12 + m13;
//            FFloat _z = v.x * m20 + v.y * m21 + v.z * m22 + m23;

//            return new FVector3(_x, _y, _z);
//        }

//        public override string ToString()
//        {
//            return
//                $"{x.x}, {y.x}, {z.x}, {w.x}\n" +
//                $"{x.y}, {y.y}, {z.y}, {w.y}\n" +
//                $"{x.z}, {y.z}, {z.z}, {w.z}\n" +
//                $"{x.w}, {y.w}, {z.w}, {w.w}\n";
//        }
//    }
//}