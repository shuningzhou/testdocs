using UnityEngine;

namespace Parallel
{
    public class ParallelSpringJoint2D : ParallelJoint2D
    {
        public bool collideConnected;
        public ParallelRigidbody2D connectedRigidBody;
        public FVector2 anchor;
        public FVector2 connectedAnchor;

        public FFloat distance;
        public FFloat frequency;
        public FFloat dampingRatio;

        protected override PJoint2D CreateJoint()
        {
            ParallelRigidbody2D self = GetComponent<ParallelRigidbody2D>();
            return Parallel2D.CreateSprintJoint(self, connectedRigidBody, anchor, connectedAnchor, collideConnected, frequency, dampingRatio);
        }
    }
}
