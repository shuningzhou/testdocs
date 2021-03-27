using UnityEngine;

namespace Parallel
{
    [RequireComponent(typeof(ParallelTransform))]
    [ExecuteInEditMode]
    public class ParallelBoxCollider2D : ParallelCollider2D
    {
        [SerializeField]
        FVector2 _size = FVector2.one;

        public FVector2 size
        {
            get
            {
                return _size;
            }
        }

        public void UpdateShape(FVector2 size)
        {
            _size = size;
            UpdateShape(_root);
        }

        void OnDrawGizmosSelected()
        {
            FVector2 s = CalculateSize();
            Gizmos.color = ParallelUtil.ColliderOutlineColor;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, (Vector2)s);
            Gizmos.matrix = Matrix4x4.identity;
        }

        FVector2 CalculateSize()
        {

            FVector2 s = _size * (FVector2)colliderScale;
            return s;

            if (s.x > FFloat.zero && s.y > FFloat.zero)
            {
                return s;
            }
            else
            {
                Debug.LogError("Invalid size");
                return FVector2.zero;
            }
        }

        protected override void UpdateShape(GameObject root)
        {
            FVector2 s = CalculateSize();

            if (s != FVector2.zero)
            {
                FFloat angle = FFloat.zero;
                FVector2 center = FVector2.zero;

                if (gameObject != root)
                {
                    angle = FFloat.DegToRad(_pTransform.localEulerAngles.z);
                    center = (FVector2)_pTransform.localPosition;
                }

                Parallel2D.UpdateBox(_shape, _fixture, s.x, s.y, center, angle);
            }
        }

        public override PShape2D CreateShape(GameObject root)
        {
            FVector2 s = CalculateSize();

            if (s != FVector2.zero)
            {
                FFloat angle = FFloat.zero;
                FVector2 center = FVector2.zero;

                if (gameObject != root)
                {
                    angle = FFloat.DegToRad(_pTransform.localEulerAngles.z);
                    center = (FVector2)_pTransform.localPosition;
                }
                
                return Parallel2D.CreateBox(FMath.Abs(s.x), FMath.Abs(s.y), center, angle);
            }
            else
            {
                return null;
            }
        }
    }
}