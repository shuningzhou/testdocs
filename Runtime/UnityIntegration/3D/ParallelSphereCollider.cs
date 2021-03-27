using UnityEngine;

namespace Parallel
{
    [RequireComponent(typeof(ParallelTransform))]
    [ExecuteInEditMode]
    public class ParallelSphereCollider : ParallelCollider3D
    {
        [SerializeField]
        FFloat _radius = FFloat.FromDivision(1, 2);

        public FFloat radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
            }
        }

        void OnDrawGizmosSelected()
        {
            FFloat r = CalculateRadius();

            if (r > FFloat.zero)
            {
                Gizmos.color = ParallelUtil.ColliderOutlineColor;
                Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, 1, 1));
                ParallelUtil.DrawHemiSphere(Vector3.zero, (float)r, Vector3.up);
                ParallelUtil.DrawHemiSphere(Vector3.zero, (float)r, Vector3.down);
                Gizmos.matrix = Matrix4x4.identity;
            }
            else
            {
                Debug.LogError("Invalid Size");
            }
        }

        FFloat CalculateRadius()
        {
            FFloat maxScale = FMath.Max(FMath.Abs(pTransform.localScale.x), FMath.Abs(pTransform.localScale.y), FMath.Abs(pTransform.localScale.z));
            FFloat result = maxScale * _radius;
            return result;
        }

        public override void UpdateShape(GameObject root)
        {
            FFloat r = CalculateRadius();

            if (r > FFloat.zero)
            {
                FVector3 center = FVector3.zero;

                if (gameObject != root)
                {
                    center = _pTransform.localPosition;
                }

                Parallel3D.UpdateSphere(_shape, _fixture, r, center);
            }
        }

        public override PShape3D CreateShape(GameObject root)
        {
            FFloat r = CalculateRadius();

            if (r > FFloat.zero)
            {
                FVector3 center = FVector3.zero;

                if (gameObject != root)
                {
                    center = _pTransform.localPosition;
                }

                _shape = Parallel3D.CreateSphere(r, center);

                if(createUnityPhysicsCollider)
                {
                    var collider = gameObject.AddComponent<SphereCollider>();
                    collider.radius = (float)radius;
                }

                return _shape;
            }
            else
            {
                Debug.LogError("Invalid Size");
                return null;
            }
        }
    }
}

//namespace Parallel
//{
//    [RequireComponent(typeof(ParallelTransform))]
//    [ExecuteInEditMode]
//    public class PSphereCollider : MonoBehaviour, IParallelCollider3D
//    {
//        PShape3D _shape;
//        PFixture3D _fixture;

//        public FFloat Radius = FFloat.FromDivision(1, 2);

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
//            FFloat r = CalculateRadius();

//            if (r > FFloat.zero)
//            {
//                Gizmos.color = DebugSettings.ColliderOutlineColor;
//                Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, 1, 1));
//                DebugDraw.DrawHemiSphere(Vector3.zero, (float)r, Vector3.up);
//                DebugDraw.DrawHemiSphere(Vector3.zero, (float)r, Vector3.down);
//                Gizmos.matrix = Matrix4x4.identity;
//            }
//            else
//            {
//                Debug.LogError("Invalid Size");
//            }
//        }

//        FFloat CalculateRadius()
//        {
//            FFloat maxScale = FMath.Max(pTransform.localScale.x, pTransform.localScale.y, pTransform.localScale.z);
//            FFloat result = maxScale * Radius;
//            return result;
//        }

//        public PShape3D CreateShape()
//        {
//            FFloat r = CalculateRadius();

//            if (r > FFloat.zero)
//            {
//                _shape = Parallel3D.CreateSphere(r);
//                return _shape;
//            }
//            else
//            {
//                Debug.LogError("Invalid Size");
//                return null;
//            }
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

//            FFloat r = CalculateRadius();

//            if (r > FFloat.zero)
//            {
//                Parallel3D.UpdateSphere(_shape, _fixture, r);
//                ShapeDirty = false;
//            }
//        }
//    }
//}
