using UnityEngine;

namespace Parallel
{
    [RequireComponent(typeof(ParallelTransform))]
    [ExecuteInEditMode]
    public class ParallelCapsuleCollider3D : ParallelCollider3D
    {
        public enum ParallelCapsuleDirection3D
        {
            XAxis = 0,
            YAxis = 1,
            ZAxis = 2
        }

        public ParallelCapsuleDirection3D Direction = ParallelCapsuleDirection3D.YAxis;

        [SerializeField]
        FFloat _radius = FFloat.FromDivision(1, 2);
        [SerializeField]
        FFloat _height = FFloat.FromDivision(2, 1);

        public FFloat height { get; private set; }
        public FFloat radius { get; private set; }

        public FVector3 point1;
        public FVector3 point2;

        void OnDrawGizmosSelected()
        {
            FFloat h = CalculateHeight();
            FFloat r = CalculateRadius();
            FVector3 p1 = FVector3.zero;
            FVector3 p2 = FVector3.zero;
            CalculatePoints(h, r, ref p1, ref p2);

            if (p1 == FVector3.zero || p2 == FVector3.zero)
            {
                Debug.LogError("Invalid Size");
                return;
            }

            Gizmos.color = ParallelUtil.ColliderOutlineColor;
            Vector3 point1 = ParallelUtil.TransformPointUnscaled(transform, (Vector3)p1);
            Vector3 point2 = ParallelUtil.TransformPointUnscaled(transform, (Vector3)p2);

            Vector3 origin = (point1 - point2) / 2 + point1;
            Gizmos.matrix = Matrix4x4.TRS(origin, Quaternion.identity, new Vector3((float)r, (float)r, (float)r));
            ParallelUtil.DrawHemispheresOfCapsule(point1, point2, (float)r);
            Gizmos.matrix = Matrix4x4.identity;
            ParallelUtil.DrawLineConnectingHS(point1, point2, (float)r);
            Gizmos.matrix = Matrix4x4.identity;
        }

        void CalculatePoints(FFloat h, FFloat r, ref FVector3 point1, ref FVector3 point2)
        {
            point1 = FVector3.zero;
            point2 = FVector3.zero;

            FFloat pointDistance = h - FFloat.FromDivision(2 , 1) * r;

            if (pointDistance <= FFloat.zero)
            {
                Debug.LogError("Invalid size");
                return;
            }

            if (Direction == ParallelCapsuleDirection3D.XAxis)
            {
                point1 = new FVector3(FFloat.one, FFloat.zero, FFloat.zero);
                point2 = new FVector3(-FFloat.one, FFloat.zero, FFloat.zero);
            }
            else if (Direction == ParallelCapsuleDirection3D.YAxis)
            {
                point1 = new FVector3(FFloat.zero, FFloat.one, FFloat.zero);
                point2 = new FVector3(FFloat.zero, -FFloat.one, FFloat.zero);
            }
            else
            {
                point1 = new FVector3(FFloat.zero, FFloat.zero, FFloat.one);
                point2 = new FVector3(FFloat.zero, FFloat.zero, -FFloat.one);
            }

            point1 = point1 * (pointDistance / FFloat.FromDivision(2, 1));
            point2 = point2 * (pointDistance / FFloat.FromDivision(2, 1));
        }

        FFloat CalculateRadius()
        {
            FFloat maxScale = FFloat.one;

            if (Direction == ParallelCapsuleDirection3D.XAxis)
            {
                maxScale = FMath.Max(FMath.Abs(pTransform.localScale.y), FMath.Abs(pTransform.localScale.z));
            }
            else if (Direction == ParallelCapsuleDirection3D.YAxis)
            {
                maxScale = FMath.Max(FMath.Abs(pTransform.localScale.x), FMath.Abs(pTransform.localScale.z));
            }
            else
            {
                maxScale = FMath.Max(FMath.Abs(pTransform.localScale.x), FMath.Abs(pTransform.localScale.y));
            }

            return maxScale * _radius;
        }

        FFloat CalculateHeight()
        {
            FFloat maxScale = FFloat.one;

            if (Direction == ParallelCapsuleDirection3D.XAxis)
            {
                maxScale = pTransform.localScale.x;
            }
            else if (Direction == ParallelCapsuleDirection3D.YAxis)
            {
                maxScale = pTransform.localScale.y;
            }
            else
            {
                maxScale = pTransform.localScale.z;
            }

            return FMath.Abs(maxScale * _height);
        }

        public override void UpdateShape(GameObject root)
        {
            FFloat h = CalculateHeight();
            FFloat r = CalculateRadius();
            FVector3 p1 = FVector3.zero;
            FVector3 p2 = FVector3.zero;
            CalculatePoints(h, r, ref p1, ref p2);

            if (p1 == FVector3.zero || p2 == FVector3.zero)
            {
                Debug.LogError("Invalid Size");
                return;
            }

            FVector3 center = FVector3.zero;
            FQuaternion rotation = FQuaternion.identity;

            if (gameObject != root)
            {
                center = _pTransform.localPosition;
                rotation = _pTransform.localRotation;
            }

            point1 = p1;
            point2 = p2;
            radius = r;
            height = h;
            Parallel3D.UpdateCapsule(_shape, _fixture, p1, p2, radius, center, rotation);
        }

        public override PShape3D CreateShape(GameObject root)
        {
            FFloat h = CalculateHeight();
            FFloat r = CalculateRadius();
            FVector3 p1 = FVector3.zero;
            FVector3 p2 = FVector3.zero;
            CalculatePoints(h, r, ref p1, ref p2);

            if (p1 == FVector3.zero || p2 == FVector3.zero)
            {
                Debug.LogError("Invalid Size");
                return null;
            }
            else
            {
                FVector3 center = FVector3.zero;
                FQuaternion rotation = FQuaternion.identity;

                if (gameObject != root)
                {
                    center = _pTransform.localPosition;
                    rotation = _pTransform.localRotation;
                }

                point1 = p1;
                point2 = p2;
                radius = r;
                height = h;
                _shape = Parallel3D.CreateCapsule(p1, p2, radius, center, rotation);

                if(createUnityPhysicsCollider)
                {
                    var collider = gameObject.AddComponent<CapsuleCollider>();
                    collider.height = (float)h;
                    collider.radius = (float)radius;
                }

                return _shape;
            }
        }
    }
}

//namespace Parallel
//{
//    [RequireComponent(typeof(ParallelTransform))]
//    [ExecuteInEditMode]
//    public class PCapsuleCollider3D : MonoBehaviour, IParallelCollider3D
//    {
//        public enum PCapsuleDirection3D
//        {
//            XAxis = 0,
//            YAxis = 1,
//            ZAxis = 2
//        }

//        PShape3D _shape;
//        PFixture3D _fixture;

//        public FFloat Radius = FFloat.FromDivision(1, 2);
//        public FFloat Height = FFloat.FromDivision(2, 1);

//        public PCapsuleDirection3D Direction = PCapsuleDirection3D.YAxis;

//        public FVector3 Point1;
//        public FVector3 Point2;

//        ParallelTransform _pTransform;

//        public bool Deterministic = true;

//        public bool ShapeDirty { get; set; }

//        public ParallelTransform pTransform
//        {
//            get
//            {
//                if (_pTransform == null)
//                {
//                    _pTransform = GetComponent<ParallelTransform>();
//                }

//                return _pTransform;
//            }
//        }

//        void Update()
//        {
//            //only import from unity in editing mode
//            if (!Deterministic || !Application.isPlaying)
//            {
//                //size = (FVector3)editorSize;
//            }
//        }

//        void OnDrawGizmosSelected()
//        {
//            FFloat height = CalculateHeight();
//            FFloat radius = CalculateRadius();
//            FVector3 p1 = FVector3.zero;
//            FVector3 p2 = FVector3.zero;
//            CalculatePoints(height, radius, ref p1, ref p2);

//            if (p1 == FVector3.zero || p2 == FVector3.zero)
//            {
//                Debug.LogError("Invalid Size");
//                return;
//            }

//            Gizmos.color = DebugSettings.ColliderOutlineColor;
//            Vector3 point1 = PMath.TransformPointUnscaled(transform, (Vector3)p1);
//            Vector3 point2 = PMath.TransformPointUnscaled(transform, (Vector3)p2);

//            Vector3 origin = (point1 - point2) / 2 + point1;
//            Gizmos.matrix = Matrix4x4.TRS(origin, Quaternion.identity, new Vector3((float)radius, (float)radius, (float)radius));
//            DebugDraw.DrawHemispheresOfCapsule(point1, point2, (float)radius);
//            Gizmos.matrix = Matrix4x4.identity;
//            DebugDraw.DrawLineConnectingHS(point1, point2, (float)radius);
//            Gizmos.matrix = Matrix4x4.identity;
//        }

//        void CalculatePoints(FFloat height, FFloat radius, ref FVector3 point1, ref FVector3 point2)
//        {
//            point1 = FVector3.zero;
//            point2 = FVector3.zero;

//            FFloat pointDistance = height - FFloat.FromDivision(2, 1) * radius;

//            if (pointDistance <= FFloat.zero)
//            {
//                Debug.LogError("Invalid size");
//                return;
//            }

//            if (Direction == PCapsuleDirection3D.XAxis)
//            {
//                point1 = new FVector3(FFloat.one, FFloat.zero, FFloat.zero);
//                point2 = new FVector3(-FFloat.one, FFloat.zero, FFloat.zero);
//            }
//            else if (Direction == PCapsuleDirection3D.YAxis)
//            {
//                point1 = new FVector3(FFloat.zero, FFloat.one, FFloat.zero);
//                point2 = new FVector3(FFloat.zero, -FFloat.one, FFloat.zero);
//            }
//            else
//            {
//                point1 = new FVector3(FFloat.zero, FFloat.zero, FFloat.one);
//                point2 = new FVector3(FFloat.zero, FFloat.zero, -FFloat.one);
//            }

//            point1 = point1 * (pointDistance / FFloat.FromDivision(2, 1));
//            point2 = point2 * (pointDistance / FFloat.FromDivision(2, 1));
//        }

//        FFloat CalculateRadius()
//        {
//            FFloat maxScale = FFloat.one;

//            if (Direction == PCapsuleDirection3D.XAxis)
//            {
//                maxScale = FMath.Max(pTransform.localScale.y, pTransform.localScale.z);
//            }
//            else if (Direction == PCapsuleDirection3D.YAxis)
//            {
//                maxScale = FMath.Max(pTransform.localScale.x, pTransform.localScale.z);
//            }
//            else
//            {
//                maxScale = FMath.Max(pTransform.localScale.x, pTransform.localScale.y);
//            }

//            return maxScale * Radius;
//        }

//        FFloat CalculateHeight()
//        {
//            FFloat maxScale = FFloat.one;

//            if (Direction == PCapsuleDirection3D.XAxis)
//            {
//                maxScale = pTransform.localScale.x;
//            }
//            else if (Direction == PCapsuleDirection3D.YAxis)
//            {
//                maxScale = pTransform.localScale.y;
//            }
//            else
//            {
//                maxScale = pTransform.localScale.z;
//            }

//            return maxScale * Height;
//        }

//        public PShape3D CreateShape()
//        {
//            FFloat height = CalculateHeight();
//            FFloat radius = CalculateRadius();
//            FVector3 p1 = FVector3.zero;
//            FVector3 p2 = FVector3.zero;
//            CalculatePoints(height, radius, ref p1, ref p2);

//            if (p1 == FVector3.zero || p2 == FVector3.zero)
//            {
//                Debug.LogError("Invalid Size");
//                return null;
//            }

//            _shape = Parallel3D.CreateCapsule(p1, p2, radius);
//            return _shape;
//        }

//        public void ReceiveFixture(PFixture3D fixture)
//        {
//            _fixture = fixture;
//        }

//        public void UpdateNativeShapeIfNecessary()
//        {
//            if (!ShapeDirty)
//            {
//                return;
//            }

//            FFloat height = CalculateHeight();
//            FFloat radius = CalculateRadius();
//            FVector3 p1 = FVector3.zero;
//            FVector3 p2 = FVector3.zero;
//            CalculatePoints(height, radius, ref p1, ref p2);

//            if (p1 == FVector3.zero || p2 == FVector3.zero)
//            {
//                Debug.LogError("Invalid Size");
//                return;
//            }

//            Parallel3D.UpdateCapsule(_shape, _fixture, p1, p2, radius);
//            ShapeDirty = false;
//        }
//    }
//}
