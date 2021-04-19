using UnityEngine;

namespace Parallel
{
    [RequireComponent(typeof(ParallelTransform))]
    [ExecuteInEditMode]
    public abstract class ParallelCollider2D : MonoBehaviour
    {
        protected PShape2D _shape;
        protected PFixture2D _fixture;
        protected ParallelTransform _pTransform;
        protected ParallelRigidbody2D _attachedBody;

        protected GameObject _root;

        protected bool _xzPlane;

        internal void XZPlane(bool xzPlane)
        {
            if (_shape != null)
            {
                //should never be here
                return;
            }
            _xzPlane = xzPlane;
        }

        [SerializeField]
        bool isTrigger = false;
        
        [SerializeField]
        FFloat _friction = FFloat.FromDivision(4, 10);

        [SerializeField]
        FFloat _bounciness = FFloat.FromDivision(2, 10);

        [SerializeField]
        internal FFloat _density = FFloat.FromDivision(1, 1);

        [SerializeField]
        internal bool _overideMass = false;

        [SerializeField]
        internal FFloat _customMass = FFloat.zero;

        public ParallelRigidbody2D attachedBody
        {
            get
            {
                return _attachedBody;
            }

        }

        [SerializeField]
        bool _useSpriteRendererSize = false;

        [SerializeField]
        FVector2 _spriteRendererSize = FVector2.one;

        protected FVector3 colliderScale
        {
            get
            {
                if (_useSpriteRendererSize && !_xzPlane)
                {
                    FFloat x = pTransform.localScale.x * _spriteRendererSize.x;
                    FFloat y = pTransform.localScale.y * _spriteRendererSize.y;
                    FFloat z = pTransform.localScale.z;

                    return new FVector3(x, y, z);
                }
                else{
                    return pTransform.localScale;
                }
            }
        }

        SpriteRenderer spriteRenderer
        {
            get
            {
                return GetComponent<SpriteRenderer>();
            }
        }

        public FFloat friction
        {
            get
            {
                return _friction;
            }
            set
            {
                _friction = value;
            }
        }

        public FFloat bounciness
        {
            get
            {
                return _bounciness;
            }
            set
            {
                _bounciness = value;
            }
        }

        public FFloat density
        {
            get
            {
                return _density;
            }
            set
            {
                _density = value;
            }
        }

        public ParallelTransform pTransform
        {
            get
            {
                if (_pTransform == null)
                {
                    _pTransform = GetComponent<ParallelTransform>();
                }

                return _pTransform;
            }
        }

        protected abstract void UpdateShape(GameObject root);
        public abstract PShape2D CreateShape(GameObject root);

        public void SetRootGameObject(GameObject root)
        {
            _root = root;
        }
        
        public void ReceiveFixture(PFixture2D fixture, ParallelRigidbody2D parallelRigidbody2D)
        {
            _attachedBody = parallelRigidbody2D;
            _fixture = fixture;
            _shape = Parallel2D.GetShapeOfFixture(fixture);
            Parallel2D.SetLayer(fixture, gameObject.layer, false);
            Parallel2D.SetFixtureProperties(fixture, isTrigger, _friction, _bounciness);
        }

        public void UpdateNativeShapeIfNecessary(GameObject root)
        {
            UpdateShape(root);
        }

        //============================== Unity Events ==============================
        void Update()
        {
            //only import from unity if in editing mode
            if (!Application.isPlaying)
            {
                if (spriteRenderer != null)
                {
                    if (spriteRenderer.drawMode == SpriteDrawMode.Sliced)
                    {
                        if(_spriteRendererSize != (FVector2)spriteRenderer.size)
                        {
                            _spriteRendererSize = (FVector2)spriteRenderer.size;
                            UnityEditor.EditorUtility.SetDirty(this);
                        }
                    }
                }
            }
        }

        void Reset()
        {
#if UNITY_EDITOR
            ImportFromUnity();
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        void ImportFromUnity()
        {
            _spriteRendererSize = FVector2.one;

            if (spriteRenderer != null)
            {
                if (spriteRenderer.drawMode == SpriteDrawMode.Sliced)
                {
                    _spriteRendererSize = (FVector2)spriteRenderer.size;
                }
            }
        }
    }
}
