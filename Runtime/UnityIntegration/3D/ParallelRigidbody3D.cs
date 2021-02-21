using UnityEngine;

namespace Parallel
{
    [RequireComponent(typeof(ParallelTransform))]
    public class ParallelRigidbody3D : MonoBehaviour, IParallelRigidbody3D
    {
        ParallelTransform _pTransform;

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
        IParallelCollision3D[] parallelCollisions = new IParallelCollision3D[0];
        IParallelTrigger3D[] parallelTriggers = new IParallelTrigger3D[0];

        ParallelCollider3D[] colliders = new ParallelCollider3D[0];

        internal PBody3D _body3D;

        [SerializeField]
        internal int sceneIndex;

        [SerializeField]
        int _bodyId;
        [SerializeField]
        bool _awake;

        //============================== Body properties ==============================
        [SerializeField]
        BodyType _bodyType = Parallel.BodyType.Dynamic;

        [SerializeField]
        Fix64Vec3 _linearDamping = Fix64Vec3.zero;

        [SerializeField]
        Fix64Vec3 _angularDamping = new Fix64Vec3(Fix64.FromDivision(5, 100), Fix64.FromDivision(5, 100), Fix64.FromDivision(5, 100));

        [SerializeField]
        Fix64Vec3 _gravityScale = Fix64Vec3.one;

        [SerializeField]
        bool _fixedRotationX = false;
        [SerializeField]
        bool _fixedRotationY = false;
        [SerializeField]
        bool _fixedRotationZ = false;

        //only used when creating the body
        [SerializeField]
        bool _overideMassData = false;
        [SerializeField]
        Fix64 _customMass = Fix64.one;
        [SerializeField]
        Fix64Vec3 _customCenterOfMass = Fix64Vec3.zero;

        public int bodyId
        {
            get
            {
                return _bodyId;
            }
        }

        public bool isAwake
        {
            get
            {
                return _awake;
            }
        }

        public Fix64Vec3 centerOfMass
        {
            get
            {
                //TODO: get com from physics engine
                return _customCenterOfMass;
            }
        }

        public Fix64 mass
        {
            get
            {
                return _body3D.mass;
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

        public Fix64Vec3 linearDamping
        {
            get
            {
                return _linearDamping;
            }
            set
            {
                _linearDamping = value;
            }
        }

        public Fix64Vec3 angularDamping
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

        public Fix64Vec3 gravityScale
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

        public bool fixedRotationX
        {
            get
            {
                return _fixedRotationX;
            }
            set
            {
                _fixedRotationX = value;
            }
        }

        public bool fixedRotationY
        {
            get
            {
                return _fixedRotationY;
            }
            set
            {
                _fixedRotationY = value;
            }
        }

        public bool fixedRotationZ
        {
            get
            {
                return _fixedRotationZ;
            }
            set
            {
                _fixedRotationZ = value;

            }
        }

        public void UpdateBodyProperties()
        {
            Parallel3D.UpdateBodyProperties(
                _body3D,
                (int)bodyType,
                linearDamping,
                angularDamping,
                gravityScale,
                fixedRotationX,
                fixedRotationY,
                fixedRotationZ);
        }

        //============================== Velocity ==============================
        public Fix64Vec3 LinearVelocity
        {
            get
            {
                return _body3D.linearVelocity;
            }
            set
            {
                _body3D.linearVelocity = value;
                Parallel3D.UpdateBodyVelocity(_body3D, LinearVelocity, AngularVelocity);
            }
        }

        public Fix64Vec3 AngularVelocity
        {
            get
            {
                return _body3D.angularVelocity;
            }
            set
            {
                _body3D.angularVelocity = value;
                Parallel3D.UpdateBodyVelocity(_body3D, LinearVelocity, AngularVelocity);
            }
        }

        //
        public Fix64Vec3 GetPointVelocity(Fix64Vec3 point)
        {
            return Parallel3D.GetPointVelocity(_body3D, point);
        }

        //============================== Force and Torque ==============================
        //Apply a force to the center of mass
        public void ApplyForce(Fix64Vec3 force)
        {
            Parallel3D.ApplyForceToCenter(_body3D, force);
        }

        //Apply a force at a world point
        public void ApplyForce(Fix64Vec3 force, Fix64Vec3 worldPoint)
        {
            Parallel3D.ApplyForce(_body3D, worldPoint, force);
        }

        //Apply an impulse to the center of mass. This immediately modifies the velocity.
        public void ApplyLinearImpulse(Fix64Vec3 impluse)
        {
            Parallel3D.ApplyLinearImpulseToCenter(_body3D, impluse);
        }

        /// Apply an impulse at a point. This immediately modifies the velocity.
        /// It also modifies the angular velocity if the point of application
        /// is not at the center of mass.
        public void ApplyLinearImpluse(Fix64Vec3 impluse, Fix64Vec3 worldPoint)
        {
            Parallel3D.ApplyLinearImpulse(_body3D, worldPoint, impluse);
        }

        /// Apply a torque. This affects the angular velocity
        /// without affecting the linear velocity of the center of mass.
        /// z-axis (out of the screen)
        public void ApplyTorque(Fix64Vec3 torque)
        {
            Parallel3D.ApplyTorque(_body3D, torque);
        }

        /// Apply an angular impulse. This immediately modifies the angular velocity
        public void ApplyAngularImpulse(Fix64Vec3 impulse)
        {
            Parallel3D.ApplyAngularImpulse(_body3D, impulse);
        }

        //============================== IParallelRigidBody ==============================
        public ParallelCollider3D FindCollider(byte shapeID)
        {
            foreach (ParallelCollider3D collider in colliders)
            {
                if (collider._shapeID == shapeID)
                {
                    return collider;
                }
            }

            return null;
        }

        public void OnParallelCollisionEnter(PCollision3D collision)
        {
            foreach (IParallelCollision3D parallelCollision in parallelCollisions)
            {
                parallelCollision.OnParallelCollisionEnter3D(collision);
            }
        }

        public void OnParallelCollisionStay(PCollision3D collision)
        {
            foreach (IParallelCollision3D parallelCollision in parallelCollisions)
            {
                parallelCollision.OnParallelCollisionStay3D(collision);
            }
        }

        public void OnParallelCollisionExit(PCollision3D collision)
        {
            foreach (IParallelCollision3D parallelCollision in parallelCollisions)
            {
                parallelCollision.OnParallelCollisionExit3D(collision);
            }
        }

        public void OnParallelTriggerEnter(IParallelRigidbody3D other, byte selfShapeID, byte otherShapeID)
        {
            ParallelCollider3D selfCollider = FindCollider(selfShapeID);

            if(selfCollider == null)
            {
                return;
            }

            if(!selfCollider.isTrigger)
            {
                return;
            }

            ParallelCollider3D otherCollider = FindCollider(otherShapeID);

            foreach (IParallelTrigger3D trigger in parallelTriggers)
            {
                trigger.OnParallelTriggerEnter3D(other as ParallelRigidbody3D, otherCollider);
            }
        }

        public void OnParallelTriggerStay(IParallelRigidbody3D other, byte selfShapeID, byte otherShapeID)
        {
            ParallelCollider3D selfCollider = FindCollider(selfShapeID);

            if (selfCollider == null)
            {
                return;
            }

            if (!selfCollider.isTrigger)
            {
                return;
            }

            ParallelCollider3D otherCollider = FindCollider(otherShapeID);

            foreach (IParallelTrigger3D trigger in parallelTriggers)
            {
                trigger.OnParallelTriggerStay3D(other as ParallelRigidbody3D, otherCollider);
            }
        }

        public void OnParallelTriggerExit(IParallelRigidbody3D other, byte selfShapeID, byte otherShapeID)
        {
            ParallelCollider3D selfCollider = FindCollider(selfShapeID);

            if (selfCollider == null)
            {
                return;
            }

            if (!selfCollider.isTrigger)
            {
                return;
            }

            ParallelCollider3D otherCollider = FindCollider(otherShapeID);

            foreach (IParallelTrigger3D trigger in parallelTriggers)
            {
                trigger.OnParallelTriggerExit3D(other as ParallelRigidbody3D, otherCollider);
            }
        }

        public void OnTransformUpdated()
        {
            _awake = _body3D.IsAwake;
            pTransform._internal_WriteTranform(_body3D.position, _body3D.orientation);
        }

        public void Step(Fix64 timeStep)
        {
            foreach (IParallelFixedUpdate parallelFixedUpdate in parallelFixedUpdates)
            {
                parallelFixedUpdate.ParallelFixedUpdate(timeStep);
            }
        }

        //============================== Unity Events ==============================
        void OnValidate()
        {
            sceneIndex = transform.GetSiblingIndex();
        }

        internal void Initialize()
        {
            parallelFixedUpdates = GetComponents<IParallelFixedUpdate>();
            parallelCollisions = GetComponents<IParallelCollision3D>();
            parallelTriggers = GetComponents<IParallelTrigger3D>();

            //pTransform.ImportFromUnity();

            colliders = GetComponentsInChildren<ParallelCollider3D>();

            _body3D = Parallel3D.AddBody(
                                        (int)bodyType,
                                        pTransform.position,
                                        pTransform.rotation,
                                        linearDamping,
                                        angularDamping,
                                        gravityScale,
                                        fixedRotationX,
                                        fixedRotationY,
                                        fixedRotationZ,
                                        this);

            _bodyId = _body3D.BodyID;

            foreach (ParallelCollider3D collider in colliders)
            {
                PShape3D shape = collider.CreateShape(gameObject);

                if (shape == null)
                {
                    Debug.LogError("Failed to create collider shape");
                    continue;
                }

                PFixture3D fixture = Parallel3D.AddFixture(_body3D, shape, (Fix64)1);

                collider.ReceiveFixture(fixture);
            }

            if(_overideMassData)
            {
                if (_customCenterOfMass != null)
                {
                    Fix64Vec3 com = _customCenterOfMass;
                    //Debug.Log(com);
                    Parallel3D.UpdateMassData(_body3D, _customMass, com);


                }
                else
                {
                    Parallel3D.UpdateMass(_body3D, _customMass);
                }
            }

            Parallel3D.ReadBodyMassInfo(_body3D);
        }

        void OnEnable()
        {
            Parallel3D.SetEnabled(_body3D, true);
        }

        void OnDisable()
        {
            Parallel3D.SetEnabled(_body3D, false);
        }

        // Path finding
        public void AddToWorldForPathFinding()
        {
            if(_bodyType != BodyType.Static)
            {
                return;
            }

            ParallelCollider3D[] colliders = GetComponentsInChildren<ParallelCollider3D>();
            PBody3D body = Parallel3D.AddBody(
                            (int)bodyType,
                            pTransform.position,
                            pTransform.rotation,
                            linearDamping,
                            angularDamping,
                            gravityScale,
                            fixedRotationX,
                            fixedRotationY,
                            fixedRotationZ,
                            this);

            foreach (ParallelCollider3D collider in colliders)
            {
                PShape3D shape = collider.CreateShape(gameObject);

                if (shape == null)
                {
                    Debug.LogError("Failed to create collider shape");
                    continue;
                }

                Parallel3D.AddFixture(body, shape, (Fix64)1);
            }

            Parallel3D.ReadBodyMassInfo(_body3D);
        }

        internal void Destroy()
        {
            Parallel3D.DestoryBody(_body3D, this);
        }
    }
}