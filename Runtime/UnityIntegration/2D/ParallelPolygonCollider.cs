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

        public FVector2[] convexVerts;
        public int convexVertsCount;

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
            FVector2[] fixedVerts = new FVector2[vertsCount];
            for (int i = 0; i < vertsCount; i++)
            {
                fixedVerts[i] = (FVector2)verts[i];
            }

            ParallelVec2List vec2List = Parallel2D.ConvexHull2D(fixedVerts, vertsCount, limit);

            convexVerts = new FVector2[vec2List.count];
            for (int i = 0; i < vec2List.count; i++)
            {
                convexVerts[i] = vec2List.points[i];
            }

            convexVertsCount = vec2List.count;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = ParallelUtil.ColliderOutlineColor;
            foreach (FVector2 v in convexVerts)
            {
                Vector2 vert = (Vector2)v;
                Gizmos.DrawWireSphere(transform.TransformPoint(vert), 0.1f);
            }
        }

        FVector2 CalculateSize()
        {
            FVector2 s = _size * (FVector2)colliderScale;
            return s;
        }

        protected override void UpdateShape(GameObject root)
        {
            FVector2 s = CalculateSize();

            if (s != FVector2.zero)
            {
                FVector2[] scaled = new FVector2[convexVertsCount];

                for (int i = 0; i < convexVertsCount; i++)
                {
                    scaled[i] = convexVerts[i] * s;
                }

                FFloat angle = FFloat.zero;
                FVector2 center = FVector2.zero;

                if (gameObject != root)
                {
                    angle = FFloat.DegToRad(_pTransform.localEulerAngles.z);
                    center = (FVector2)_pTransform.localPosition;
                }

                Parallel2D.UpdatePolygon(_shape, _fixture, scaled, convexVertsCount, center, angle);
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

                if (s == FVector2.one)
                {
                    return Parallel2D.CreatePolygon(convexVerts, convexVertsCount, center, angle);
                }
                else
                {
                    FVector2[] scaled = new FVector2[convexVertsCount];

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