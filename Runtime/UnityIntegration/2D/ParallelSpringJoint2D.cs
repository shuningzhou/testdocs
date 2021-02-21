using UnityEngine;

namespace Parallel
{
    public class ParallelSpringJoint2D : ParallelJoint2D
    {
        public bool collideConnected;
        public ParallelRigidbody2D connectedRigidBody;
        public Fix64Vec2 anchor;
        public Fix64Vec2 connectedAnchor;

        public Fix64 distance;
        public Fix64 frequency;
        public Fix64 dampingRatio;

        protected override PJoint2D CreateJoint()
        {
            ParallelRigidbody2D self = GetComponent<ParallelRigidbody2D>();
            return Parallel2D.CreateSprintJoint(self, connectedRigidBody, anchor, connectedAnchor, collideConnected, frequency, dampingRatio);
        }
    }
}
