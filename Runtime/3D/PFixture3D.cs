using System;
namespace Parallel
{
    public class PFixture3D : NativeObject
    {
        public byte shapeID { get; private set; }
        public PFixture3D(byte id, IntPtr intPtr) : base(intPtr)
        {
            shapeID = id;
        }
    }
}
