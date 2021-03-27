using UnityEngine;

namespace Parallel
{
    [RequireComponent(typeof(ParallelTransform))]
    public class ParallelCubeCollider : ParallelCollider3D
    {
        [SerializeField]
        FVector3 _size = FVector3.one;

        public FVector3 size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }

        void OnDrawGizmosSelected()
        {
            FVector3 s = CalculateSize();
            Gizmos.color = ParallelUtil.ColliderOutlineColor;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, (Vector3)s);
            Gizmos.matrix = Matrix4x4.identity;
        }

        FVector3 CalculateSize()
        {
            FVector3 s = size * pTransform.localScale;
            return s;
        }

        public override void UpdateShape(GameObject root)
        {
            FVector3 s = CalculateSize();
            if (s != FVector3.zero)
            {
                FVector3 center = FVector3.zero;
                FQuaternion rotation = FQuaternion.identity;

                if(gameObject != root)
                {
                    center = _pTransform.localPosition;
                    rotation = _pTransform.localRotation;
                }

                Parallel3D.UpdateCube(_shape, _fixture, s.x, s.y, s.z, center, rotation);
            }
        }

        public override PShape3D CreateShape(GameObject root)
        {
            FVector3 s = CalculateSize();

            if (s != FVector3.zero)
            {
                FVector3 center = FVector3.zero;
                FQuaternion rotation = FQuaternion.identity;

                if (gameObject != root)
                {
                    center = _pTransform.localPosition;
                    rotation = _pTransform.localRotation;
                }

                _shape = Parallel3D.CreateCube(FMath.Abs(s.x), FMath.Abs(s.y), FMath.Abs(s.z), center, rotation);

                if(createUnityPhysicsCollider)
                {
                    var collider = gameObject.AddComponent<BoxCollider>();
                    collider.size = (Vector3)size;
                }

                return _shape;
            }
            else
            {
                return null;
            }
        }
    }
}