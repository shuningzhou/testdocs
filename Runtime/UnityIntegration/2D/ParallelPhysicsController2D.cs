using UnityEngine;
using Parallel;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Parallel
{
    public class ParallelPhysicsController2D : MonoBehaviour
    {
        public Parallel.LogLevel LoggingLevel;
        public bool autoUpdate = true;
        public bool autoInitialization = true;
        public bool XZPlane = false;
        static bool sXZPlane = false;

        public FFloat fixedUpdateTime = FFloat.FromDivision(2, 100);
        public int velocityIteration = 4;
        public bool allowSleep = true;
        public bool warmStart = true;
        public FVector2 gravity = new FVector2(FFloat.zero, FFloat.FromDivision(-98, 10));

        bool _initialized = false;
        Action<ParallelRigidbody2D> _rollbackRemoveCallback;

        public void Initialize()
        {
            if(!_initialized)
            {
                _initialized = true;
                sXZPlane = XZPlane;
                Parallel2D.gravity = gravity;
                Parallel2D.allowSleep = allowSleep;
                Parallel2D.warmStart = warmStart;
                Parallel2D.SetLoggingLevel(LoggingLevel);
                Parallel2D._rollbackRemoveRigidbodyCallback = RollbackRemoveRigidbody;
                Time.fixedDeltaTime = (float)fixedUpdateTime;

                ParallelRigidbody2D[] parallelRigidbody2Ds = FindObjectsOfType<ParallelRigidbody2D>().OrderBy(m => m.sceneIndex).ToArray();

                List<ParallelJoint2D> jointsToInitialize = new List<ParallelJoint2D>();
                foreach(ParallelRigidbody2D parallelRigidbody2D in parallelRigidbody2Ds)
                {
                    //Debug.Log("Init " + parallelRigidbody2D.name + " index=" + parallelRigidbody2D.sceneIndex);
                    Debug.Log("Init " + parallelRigidbody2D.name + " index=" + parallelRigidbody2D.sceneIndex);
                    if (parallelRigidbody2D.parent != null)
                    {
                        //skip child rigidbody
                        Debug.Log("skipped as child rigidbody");
                        continue;
                    }

                    parallelRigidbody2D.XZPlane(XZPlane);
                    parallelRigidbody2D.Initialize();

                    ParallelJoint2D[] joints = parallelRigidbody2D.GetComponents<ParallelJoint2D>();

                    foreach(ParallelJoint2D j in joints)
                    {
                        jointsToInitialize.Add(j);
                    }
                }

                foreach(ParallelJoint2D j in jointsToInitialize)
                {
                    j.InitializeJoint();
                }
            }
        }

        public void SetRollbackAddRigidbodyCallback(Action<UInt32, UInt16, IntPtr> callback)
        {
            Parallel2D._rollbackAddRigidbodyCallback = callback;
        }

        public void SetRollbackRemoveRigidbodyCallback(Action<ParallelRigidbody2D> callback)
        {
            _rollbackRemoveCallback = callback;
        }

        void RollbackRemoveRigidbody(uint externalId, ushort bodyId)
        {
            Debug.Log($"RollbackRemoveRigidbody {bodyId}");
            PBody2D body = Parallel2D.FindBodyByID(bodyId);

            if (body == null)
            {
                //already removed as child rigibody
                Debug.Log("already removed as child rigibody");
                return;
            }

            ParallelRigidbody2D parallelRigidbody2D = body.RigidBody as ParallelRigidbody2D;

            if(_rollbackRemoveCallback != null)
            {
                _rollbackRemoveCallback(parallelRigidbody2D);
            }

            DestroyParallelObject(parallelRigidbody2D.gameObject);
        }

        public IEnumerable<ParallelRigidbody2D> rigidbodies
        {
            get
            {
                foreach (var pair in Parallel2D.bodySortedList)
                {
                    PBody2D body = pair.Value;
                    ParallelRigidbody2D prb = body.RigidBody as ParallelRigidbody2D;
                    yield return prb;
                }
            }
        }

        public static GameObject InstantiateParallelObject(GameObject original, FVector3 position, FQuaternion rotation)
        {
            GameObject go = Instantiate(original, (Vector3)position, (Quaternion)rotation);

            ParallelTransform parallelTransform = go.GetComponent<ParallelTransform>();

            parallelTransform._internal_WriteTranform(position, rotation);

            ParallelRigidbody2D[] parallelRigidbody2Ds = go.GetComponentsInChildren<ParallelRigidbody2D>();
            List<ParallelJoint2D> jointsToInitialize = new List<ParallelJoint2D>();

            foreach (ParallelRigidbody2D parallelRigidbody2D in parallelRigidbody2Ds)
            {
                parallelRigidbody2D.XZPlane(sXZPlane);
                parallelRigidbody2D.Initialize();

                ParallelJoint2D[] joints = parallelRigidbody2D.GetComponents<ParallelJoint2D>();

                foreach (ParallelJoint2D j in joints)
                {
                    jointsToInitialize.Add(j);
                }
            }

            foreach (ParallelJoint2D j in jointsToInitialize)
            {
                j.InitializeJoint();
            }

            return go;
        }

        public static GameObject RestoreParallelObject(GameObject original, uint externalId, ushort bodyId, IntPtr pBody2D)
        {
            GameObject go = Instantiate(original, Vector3.zero, Quaternion.identity);
            ParallelRigidbody2D rigidbody2D = go.GetComponent<ParallelRigidbody2D>();
            rigidbody2D.Insert(bodyId, externalId, pBody2D);
            return go;
        }

        public static void DestroyParallelObject(GameObject gameObject)
        {
            ParallelRigidbody2D rigidbody2D = gameObject.GetComponent<ParallelRigidbody2D>();
            rigidbody2D.Destroy();
            Destroy(gameObject);
        }

        public static T AddJointToRigibody<T>(ParallelRigidbody2D rigidbody2D) where T : ParallelJoint2D
        {
            T joint = rigidbody2D.gameObject.AddComponent<T>();
            joint.InitializeJoint();
            return joint;
        }

        public static void DestroyJoint(ParallelJoint2D joint2D)
        {
            joint2D.DestroyJoint();
        }

        private void Awake()
        {
            if(autoInitialization)
            {
                Initialize();
            }
        }

        private void OnDestroy()
        {
            Parallel2D.CleanUp();
        }

        private void FixedUpdate()
        {
            if(autoUpdate)
            {
                Step(fixedUpdateTime);
                ExcuteUserCallbacks(fixedUpdateTime);
                ExcuteUserFixedUpdate(fixedUpdateTime);
            }
        }

        public void Step(FFloat deltaTime)
        {
            //using (new SProfiler($"==========ExcuteUserCallbacks========"))
            {
                Parallel2D.Step(deltaTime, velocityIteration, 1);
            }            
        }

        public void ExcuteUserCallbacks(FFloat deltaTime)
        {
            Parallel2D.ExcuteUserCallbacks(deltaTime);
        }

        public void ExcuteUserFixedUpdate(FFloat deltaTime)
        {
            Parallel2D.ExcuteUserFixedUpdate(deltaTime);
        }

        public void Update()
        {
            if (!autoUpdate)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Step(fixedUpdateTime);
                    ExcuteUserCallbacks(fixedUpdateTime);
                }
            }
        }
    }
}
