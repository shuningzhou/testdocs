using UnityEngine;

namespace Parallel
{
    [RequireComponent(typeof(ParallelTransform))]
    [ExecuteInEditMode]
    public class ParallelPolygonCollider : ParallelCollider2D
    { 
        Sprite sprite;
        public Vector2[] verts;
        public int vertsCount;

        public Fix64Vec2[] convexVerts;
        public int convexVertsCount;

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

        public int limit = 8;

        void Reset()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                sprite = spriteRenderer.sprite;
                if (sprite != null)
                {
                    verts = sprite.vertices;
                    vertsCount = verts.Length;
                    ConvexHull2D();
                }
            }
        }

        void ConvexHull2D()
        {
            Fix64Vec2[] fixedVerts = new Fix64Vec2[vertsCount];
            for (int i = 0; i < vertsCount; i++)
            {
                fixedVerts[i] = (Fix64Vec2)verts[i];
            }

            ParallelVec2List vec2List = Parallel2D.ConvexHull2D(fixedVerts, vertsCount, limit);

            convexVerts = new Fix64Vec2[vec2List.count];
            for (int i = 0; i < vec2List.count; i++)
            {
                convexVerts[i] = vec2List.points[i];
            }

            convexVertsCount = vec2List.count;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = ParallelUtil.ColliderOutlineColor;
            foreach (Fix64Vec2 v in convexVerts)
            {
                Vector2 vert = (Vector2)v;
                Gizmos.DrawWireSphere(transform.TransformPoint(vert), 0.1f);
            }
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

                Fix64Vec2[] scaled = new Fix64Vec2[convexVertsCount];

                for (int i = 0; i < convexVertsCount; i++)
                {
                    scaled[i] = convexVerts[i] * s;
                }

                Fix64 angle = Fix64.zero;
                Fix64Vec2 center = Fix64Vec2.zero;

                if (gameObject != root)
                {
                    angle = Fix64.DegToRad(_pTransform.localEulerAngles.z);
                    center = (Fix64Vec2)_pTransform.localPosition;
                }

                Parallel2D.UpdatePolygon(_shape, _fixture, scaled, convexVertsCount, center, angle);
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

                if (s == Fix64Vec2.one)
                {
                    return Parallel2D.CreatePolygon(convexVerts, convexVertsCount, center, angle);
                }
                else
                {
                    Fix64Vec2[] scaled = new Fix64Vec2[convexVertsCount];

                    for (int i = 0; i < convexVertsCount; i++)
                    {
                        scaled[i] = convexVerts[i] * s;
                    }

                    return Parallel2D.CreatePolygon(scaled, convexVertsCount, center, angle);
                }
            }
            else
            {
                return null;
            }
        }
    }
}