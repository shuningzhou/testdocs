using UnityEngine;

namespace Parallel
{
    public class ParallelHingeJoint2D : ParallelJoint2D
    {
        public bool collideConnected;
        public ParallelRigidbody2D connectedRigidBody;
        public Fix64Vec2 worldAnchor;

        public bool useMotor;
        public Fix64 motorSpeed;
        public Fix64 motorTorque;

        public bool useLimites;
        public Fix64 lowerAngle;
        public Fix64 upperAngle;

        protected override PJoint2D CreateJoint()
        {
            ParallelRigidbody2D self = GetComponent<ParallelRigidbody2D>();

            return Parallel2D.CreateHingeJoint(self, connectedRigidBody, worldAnchor, collideConnected, useLimites, Fix64.DegToRad(lowerAngle), Fix64.DegToRad(upperAngle), useMotor, Fix64.DegToRad(motorSpeed), motorTorque);
        }
    }
}
