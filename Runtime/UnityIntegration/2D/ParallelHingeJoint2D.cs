using UnityEngine;

namespace Parallel
{
    public class ParallelHingeJoint2D : ParallelJoint2D
    {
        public bool collideConnected;
        public ParallelRigidbody2D connectedRigidBody;
        public FVector2 worldAnchor;

        public bool useMotor;
        public FFloat motorSpeed;
        public FFloat motorTorque;

        public bool useLimites;
        public FFloat lowerAngle;
        public FFloat upperAngle;

        protected override PJoint2D CreateJoint()
        {
            ParallelRigidbody2D self = GetComponent<ParallelRigidbody2D>();

            return Parallel2D.CreateHingeJoint(self, connectedRigidBody, worldAnchor, collideConnected, useLimites, FFloat.DegToRad(lowerAngle), FFloat.DegToRad(upperAngle), useMotor, FFloat.DegToRad(motorSpeed), motorTorque);
        }
    }
}
