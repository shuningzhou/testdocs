﻿using UnityEngine;

namespace Parallel
{
    [RequireComponent(typeof(ParallelTransform))]
    public abstract class ParallelCollider3D : MonoBehaviour
    {
        protected PShape3D _shape;
        protected PFixture3D _fixture;
        protected ParallelTransform _pTransform;
        protected ParallelRigidbody3D _attachedBody;

        [SerializeField]
        internal int _shapeID;

        public bool isTrigger = false;
        public bool createUnityPhysicsCollider = false;
        
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

        public bool ShapeDirty { get; set; }

        public ParallelRigidbody3D attachedBody
        {
            get
            {
                return _attachedBody;
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

        public abstract void UpdateShape(GameObject root);
        public abstract PShape3D CreateShape(GameObject root);

        public void ReceiveFixture(PFixture3D fixture, ParallelRigidbody3D parallelRigidbody3D)
        {
            _attachedBody = parallelRigidbody3D;
            _fixture = fixture;
            _shapeID = fixture.shapeID;
            _shape = Parallel3D.GetShapeOfFixture(fixture);
            Parallel3D.SetLayer(fixture, gameObject.layer, false);
            Parallel3D.SetFixtureProperties(fixture, isTrigger, _friction, _bounciness);
        }

        public void UpdateNativeShapeIfNecessary(GameObject root)
        {
            if (!ShapeDirty)
            {
                return;
            }

            UpdateShape(root);

            ShapeDirty = false;
        }
    }
}
