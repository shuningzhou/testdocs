using System;
namespace Parallel
{
    public class PContact3D
    {
        internal ContactState state;

        internal UInt64 ContactID { get; private set; }
        internal UInt16 Body1ID { get; private set; }
        internal UInt16 Body2ID { get; private set; }

        internal byte Shape1ID { get; private set; }
        internal byte Shape2ID { get; private set; }

        internal FVector3 RelativeVelocity { get; private set; }
        internal IntPtr IntPointer { get; private set; }
        internal bool IsTrigger { get; private set; }

        public void Update(
            IntPtr nativeHandle,
            FVector3 relativeVelocity,
            bool isTrigger
            )
        {
            IntPointer = nativeHandle;
            RelativeVelocity = relativeVelocity;
            IsTrigger = isTrigger;
        }

        public PContact3D(UInt64 contactID)
        {
            ContactID = contactID;
            UInt32 bodyIds = (UInt32)(ContactID / 1000000);

            Body1ID = (UInt16)(bodyIds % 100000);
            Body2ID = (UInt16)(bodyIds / 100000);

            UInt32 shapeIds = (UInt32)(ContactID % 1000000);

            Shape1ID = (byte)(shapeIds % 1000);
            Shape2ID = (byte)(shapeIds / 1000);
        }
    }
}
