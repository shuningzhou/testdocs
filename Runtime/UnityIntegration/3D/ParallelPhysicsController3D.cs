using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Parallel
{
    public class ParallelPhysicsController3D : MonoBehaviour
    {
        public SimulationMode simulationMode = SimulationMode.StatefulSleep;

        public LogLevel LoggingLevel = LogLevel.Error;

        public bool autoUpdate = true;
        public bool autoInitialization = true;

        public FFloat fixedUpdateTime = FFloat.FromDivision(2, 100);
        public int velocityIteration = 4;
        public FVector3 gravity = new FVector3(FFloat.zero, FFloat.FromDivision(-98, 10), FFloat.zero);

        bool _initialized = false;
        Action<ParallelRigidbody3D> _rollbackRemoveCallback;

        public void Initialize()
        {
            if (!_initialized)
            {

                _initialized = true;
                Parallel3D.gravity = gravity;
                Parallel3D.simulationMode = simulationMode;
                Parallel3D.SetLoggingLevel(LoggingLevel);
                Parallel3D._rollbackRemoveRigidbodyCallback3D = RollbackRemoveRigidbody;
                Time.fixedDeltaTime = (float)fixedUpdateTime;

                //using (new SProfiler($"ParallelPhysicsController3D Initialize"))
                {
                    ParallelRigidbody3D[] parallelRigidbody3Ds = FindObjectsOfType<ParallelRigidbody3D>().OrderBy(m => m.sceneIndex).ToArray();
                    List<ParallelJoint3D> jointsToInitialize = new List<ParallelJoint3D>();

                    foreach (ParallelRigidbody3D parallelRigidbody3D in parallelRigidbody3Ds)
                    {
                        Debug.Log("Init " + parallelRigidbody3D.name + " index=" + parallelRigidbody3D.sceneIndex);
                        if ( parallelRigidbody3D.parent != null)
                        {
                            //skip child rigidbody
                            Debug.Log("skipped as child rigidbody");
                            continue;
                        }

                        parallelRigidbody3D.Initialize();

                        ParallelJoint3D[] joints = parallelRigidbody3D.GetComponents<ParallelJoint3D>();

                        foreach (ParallelJoint3D j in joints)
                        {
                            jointsToInitialize.Add(j);
                        }
                    }

                    foreach (ParallelJoint3D j in jointsToInitialize)
                    {
                        j.InitializeJoint();
                    }
                }
            }
        }
        public void SetRollbackAddRigidbodyCallback(Action<UInt16, UInt16, IntPtr> callback)
        {
            Parallel3D._rollbackAddRigidbodyCallback3D = callback;
        }

        public void SetRollbackRemoveRigidbodyCallback(Action<ParallelRigidbody3D> callback)
        {
            _rollbackRemoveCallback = callback;
        }

        void RollbackRemoveRigidbody(UInt16 externalId, UInt16 bodyId)
        {
            Debug.Log($"RollbackRemoveRigidbody {bodyId}");
            PBody3D body = Parallel3D.FindBodyByID(bodyId);

            if(body == null)
            {
                //already removed as child rigibody
                Debug.Log("already removed as child rigibody");
                return;
            }

            ParallelRigidbody3D rigidbody3D = body.RigidBody as ParallelRigidbody3D;

           
            if (_rollbackRemoveCallback != null)
            {
                _rollbackRemoveCallback(rigidbody3D);
            }

            DestroyParallelObject(rigidbody3D.gameObject);
        }

        public IEnumerable<ParallelRigidbody3D> rigidbodies
        {
            get
            {
                foreach (var pair in Parallel3D.bodySortedList)
                {
                    PBody3D body = pair.Value;
                    ParallelRigidbody3D prb = body.RigidBody as ParallelRigidbody3D;
                    yield return prb;
                }
            }
        }

        public static GameObject InstantiateParallelObject(GameObject original, FVector3 position, FQuaternion rotation)
        {
            GameObject go = Instantiate(original, (Vector3)position, (Quaternion)rotation);

            ParallelTransform parallelTransform = go.GetComponent<ParallelTransform>();

            parallelTransform._internal_WriteTranform(position, rotation);

            ParallelRigidbody3D[] parallelRigidbody3Ds = go.GetComponentsInChildren<ParallelRigidbody3D>();
            List<ParallelJoint3D> jointsToInitialize = new List<ParallelJoint3D>();

            foreach (ParallelRigidbody3D parallelRigidbody3D in parallelRigidbody3Ds)
            {
                if(parallelRigidbody3D.parent == null)
                {
                    parallelRigidbody3D.Initialize();
                }
                
                ParallelJoint3D[] joints = parallelRigidbody3D.GetComponents<ParallelJoint3D>();

                foreach (ParallelJoint3D j in joints)
                {
                    jointsToInitialize.Add(j);
                }
            }

            foreach (ParallelJoint3D j in jointsToInitialize)
            {
                j.InitializeJoint();
            }

            return go;
        }

        public static GameObject RestoreParallelObject(GameObject original, UInt16 externalId, ushort bodyId, IntPtr pBody3D)
        {
            GameObject go = Instantiate(original, Vector3.zero, Quaternion.identity);
            ParallelRigidbody3D rigidbody3D = go.GetComponent<ParallelRigidbody3D>();
            rigidbody3D.Insert(bodyId, externalId, pBody3D);
            return go;
        }

        public static void DestroyParallelObject(GameObject gameObject)
        {
            ParallelRigidbody3D rigidbody3D = gameObject.GetComponent<ParallelRigidbody3D>();
            rigidbody3D.Destroy();
            Destroy(gameObject);
        }

        public static T AddJointToRigibody<T>(ParallelRigidbody3D rigidbody3D) where T : ParallelJoint3D
        {
            T joint = rigidbody3D.gameObject.AddComponent<T>();
            joint.InitializeJoint();
            return joint;
        }

        public static void DestroyJoint(ParallelJoint3D joint3D)
        {
            joint3D.DestroyJoint();
        }

        private void Awake()
        {
            if (autoInitialization)
            {
                Initialize();
            }
        }

        private void OnDestroy()
        {
            Parallel3D.CleanUp();
        }

        private void FixedUpdate()
        {
            if (autoUpdate)
            {
                Step(fixedUpdateTime);
                ExcuteUserCallbacks(fixedUpdateTime);
                ExcuteUserFixedUpdate(fixedUpdateTime);

                ParallelTransform.Commit(Time.fixedDeltaTime);
            }
        }

        public void Step(FFloat deltaTime)
        {
            Parallel3D.Step(deltaTime, velocityIteration, 1);
        }

        public void ExcuteUserCallbacks(FFloat deltaTime)
        {
            //using (new SProfiler($"==========ExcuteUserCallbacks========"))
            {
                Parallel3D.ExcuteUserCallbacks(deltaTime);
            }
        }

        public void ExcuteUserFixedUpdate(FFloat deltaTime)
        {
            Parallel3D.ExcuteUserFixedUpdate(deltaTime);
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
