using UnityEngine;
using Parallel;
using System;
using System.Linq;

namespace Parallel
{
    public class ParallelPhysicsController2D : MonoBehaviour
    {
        public Parallel.LogLevel LoggingLevel;
        public bool autoUpdate = true;
        public bool autoInitialization = true;

        public Fix64 fixedUpdateTime = Fix64.FromDivision(2, 100);
        public int velocityIteration = 4;

        public Fix64Vec2 gravity = new Fix64Vec2(Fix64.zero, Fix64.FromDivision(-98, 10));

        bool _initialized = false;

        public void Initialize()
        {
            if(!_initialized)
            {
                _initialized = true;
                Parallel2D.gravity = gravity;
                Parallel2D.SetLoggingLevel(LoggingLevel);
                Time.fixedDeltaTime = (float)fixedUpdateTime;

                ParallelRigidbody2D[] parallelRigidbody2Ds = FindObjectsOfType<ParallelRigidbody2D>().OrderBy(m => m.sceneIndex).ToArray();

                foreach(ParallelRigidbody2D parallelRigidbody2D in parallelRigidbody2Ds)
                {
                    Debug.Log("Init " + parallelRigidbody2D.name + " index=" + parallelRigidbody2D.sceneIndex);
                   
                    parallelRigidbody2D.Initialize();
                }
            }
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
