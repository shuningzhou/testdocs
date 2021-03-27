using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Parallel
{
    //column-major order.
    public struct FMatrix3x3
    {
        public FVector3 x;
        public FVector3 y;
        public FVector3 z;

        public FMatrix3x3(FQuaternion quat)
        {
            FFloat _x = quat.x;
            FFloat _y = quat.y;
            FFloat _z = quat.z;
            FFloat _w = quat.w;

            FFloat x2 = _x + _x, y2 = _y + _y, z2 = _z + _z;
            FFloat xx = _x * x2, xy = _x * y2, xz = _x * z2;
            FFloat yy = _y * y2, yz = _y * z2, zz = _z * z2;
            FFloat wx = _w * x2, wy = _w * y2, wz = _w * z2;

            x = new FVector3(FFloat.one - (yy + zz), xy + wz, xz - wy);
			y = new FVector3(xy - wz, FFloat.one - (xx + zz), yz + wx);
			z = new FVector3(xz + wy, yz - wx, FFloat.one - (xx + yy));
        }
    }
}
