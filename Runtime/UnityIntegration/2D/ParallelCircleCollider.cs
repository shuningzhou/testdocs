using UnityEngine;

namespace Parallel
{
    [RequireComponent(typeof(ParallelTransform))]
    [ExecuteInEditMode]
    public class ParallelCircleCollider : ParallelCollider2D
    {
        [SerializeField]
        FFloat _radius = FFloat.FromDivision(1,2);

        public FFloat radius
        {
            get
            {
                return _radius;
            }
        }

        public void UpdateShape(FFloat radius)
        {
            _radius = radius;
            UpdateShape(_root);
        }

        void OnDrawGizmosSelected()
        {
            FFloat r = CalculateRadius();
            if(r <= FFloat.zero)
            {
                Debug.LogError("Invalid Size");
                return;
            }
            Gizmos.color = ParallelUtil.ColliderOutlineColor;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireSphere(Vector3.zero, (float)r);
            Gizmos.matrix = Matrix4x4.identity;
        }

        FFloat CalculateRadius()
        {

            FFloat maxScale = FMath.Max(FMath.Abs(colliderScale.x), FMath.Abs(colliderScale.y));
            FFloat result = maxScale * _radius;
            return result;
        }

        protected override void UpdateShape(GameObject root)
        {
            FFloat r = CalculateRadius();
            if (r <= FFloat.zero)
            {
                Debug.LogError("Invalid Size");
            }
            else
            {
                FVector2 center = FVector2.zero;

                if (gameObject != root)
                {
                    center = (FVector2)_pTransform.localPosition;
                }

                Parallel2D.UpdateCircle(_shape, _fixture, r, center);
            }
        }

        public override PShape2D CreateShape(GameObject root)
        {
            FFloat r = CalculateRadius();
            if (r <= FFloat.zero)
            {
                Debug.LogError("Invalid Size");
                return null;
            }
            else
            {
                FVector2 center = FVector2.zero;

                if (gameObject != root)
                {
                    center = (FVector2)_pTransform.localPosition;
                }

                return Parallel2D.CreateCircle(r, center);
            }
        }
    }
}