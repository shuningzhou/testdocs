using UnityEngine;

namespace Parallel
{
    public class ParallelSpringJoint3D : ParallelJoint3D
    {
        public bool collideConnected;
        public ParallelRigidbody3D connectedRigidBody;
        public FVector3 anchor;
        public FVector3 connectedAnchor;

        public FFloat distance;
        public FFloat frequency;
        public FFloat dampingRatio;

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
