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

         [SerializeField]
        bool isTrigger = false;
        
        [SerializeField]
        Fix64 _friction = Fix64.FromDivision(4, 10);

        [SerializeField]
        Fix64 _bounciness = Fix64.FromDivision(2, 10);

        [SerializeField]
        Fix64 _density = Fix64.FromDivision(1, 1);

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
        Fix64Vec2 _spriteRendererSize = Fix64Vec2.one;

        protected Fix64Vec3 colliderScale
        {
            get
            {
                if (_useSpriteRendererSize)
                {
                    Fix64 x = pTransform.localScale.x * _spriteRendererSize.x;
                    Fix64 y = pTransform.localScale.y * _spriteRendererSize.y;
                    Fix64 z = pTransform.localScale.z;

                    return new Fix64Vec3(x, y, z);
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

        public Fix64 friction
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

        public Fix64 bounciness
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

        public Fix64 density
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
                        if(_spriteRendererSize != (Fix64Vec2)spriteRenderer.size)
                        {
                            _spriteRendererSize = (Fix64Vec2)spriteRenderer.size;
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
            _spriteRendererSize = Fix64Vec2.one;

            if (spriteRenderer != null)
            {
                if (spriteRenderer.drawMode == SpriteDrawMode.Sliced)
                {
                    _spriteRendererSize = (Fix64Vec2)spriteRenderer.size;
                }
            }
        }
    }
}
