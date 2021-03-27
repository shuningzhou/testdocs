using UnityEngine;

namespace Parallel
{
    public class ParallelHingeJoint3D : ParallelJoint3D
    {
        public bool collideConnected;
        public ParallelRigidbody3D connectedRigidBody;
        public FVector3 worldAnchor;
        public FVector3 worldAxis;

        public bool useMotor;
        public FFloat motorSpeed;
        public FFloat motorTorque;

        public bool useLimit;
        public FFloat lowerAngle;
        public FFloat upperAngle;

        protected override PJoint3D CreateJoint()
        {
            ParallelRigidbody3D self = GetComponent<ParallelRigidbody3D>();
            rbA = self;
            if (connectedRigidBody == null)
            {
                rbBIsNull = true;
            }
            else
            {
                rbB = connectedRigidBody;
            }
            return Parallel3D.CreateHingeJoint(self, connectedRigidBody, worldAnchor, worldAxis, collideConnected, useLimit, FFloat.DegToRad(lowerAngle), FFloat.DegToRad(upperAngle), useMotor, FFloat.DegToRad(motorSpeed), motorTorque);
        }
    }
}
