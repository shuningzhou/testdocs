using UnityEngine;

namespace Parallel
{
    [RequireComponent(typeof(ParallelRigidbody3D))]
    public abstract class ParallelJoint3D : MonoBehaviour
    {
        protected PJoint3D joint;
        protected ParallelRigidbody3D rbA;

        protected bool rbBIsNull;
        protected ParallelRigidbody3D rbB;

        public void InitializeJoint()
        {
            if (joint == null)
            {
                joint = CreateJoint();
            }
        }

        protected abstract PJoint3D CreateJoint();

        public void DestroyJoint()
        {
            if (joint != null)
            {
                if(rbA == null)
                {
                    return;
                }

                if(rbB == null && !rbBIsNull)
                {
                    return;
                }

                Parallel3D.DestroyJoint(joint);
                joint = null;
            }
        }
    }
}
