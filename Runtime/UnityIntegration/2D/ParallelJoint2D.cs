using UnityEngine;

namespace Parallel
{
    [RequireComponent(typeof(ParallelRigidbody2D))]
    public abstract class ParallelJoint2D : MonoBehaviour
    {
        protected PJoint2D joint;
        protected ParallelRigidbody2D rbA;

        protected bool rbBIsNull;
        protected ParallelRigidbody2D rbB;

        public void InitializeJoint()
        {
            if(joint == null)
            {
                joint = CreateJoint();
            }
        }

        protected abstract PJoint2D CreateJoint();

        public void DestroyJoint()
        {
            if (joint != null)
            {
                if (rbA == null)
                {
                    return;
                }

                if (rbB == null && !rbBIsNull)
                {
                    return;
                }

                Parallel2D.DestroyJoint(joint);
                joint = null;
            }
        }
    }
}
