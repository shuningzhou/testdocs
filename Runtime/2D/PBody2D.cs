using System;
namespace Parallel
{
    public struct PBodyExport2D
    {
        public Fix64Vec2 linearVelocity;
        public Fix64 angularVelocity;

        public Fix64Vec2 position;
        public Fix64 angle;
    }

    [Serializable]
    public class PBody2D : NativeObject
    {
        public Fix64Vec2 position;
        public Fix64 angle;

        internal Fix64Vec2 linearVelocity;
        internal Fix64 angularVelocity;

        public Fix64 mass;

        public UInt16 BodyID { get; private set; }

        public IParallelRigidbody2D RigidBody { get; private set; }

        public PBody2D(IntPtr intPtr, UInt16 bodyID, IParallelRigidbody2D rigidBody) : base(intPtr)
        {
            BodyID = bodyID;
            RigidBody = rigidBody;
        }

        public void ReadNative()
        {
            Parallel2D.ReadNativeBody(this);
            RigidBody.OnTransformUpdated();
        }

        public void Step(Fix64 time)
        {
            RigidBody.Step(time);
        }
    }
}
