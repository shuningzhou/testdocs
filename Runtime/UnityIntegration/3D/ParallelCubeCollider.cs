using UnityEngine;

namespace Parallel
{
    [RequireComponent(typeof(ParallelTransform))]
    public class ParallelCubeCollider : ParallelCollider3D
    {
        [SerializeField]
        Fix64Vec3 _size = Fix64Vec3.one;

        public Fix64Vec3 size
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
            Fix64Vec3 s = CalculateSize();
            Gizmos.color = ParallelUtil.ColliderOutlineColor;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, (Vector3)s);
            Gizmos.matrix = Matrix4x4.identity;
        }

        Fix64Vec3 CalculateSize()
        {
            Fix64Vec3 s = size * pTransform.localScale;
            return s;
        }

        public override void UpdateShape(GameObject root)
        {
            Fix64Vec3 s = CalculateSize();
            if (s != Fix64Vec3.zero)
            {
                Fix64Vec3 center = Fix64Vec3.zero;
                Fix64Quat rotation = Fix64Quat.identity;

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
            Fix64Vec3 s = CalculateSize();

            if (s != Fix64Vec3.zero)
            {
                Fix64Vec3 center = Fix64Vec3.zero;
                Fix64Quat rotation = Fix64Quat.identity;

                if (gameObject != root)
                {
                    center = _pTransform.localPosition;
                    rotation = _pTransform.localRotation;
                }

                _shape = Parallel3D.CreateCube(Fix64Math.Abs(s.x), Fix64Math.Abs(s.y), Fix64Math.Abs(s.z), center, rotation);

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