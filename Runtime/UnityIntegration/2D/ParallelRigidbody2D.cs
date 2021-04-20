﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Parallel
{
    [RequireComponent(typeof(ParallelTransform))]
    [ExecuteInEditMode]
    public class ParallelRigidbody2D : MonoBehaviour, IParallelRigidbody2D
    {
        ParallelTransform _pTransform;
        internal bool _xzPlane;

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

        IParallelFixedUpdate[] parallelFixedUpdates = new IParallelFixedUpdate[0];
        IParallelCollision2D[] parallelCollisions = new IParallelCollision2D[0];
        IParallelTrigger2D[] parallelTriggers = new IParallelTrigger2D[0];
        ParallelCollider2D[] colliders = new ParallelCollider2D[0];

        public List<ParallelRigidbody2D> childrenRigidbodies = new List<ParallelRigidbody2D>();
        public ParallelRigidbody2D parent;

        internal PBody2D _body2D;
        [SerializeField]
        int _bodyId;

        [SerializeField]
        internal int sceneIndex;

        public Transform parentTransform;
        public uint externalId;

        //============================== Body properties ==============================
        [SerializeField]
        BodyType _bodyType = Parallel.BodyType.Dynamic;
        [SerializeField]
        FFloat _linearDampling = FFloat.zero;
        [SerializeField]
        FFloat _angularDamping = FFloat.FromDivision(5, 100);
        [SerializeField]
        FFloat _gravityScale = FFloat.one;
        [SerializeField]
        bool _fixedRotation = false;

        [SerializeField]
        FFloat _mass = FFloat.zero;

        public int bodyId
        {
            get
            {
                return _bodyId;
            }
        }

        public FFloat mass
        {
            get
            {
                return _body2D.mass;
            }
        }

        public BodyType bodyType
        {
            get
            {
                return _bodyType;
            }
            set
            {
                _bodyType = value;
            }
        }

        public FFloat linearDampling
        {
            get
            {
                return _linearDampling;
            }
            set
            {
                _linearDampling = value;
            }
        }

        public FFloat angularDamping
        {
            get
            {
                return _angularDamping;
            }
            set
            {
                _angularDamping = value;
            }
        }

        public FFloat gravityScale
        {
            get
            {
                return _gravityScale;
            }
            set
            {
                _gravityScale = value;
            }
        }

        public bool fixedRotation
        {
            get
            {
                return _fixedRotation;
            }
            set
            {
                _fixedRotation = value;
            }
        }

        public void UpdateBodyProperties()
        {
            Parallel2D.UpdateBodyProperties(_body2D, (int)bodyType, linearDampling, angularDamping, fixedRotation, gravityScale);
        }

        //============================== Velocity ==============================
        public FVector2 LinearVelocity
        {
            get
            {
                return _body2D.linearVelocity;
            }
            set
            {
                _body2D.linearVelocity = value;
                 Parallel2D.UpdateBodyVelocity(_body2D, LinearVelocity, AngularVelocity);
            }
        }

        // rad/sec, z-axis (out of the screen)
        public FFloat AngularVelocity
        {
            get
            {
                return _body2D.angularVelocity;
            }
            set
            {
                _body2D.angularVelocity = value;
                 Parallel2D.UpdateBodyVelocity(_body2D, LinearVelocity, AngularVelocity);
            }
        }

        //============================== Force and Torque ==============================
        //Apply a force to the center of mass
        public void ApplyForce(FVector2 force)
        {
            Parallel2D.ApplyForceToCenter(_body2D, force);
        }

        //Apply a force at a world point
        public void ApplyForce(FVector2 force, FVector2 worldPoint)
        {
            Parallel2D.ApplyForce(_body2D, worldPoint, force);
        }

        //Apply an impulse to the center of mass. This immediately modifies the velocity.
        public void ApplyLinearImpulse(FVector2 impluse)
        {
            Parallel2D.ApplyLinearImpulseToCenter(_body2D, impluse);
        }

        /// Apply an impulse at a point. This immediately modifies the velocity.
        /// It also modifies the angular velocity if the point of application
        /// is not at the center of mass.
        public void ApplyLinearImpluse(FVector2 impluse, FVector2 worldPoint)
        {
            Parallel2D.ApplyLinearImpulse(_body2D, worldPoint, impluse);
        }

        /// Apply a torque. This affects the angular velocity
        /// without affecting the linear velocity of the center of mass.
        /// z-axis (out of the screen)
        public void ApplyTorque(FFloat torque)
        {
            Parallel2D.ApplyTorque(_body2D, torque);
        }

        /// Apply an angular impulse. This immediately modifies the angular velocity
        public void ApplyAngularImpulse(FFloat impulse)
        {
            Parallel2D.ApplyAngularImpulse(_body2D, impulse);
        }

        //============================== IParallelRigidBody ==============================
        public void OnParallelCollisionEnter(PCollision2D collision)
        {
            foreach (IParallelCollision2D parallelCollision in parallelCollisions)
            {
                parallelCollision.OnParallelCollisionEnter2D(collision);
            }
        }

        public void OnParallelCollisionStay(PCollision2D collision)
        {
            foreach (IParallelCollision2D parallelCollision in parallelCollisions)
            {
                parallelCollision.OnParallelCollisionStay2D(collision);
            }
        }

        public void OnParallelCollisionExit(PCollision2D collision)
        {
            foreach (IParallelCollision2D parallelCollision in parallelCollisions)
            {
                parallelCollision.OnParallelCollisionExit2D(collision);
            }
        }

        public void OnParallelTriggerEnter(IParallelRigidbody2D other)
        {
            foreach (IParallelTrigger2D trigger in parallelTriggers)
            {
                trigger.OnParallelTriggerEnter2D(other as ParallelRigidbody2D);
            }
        }

        public void OnParallelTriggerStay(IParallelRigidbody2D other)
        {
            foreach (IParallelTrigger2D trigger in parallelTriggers)
            {
                trigger.OnParallelTriggerStay2D(other as ParallelRigidbody2D);
            }
        }

        public void OnParallelTriggerExit(IParallelRigidbody2D other)
        {
            foreach (IParallelTrigger2D trigger in parallelTriggers)
            {
                trigger.OnParallelTriggerExit2D(other as ParallelRigidbody2D);
            }
        }

        public void OnTransformUpdated()
        {
            if (parent != null)
            {
                return;
            }

            if(_xzPlane)
            {
                pTransform._internal_WriteTranform2D(_body2D.position, new FVector3(FFloat.zero, -FFloat.RadToDeg(_body2D.angle), FFloat.zero), _xzPlane);
            }
            else
            {
                pTransform._internal_WriteTranform2D(_body2D.position, new FVector3(FFloat.zero, FFloat.zero, FFloat.RadToDeg(_body2D.angle)), _xzPlane);
            }

            

            UpdateChildren();
        }

        void UpdateChildren()
        {
            //update children rigidbodies
            foreach (ParallelRigidbody2D child in childrenRigidbodies)
            {
                //todo: support xzplane
                Parallel2D.UpdateBodyTransForm(child._body2D, (FVector2)child.pTransform.position, FFloat.DegToRad(child.pTransform.eulerAngles.z));

                child.UpdateChildren();
            }
        }

        public void Step(FFloat timeStep)
        {
            foreach(IParallelFixedUpdate parallelFixedUpdate in parallelFixedUpdates)
            {
                parallelFixedUpdate.ParallelFixedUpdate(timeStep);
            }
        }

        public void UpdateRigidbodyHierarchy()
        {
            if (parent != null)
            {
                parent.childrenRigidbodies.Remove(this);
            }

            parent = ParallelUtil.FindParentComponent<ParallelRigidbody2D>(this.gameObject);

            if (parent != null)
            {
                if (!parent.childrenRigidbodies.Contains(this))
                {
                    parent.childrenRigidbodies.Add(this);
                }
                UnityEditor.EditorUtility.SetDirty(parent);
            }

            childrenRigidbodies.RemoveAll(item => item == null);
        }

        //============================== Unity Events ==============================
        void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (sceneIndex != transform.GetSiblingIndex())
                {
                    sceneIndex = transform.GetSiblingIndex();
                    UnityEditor.EditorUtility.SetDirty(this);
                }

                if(transform.parent != null)
                {
                    if (parentTransform == null || parentTransform != transform.parent)
                    {
                        parentTransform = transform.parent;
                        UpdateRigidbodyHierarchy();
                        UnityEditor.EditorUtility.SetDirty(this);
                    }
                }
            }
#endif
        }

        void OnDestroy()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (parent != null)
                {
                    parent.childrenRigidbodies.Remove(this);
                    UnityEditor.EditorUtility.SetDirty(parent);
                }
            }
#endif
        }

        internal void XZPlane(bool xzPlane)
        {
            if (_body2D != null)
            {
                //should never be here
                return;
            }
            _xzPlane = xzPlane;
        }

        internal void Initialize()
        {
            parallelFixedUpdates = GetComponents<IParallelFixedUpdate>();
            parallelCollisions = GetComponents<IParallelCollision2D>();
            parallelTriggers = GetComponents<IParallelTrigger2D>();

            //pTransform.ImportFromUnity();

            colliders = GetComponentsInChildren<ParallelCollider2D>();

            FVector2 pos = (FVector2)pTransform.position;
            FFloat angle = pTransform.rotation.GetZAngle();

            if (_xzPlane)
            {
                pos = pTransform.position.xz;
                angle = -pTransform.rotation.GetYAngle();
            }

            _body2D = Parallel2D.AddBody(
                                                (int)bodyType,
                                                pos,
                                                angle,
                                                linearDampling,
                                                angularDamping,
                                                fixedRotation,
                                                gravityScale,
                                                this,
                                                externalId);

            _bodyId = _body2D.BodyID;

            //mark all children rigidbodies as static and add to the physics world 
            //all colliders of the children will be added first
            foreach (ParallelRigidbody2D child in childrenRigidbodies)
            {
                child.bodyType = BodyType.Static;
                child.Initialize();
            }

            foreach (ParallelCollider2D collider in colliders)
            {
                if (collider.attachedBody != null)
                {
                    continue;
                }

                collider.SetRootGameObject(gameObject);
                collider.XZPlane(_xzPlane);
                PShape2D shape = collider.CreateShape(gameObject);

                if (shape == null)
                {
                    Debug.LogError("Failed to create collider shape");
                    continue;
                }

                FFloat mass = FFloat.zero;

                if (collider._overideMass)
                {
                    mass = collider._customMass;
                }

                PFixture2D fixture2D = Parallel2D.AddFixture(_body2D, shape, collider.density, mass);

                collider.ReceiveFixture(fixture2D, this);
            }

            if (bodyType == BodyType.Static)
            {
                return;
            }

            Parallel2D.ReadBodyMassInfo(_body2D);

            _mass = _body2D.mass;
        }

        internal PBody2D Insert(UInt16 bId, UInt32 exId, IntPtr previousBody)
        {
            parallelFixedUpdates = GetComponents<IParallelFixedUpdate>();
            parallelCollisions = GetComponents<IParallelCollision2D>();
            parallelTriggers = GetComponents<IParallelTrigger2D>();

            colliders = GetComponentsInChildren<ParallelCollider2D>();

            FVector2 pos = (FVector2)pTransform.position;
            FFloat angle = pTransform.rotation.GetZAngle();

            if (_xzPlane)
            {
                pos = pTransform.position.xz;
                angle = -pTransform.rotation.GetYAngle();
            }

            _body2D = Parallel2D.InsertBody(
                                                (int)bodyType,
                                                pos,
                                                angle,
                                                linearDampling,
                                                angularDamping,
                                                fixedRotation,
                                                gravityScale,
                                                this,
                                                bId,
                                                exId,
                                                previousBody
                                                );

            _bodyId = bId;
            externalId = exId;

            //mark all children rigidbodies as static and add to the physics world 
            //all colliders of the children will be added first
            PBody2D lastInsertedBody = _body2D;
            bId = (UInt16)(lastInsertedBody.BodyID + 1);
            exId = 0; //does not matter because children are created when parent is created, for now, we don't allow create/destory children directly

            foreach (ParallelRigidbody2D child in childrenRigidbodies)
            {
                child.bodyType = BodyType.Static;

                lastInsertedBody = child.Insert(bId, exId, lastInsertedBody.IntPointer);

                bId = (UInt16)(lastInsertedBody.BodyID + 1);
            }

            foreach (ParallelCollider2D collider in colliders)
            {
                if (collider.attachedBody != null)
                {
                    continue;
                }

                collider.SetRootGameObject(gameObject);
                PShape2D shape = collider.CreateShape(gameObject);

                if (shape == null)
                {
                    Debug.LogError("Failed to create collider shape");
                    continue;
                }

                FFloat mass = FFloat.zero;

                if (collider._overideMass)
                {
                    mass = collider._customMass;
                }

                PFixture2D fixture2D = Parallel2D.AddFixture(_body2D, shape, collider.density, mass);

                collider.ReceiveFixture(fixture2D, this);
            }

            if (bodyType == BodyType.Static)
            {
                return lastInsertedBody;
            }

            Parallel2D.ReadBodyMassInfo(_body2D);

            _mass = _body2D.mass;

            return lastInsertedBody;
        }

        internal void Destroy()
        {
            Parallel2D.DestoryBody(_body2D, this);

            foreach (ParallelRigidbody2D child in childrenRigidbodies)
            {
                child.Destroy();
            }
        }

    }
}