using System;
namespace Parallel
{
    public struct PBodyExport3D
    {
        public FVector3 linearVelocity;
        public FVector3 angularVelocity;

        public FVector3 position;
        public FQuaternion orientation;
        public FQuaternion orientation0;
    }

    public class PBody3D : NativeObject
    {
        public bool awake;
        public FFloat sleepTime;
        public FVector3 position;
        public FQuaternion orientation;
        public FQuaternion orientation0;

        public FVector3 linearVelocity;
        public FVector3 angularVelocity;

        public FFloat mass;

        public UInt16 BodyID { get; private set; }
        public UInt32 ExternalID { get; private set; }

        public IParallelRigidbody3D RigidBody { get; private set; }
        public bool IsAwake
        {
            get{
                return awake;
            }
        }

        public PBody3D(IntPtr intPtr, UInt16 bodyID, UInt32 externalID, IParallelRigidbody3D rigidBody) : base(intPtr)
        {
            BodyID = bodyID;
            ExternalID = externalID;

            RigidBody = rigidBody;

            awake = true;
        }

        public void ReadNative()
        {
            Parallel3D.ReadNativeBody(this);
            RigidBody.OnTransformUpdated();
        }

        public void Step(FFloat time)
        {
            RigidBody.Step(time);
        }
    }
}
