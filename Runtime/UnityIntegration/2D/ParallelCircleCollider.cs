using UnityEngine;

namespace Parallel
{
    [RequireComponent(typeof(ParallelTransform))]
    [ExecuteInEditMode]
    public class ParallelCircleCollider : ParallelCollider2D
    {
        [SerializeField]
        Fix64 _radius = Fix64.FromDivision(1,2);

        public Fix64 radius
        {
            get
            {
                return _radius;
            }
        }

        public void UpdateShape(Fix64 radius)
        {
            _radius = radius;
            UpdateShape(_root);
        }

        void OnDrawGizmosSelected()
        {
            Fix64 r = CalculateRadius();
            if(r <= Fix64.zero)
            {
                Debug.LogError("Invalid Size");
                return;
            }
            Gizmos.color = ParallelUtil.ColliderOutlineColor;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireSphere(Vector3.zero, (float)r);
            Gizmos.matrix = Matrix4x4.identity;
        }

        Fix64 CalculateRadius()
        {
            Fix64 maxScale = Fix64Math.Max(colliderScale.x, colliderScale.y);
            Fix64 result = maxScale * _radius;
            return result;
        }

        protected override void UpdateShape(GameObject root)
        {
            Fix64 r = CalculateRadius();
            if (r <= Fix64.zero)
            {
                Debug.LogError("Invalid Size");
            }
            else
            {
                Fix64Vec2 center = Fix64Vec2.zero;

                if (gameObject != root)
                {
                    center = (Fix64Vec2)_pTransform.localPosition;
                }

                Parallel2D.UpdateCircle(_shape, _fixture, r, center);
            }
        }

        public override PShape2D CreateShape(GameObject root)
        {
            Fix64 r = CalculateRadius();
            if (r <= Fix64.zero)
            {
                Debug.LogError("Invalid Size");
                return null;
            }
            else
            {
                Fix64Vec2 center = Fix64Vec2.zero;

                if (gameObject != root)
                {
                    center = (Fix64Vec2)_pTransform.localPosition;
                }

                return Parallel2D.CreateCircle(r, center);
            }
        }
    }
}