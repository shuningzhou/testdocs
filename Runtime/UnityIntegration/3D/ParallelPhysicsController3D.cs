using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Parallel
{
    public class ParallelPhysicsController3D : MonoBehaviour
    {
        public Parallel.LogLevel LoggingLevel;
        public bool autoUpdate = true;
        public bool autoInitialization = true;

        public Fix64 fixedUpdateTime = Fix64.FromDivision(2, 100);
        public int velocityIteration = 4;
        public bool allowSleep = true;
        public bool warmStart = true;
        public Fix64Vec3 gravity = new Fix64Vec3(Fix64.zero, Fix64.FromDivision(-98, 10), Fix64.zero);

        bool _initialized = false;

        public void Initialize()
        {
            if (!_initialized)
            {

                _initialized = true;
                Parallel3D.gravity = gravity;
                Parallel3D.allowSleep = allowSleep;
                Parallel3D.warmStart = warmStart;
                Parallel3D.SetLoggingLevel(LoggingLevel);
                Time.fixedDeltaTime = (float)fixedUpdateTime;

                //using (new SProfiler($"ParallelPhysicsController3D Initialize"))
                {
                    ParallelRigidbody3D[] parallelRigidbody3Ds = FindObjectsOfType<ParallelRigidbody3D>().OrderBy(m => m.sceneIndex).ToArray();
                    List<ParallelJoint3D> jointsToInitialize = new List<ParallelJoint3D>();

                    foreach (ParallelRigidbody3D parallelRigidbody3D in parallelRigidbody3Ds)
                    {
                        //Debug.Log("Init " + parallelRigidbody3D.name + " index=" + parallelRigidbody3D.sceneIndex);

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

        public static GameObject InstantiateParallelObject(GameObject original, Fix64Vec3 position, Fix64Quat rotation)
        {
            GameObject go = Instantiate(original, (Vector3)position, (Quaternion)rotation);

            ParallelTransform parallelTransform = go.GetComponent<ParallelTransform>();

            parallelTransform.position = position;
            parallelTransform.rotation = rotation;

            ParallelRigidbody3D[] parallelRigidbody3Ds = go.GetComponentsInChildren<ParallelRigidbody3D>();
            List<ParallelJoint3D> jointsToInitialize = new List<ParallelJoint3D>();

            foreach (ParallelRigidbody3D parallelRigidbody3D in parallelRigidbody3Ds)
            {
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
            }
        }

        public void Step(Fix64 deltaTime)
        {
            Parallel3D.Step(deltaTime, velocityIteration, 1);
        }

        public void ExcuteUserCallbacks(Fix64 deltaTime)
        {
            //using (new SProfiler($"==========ExcuteUserCallbacks========"))
            {
                Parallel3D.ExcuteUserCallbacks(deltaTime);
            }
        }

        public void ExcuteUserFixedUpdate(Fix64 deltaTime)
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
