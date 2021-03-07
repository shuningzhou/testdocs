using System;
namespace Parallel
{
    public struct PBodyExport3D
    {
        public Fix64Vec3 linearVelocity;
        public Fix64Vec3 angularVelocity;

        public Fix64Vec3 position;
        public Fix64Quat orientation;
        public Fix64Quat orientation0;
    }

    public class PBody3D : NativeObject
    {
        public bool awake;
        public Fix64 sleepTime;
        public Fix64Vec3 position;
        public Fix64Quat orientation;
        public Fix64Quat orientation0;

        public Fix64Vec3 linearVelocity;
        public Fix64Vec3 angularVelocity;

        public Fix64 mass;

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

        public void Step(Fix64 time)
        {
            RigidBody.Step(time);
        }
    }
}
