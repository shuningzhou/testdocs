using UnityEngine;

namespace Parallel
{
    public class ParallelHingeJoint3D : ParallelJoint3D
    {
        public bool collideConnected;
        public ParallelRigidbody3D connectedRigidBody;
        public Fix64Vec3 worldAnchor;
        public Fix64Vec3 worldAxis;

        public bool useMotor;
        public Fix64 motorSpeed;
        public Fix64 motorTorque;

        public bool useLimit;
        public Fix64 lowerAngle;
        public Fix64 upperAngle;

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
            return Parallel3D.CreateHingeJoint(self, connectedRigidBody, worldAnchor, worldAxis, collideConnected, useLimit, Fix64.DegToRad(lowerAngle), Fix64.DegToRad(upperAngle), useMotor, Fix64.DegToRad(motorSpeed), motorTorque);
        }
    }
}
