using UnityEngine;

namespace Parallel
{
    public class ParallelSpringJoint3D : ParallelJoint3D
    {
        public bool collideConnected;
        public ParallelRigidbody3D connectedRigidBody;
        public Fix64Vec3 anchor;
        public Fix64Vec3 connectedAnchor;

        public Fix64 distance;
        public Fix64 frequency;
        public Fix64 dampingRatio;

        protected override PJoint3D CreateJoint()
        {
            ParallelRigidbody3D self = GetComponent<ParallelRigidbody3D>();
            rbA = self;
            if(connectedRigidBody == null)
            {
                rbBIsNull = true;
            }
            else
            {
                rbB = connectedRigidBody;
            }
            return Parallel3D.CreateSprintJoint(self, connectedRigidBody, anchor, connectedAnchor, collideConnected, frequency, dampingRatio);
        }
    }
}
