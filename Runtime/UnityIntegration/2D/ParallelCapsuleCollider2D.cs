using UnityEngine;

namespace Parallel
{
    [RequireComponent(typeof(ParallelTransform))]
    [ExecuteInEditMode]
    public class ParallelCapsuleCollider2D : ParallelCollider2D
    {
        public enum PCapsuleDirection2D
        {
            Horizontal,
            Vertical
        }

        public PCapsuleDirection2D Direction = PCapsuleDirection2D.Vertical;

        [SerializeField]
        FFloat _radius = FFloat.FromDivision(25, 100);
        public FFloat radius
        {
            get
            {
                return _radius;
            }
        }

        [SerializeField]
        FFloat _height = FFloat.FromDivision(1, 1);
        public FFloat height
        {
            get
            {
                return _height;
            }
        }

        public void UpdateShape(FFloat radius, FFloat height)
        {
            _radius = radius;
            _height = height;
            UpdateShape(_root);
        }

        FVector2 v1;
        FVector2 v2;

        void OnDrawGizmosSelected()
        {
            FFloat radius = CalculateRadius();
            FFloat height = CalculateHeight();

            if (radius > FFloat.zero && height > FFloat.zero)
            {
                FVector2 p1 = FVector2.zero;
                FVector2 p2 = FVector2.zero;

                CalculatePoints(height, radius, ref p1, ref p2);

                if (p1 == FVector2.zero || p2 == FVector2.zero)
                {
                    return;
                }

                Gizmos.color = ParallelUtil.ColliderOutlineColor;
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                Gizmos.DrawWireSphere((Vector2)p1, (float)radius);
                Gizmos.DrawWireSphere((Vector2)p2, (float)radius);

                Gizmos.DrawLine((Vector2)p1 - Vector2.left * (float)radius, (Vector2)p2 - Vector2.left * (float)radius);
                Gizmos.DrawLine((Vector2)p1 - Vector2.right * (float)radius, (Vector2)p2 - Vector2.right * (float)radius);
                Gizmos.matrix = Matrix4x4.identity;
            }
        }

        void CalculatePoints(FFloat height, FFloat radius, ref FVector2 point1, ref FVector2 point2)
        {
            point1 = FVector2.zero;
            point2 = FVector2.zero;

            FFloat pointDistance = height - FFloat.FromDivision(2, 1) * radius;

            if (pointDistance <= FFloat.zero)
            {
                Debug.LogError("Invalid size");
                return;
            }

            if (Direction == PCapsuleDirection2D.Horizontal)
            {
                point1 = new FVector2(FFloat.one, FFloat.zero);
                point2 = new FVector2(-FFloat.one, FFloat.zero);
            }
            else
            {
                point1 = new FVector2(FFloat.zero, FFloat.one);
                point2 = new FVector2(FFloat.zero, -FFloat.one);
            }

            point1 = point1 * (pointDistance / FFloat.FromDivision(2, 1));
            point2 = point2 * (pointDistance / FFloat.FromDivision(2, 1));
        }

        FFloat CalculateRadius()
        {
            FFloat maxScale = FFloat.one;

            if (Direction == PCapsuleDirection2D.Horizontal)
            {
                if (_xzPlane)
                {
                    maxScale = colliderScale.z;
                }
                else
                {
                    maxScale = colliderScale.y;
                }
            }
            else
            {
                maxScale = colliderScale.x;
            }

            FFloat result = maxScale * _radius;
            return FMath.Abs(result);
        }

        FFloat CalculateHeight()
        {
            FFloat maxScale = FFloat.one;

            if (Direction == PCapsuleDirection2D.Horizontal)
            {
                maxScale = colliderScale.x;
            }
            else
            {
                if (_xzPlane)
                {
                    maxScale = colliderScale.z;
                }
                else
                {
                    maxScale = colliderScale.y;
                }
            }

            FFloat result = maxScale * _height;

            return FMath.Abs(result);
        }

        protected override void UpdateShape(GameObject root)
        {
            FFloat radius = CalculateRadius();
            FFloat height = CalculateHeight();

            CalculatePoints(height, radius, ref v1, ref v2);

            if (v1 != FVector2.zero && v2 != FVector2.zero)
            {
                FFloat angle = FFloat.zero;
                FVector2 center = FVector2.zero;

                if (gameObject != root)
                {
                    if (_xzPlane)
                    {
                        angle = -FFloat.DegToRad(_pTransform.localEulerAngles.y);
                        center = _pTransform.localPosition.xz;
                    }
                    else
                    {
                        angle = FFloat.DegToRad(_pTransform.localEulerAngles.z);
                        center = (FVector2)_pTransform.localPosition;
                    }
                }

                Parallel2D.UpdateCapsule(_shape, _fixture, v1, v2, radius, center, angle);
            }
        }

        public override PShape2D CreateShape(GameObject root)
        {
            FFloat radius = CalculateRadius();
            FFloat height = CalculateHeight();

            CalculatePoints(height, radius, ref v1, ref v2);

            if (v1 == FVector2.zero || v2 == FVector2.zero)
            {
                return null;
            }
            else
            {
                FFloat angle = FFloat.zero;
                FVector2 center = FVector2.zero;

                if (gameObject != root)
                {
                    if (_xzPlane)
                    {
                        angle = -FFloat.DegToRad(_pTransform.localEulerAngles.y);
                        center = _pTransform.localPosition.xz;
                    }
                    else
                    {
                        angle = FFloat.DegToRad(_pTransform.localEulerAngles.z);
                        center = (FVector2)_pTransform.localPosition;
                    }
                }

                return Parallel2D.CreateCapsule(v1, v2, radius, center, angle);
            }
        }
    }
}