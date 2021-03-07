﻿using UnityEngine;

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
        Fix64 _radius = Fix64.FromDivision(25, 100);
        public Fix64 radius
        {
            get
            {
                return _radius;
            }
        }

        [SerializeField]
        Fix64 _height = Fix64.FromDivision(1, 1);
        public Fix64 height
        {
            get
            {
                return _height;
            }
        }

        public void UpdateShape(Fix64 radius, Fix64 height)
        {
            _radius = radius;
            _height = height;
            UpdateShape(_root);
        }

        Fix64Vec2 v1;
        Fix64Vec2 v2;

        void OnDrawGizmosSelected()
        {
            Fix64 radius = CalculateRadius();
            Fix64 height = CalculateHeight();

            if (radius > Fix64.zero && height > Fix64.zero)
            {
                Fix64Vec2 p1 = Fix64Vec2.zero;
                Fix64Vec2 p2 = Fix64Vec2.zero;

                CalculatePoints(height, radius, ref p1, ref p2);

                if (p1 == Fix64Vec2.zero || p2 == Fix64Vec2.zero)
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

        void CalculatePoints(Fix64 height, Fix64 radius, ref Fix64Vec2 point1, ref Fix64Vec2 point2)
        {
            point1 = Fix64Vec2.zero;
            point2 = Fix64Vec2.zero;

            Fix64 pointDistance = height - Fix64.FromDivision(2, 1) * radius;

            if (pointDistance <= Fix64.zero)
            {
                Debug.LogError("Invalid size");
                return;
            }

            if (Direction == PCapsuleDirection2D.Horizontal)
            {
                point1 = new Fix64Vec2(Fix64.one, Fix64.zero);
                point2 = new Fix64Vec2(-Fix64.one, Fix64.zero);
            }
            else
            {
                point1 = new Fix64Vec2(Fix64.zero, Fix64.one);
                point2 = new Fix64Vec2(Fix64.zero, -Fix64.one);
            }

            point1 = point1 * (pointDistance / Fix64.FromDivision(2, 1));
            point2 = point2 * (pointDistance / Fix64.FromDivision(2, 1));
        }

        Fix64 CalculateRadius()
        {
            Fix64 maxScale = Fix64.one;

            if (Direction == PCapsuleDirection2D.Horizontal)
            {
                maxScale = colliderScale.y;
            }
            else
            {
                maxScale = colliderScale.x;
            }


            Fix64 result = maxScale * _radius;
            return Fix64Math.Abs(result);
        }

        Fix64 CalculateHeight()
        {
            Fix64 maxScale = Fix64.one;

            if (Direction == PCapsuleDirection2D.Horizontal)
            {
                maxScale = colliderScale.x;
            }
            else
            {
                maxScale = colliderScale.y;
            }

            Fix64 result = maxScale * _height;

            return Fix64Math.Abs(result);
        }

        protected override void UpdateShape(GameObject root)
        {
            Fix64 radius = CalculateRadius();
            Fix64 height = CalculateHeight();

            CalculatePoints(height, radius, ref v1, ref v2);

            if (v1 != Fix64Vec2.zero && v2 != Fix64Vec2.zero)
            {
                Fix64 angle = Fix64.zero;
                Fix64Vec2 center = Fix64Vec2.zero;

                if (gameObject != root)
                {
                    angle = Fix64.DegToRad(_pTransform.localEulerAngles.z);
                    center = (Fix64Vec2)_pTransform.localPosition;
                }

                Parallel2D.UpdateCapsule(_shape, _fixture, v1, v2, radius, center, angle);
            }
        }

        public override PShape2D CreateShape(GameObject root)
        {
            Fix64 radius = CalculateRadius();
            Fix64 height = CalculateHeight();

            CalculatePoints(height, radius, ref v1, ref v2);

            if (v1 == Fix64Vec2.zero || v2 == Fix64Vec2.zero)
            {
                return null;
            }
            else
            {
                Fix64 angle = Fix64.zero;
                Fix64Vec2 center = Fix64Vec2.zero;

                if (gameObject != root)
                {
                    angle = Fix64.DegToRad(_pTransform.localEulerAngles.z);
                    center = (Fix64Vec2)_pTransform.localPosition;
                }

                return Parallel2D.CreateCapsule(v1, v2, radius, center, angle);
            }
        }
    }
}