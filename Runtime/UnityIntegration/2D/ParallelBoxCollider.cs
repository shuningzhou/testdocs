using UnityEngine;

namespace Parallel
{
    [RequireComponent(typeof(ParallelTransform))]
    [ExecuteInEditMode]
    public class ParallelBoxCollider : ParallelCollider2D
    {
        [SerializeField]
        Fix64Vec2 _size = Fix64Vec2.one;

        public Fix64Vec2 size
        {
            get
            {
                return _size;
            }
        }

        public void UpdateShape(Fix64Vec2 size)
        {
            _size = size;
            UpdateShape(_root);
        }

        void OnDrawGizmosSelected()
        {
            Fix64Vec2 s = CalculateSize();
            Gizmos.color = ParallelUtil.ColliderOutlineColor;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, (Vector2)s);
            Gizmos.matrix = Matrix4x4.identity;
        }

        Fix64Vec2 CalculateSize()
        {

            Fix64Vec2 s = _size * (Fix64Vec2)colliderScale;

            if (s.x > Fix64.zero && s.y > Fix64.zero)
            {
                return s;
            }
            else
            {
                Debug.LogError("Invalid size");
                return Fix64Vec2.zero;
            }
        }

        protected override void UpdateShape(GameObject root)
        {
            Fix64Vec2 s = CalculateSize();

            if (s != Fix64Vec2.zero)
            {
                Fix64 angle = Fix64.zero;
                Fix64Vec2 center = Fix64Vec2.zero;

                if (gameObject != root)
                {
                    angle = Fix64.DegToRad(_pTransform.localEulerAngles.z);
                    center = (Fix64Vec2)_pTransform.localPosition;
                }

                Parallel2D.UpdateBox(_shape, _fixture, s.x, s.y, center, angle);
            }
        }

        public override PShape2D CreateShape(GameObject root)
        {
            Fix64Vec2 s = CalculateSize();

            if (s != Fix64Vec2.zero)
            {
                Fix64 angle = Fix64.zero;
                Fix64Vec2 center = Fix64Vec2.zero;

                if (gameObject != root)
                {
                    angle = Fix64.DegToRad(_pTransform.localEulerAngles.z);
                    center = (Fix64Vec2)_pTransform.localPosition;
                }

                return Parallel2D.CreateBox(s.x, s.y, center, angle);
            }
            else
            {
                return null;
            }
        }
    }
}