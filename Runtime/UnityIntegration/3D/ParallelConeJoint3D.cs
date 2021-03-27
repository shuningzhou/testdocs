using UnityEngine;

namespace Parallel
{
    public class ParallelConeJoint3D : ParallelJoint3D
    {
        public bool collideConnected = true;
        public ParallelRigidbody3D connectedRigidBody;
        public FVector3 worldAnchor;
        public FVector3 worldAxis = FVector3.axisZ;

        public bool coneLimit;
        public FFloat coneAngle;

        public bool twistLimit;
        public FFloat lowerAngle;
        public FFloat upperAngle;

        void Reset()
        {
            worldAnchor = (FVector3)transform.position;
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying && connectedRigidBody != null)
            {
                Vector3 size = new Vector3(0.1f, 0.1f, 0.1f);
                Gizmos.DrawCube(connectedRigidBody.transform.position, size);
                Gizmos.DrawCube(transform.position, size);
                Gizmos.DrawCube((Vector3)worldAnchor, size);

                Gizmos.DrawLine(connectedRigidBody.transform.position, (Vector3)worldAnchor);
                Gizmos.DrawLine(transform.position, (Vector3)worldAnchor);
            }
        }

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
            return Parallel3D.CreateConeJoint(self, connectedRigidBody, worldAnchor, worldAxis, collideConnected, coneLimit, FFloat.DegToRad(coneAngle), twistLimit, FFloat.DegToRad(lowerAngle), FFloat.DegToRad(upperAngle));
        }
    }
}
