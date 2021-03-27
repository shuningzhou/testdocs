using System;
namespace Parallel
{
    public struct PBodyExport2D
    {
        public FVector2 linearVelocity;
        public FFloat angularVelocity;

        public FVector2 position;
        public FFloat angle;
    }

    [Serializable]
    public class PBody2D : NativeObject
    {
        public FVector2 position;
        public FFloat angle;

        internal FVector2 linearVelocity;
        internal FFloat angularVelocity;

        public FFloat mass;

        public UInt16 BodyID { get; private set; }
        public UInt32 ExternalID { get; private set; }

        public IParallelRigidbody2D RigidBody { get; private set; }

        public PBody2D(IntPtr intPtr, UInt16 bodyID, UInt32 externalID, IParallelRigidbody2D rigidBody) : base(intPtr)
        {
            BodyID = bodyID;
            ExternalID = externalID;
            RigidBody = rigidBody;
        }

        public void ReadNative()
        {
            Parallel2D.ReadNativeBody(this);
            RigidBody.OnTransformUpdated();
        }

        public void Step(FFloat time)
        {
            RigidBody.Step(time);
        }
    }
}
