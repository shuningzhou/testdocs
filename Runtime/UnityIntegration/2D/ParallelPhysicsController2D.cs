﻿using UnityEngine;
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

        public Fix64 fixedUpdateTime = Fix64.FromDivision(2, 100);
        public int velocityIteration = 4;
        public bool allowSleep = true;
        public bool warmStart = true;
        public Fix64Vec2 gravity = new Fix64Vec2(Fix64.zero, Fix64.FromDivision(-98, 10));

        bool _initialized = false;

        public void Initialize()
        {
            if(!_initialized)
            {
                _initialized = true;
                Parallel2D.gravity = gravity;
                Parallel2D.allowSleep = allowSleep;
                Parallel2D.warmStart = warmStart;
                Parallel2D.SetLoggingLevel(LoggingLevel);
                Time.fixedDeltaTime = (float)fixedUpdateTime;

                ParallelRigidbody2D[] parallelRigidbody2Ds = FindObjectsOfType<ParallelRigidbody2D>().OrderBy(m => m.sceneIndex).ToArray();

                List<ParallelJoint2D> jointsToInitialize = new List<ParallelJoint2D>();
                foreach(ParallelRigidbody2D parallelRigidbody2D in parallelRigidbody2Ds)
                {
                    Debug.Log("Init " + parallelRigidbody2D.name + " index=" + parallelRigidbody2D.sceneIndex);
                   
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

        public static GameObject InstantiateParallelObject(GameObject original, Fix64Vec3 position, Fix64Quat rotation)
        {
            GameObject go = Instantiate(original, (Vector3)position, (Quaternion)rotation);

            ParallelTransform parallelTransform = go.GetComponent<ParallelTransform>();

            parallelTransform.position = position;
            parallelTransform.rotation = rotation;

            ParallelRigidbody2D[] parallelRigidbody2Ds = go.GetComponentsInChildren<ParallelRigidbody2D>();
            List<ParallelJoint2D> jointsToInitialize = new List<ParallelJoint2D>();

            foreach (ParallelRigidbody2D parallelRigidbody2D in parallelRigidbody2Ds)
            {
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

        public void Step(Fix64 deltaTime)
        {
            Parallel2D.Step(deltaTime, velocityIteration, 1); 
        }

        public void ExcuteUserCallbacks(Fix64 deltaTime)
        {
            Parallel2D.ExcuteUserCallbacks(deltaTime);
        }

        public void ExcuteUserFixedUpdate(Fix64 deltaTime)
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
