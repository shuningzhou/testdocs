using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parallel;

namespace Parallel.Sample
{
    public class Rotate : MonoBehaviour, IParallelFixedUpdate
    {
        ParallelTransform pTransform;
        public float rotateSpeed;

        public void ParallelFixedUpdate(FFloat deltaTime)
        {
            pTransform.RotateInWorldSpace(FVector3.forward * (FFloat)rotateSpeed * deltaTime);
        }

        // Start is called before the first frame update
        void Start()
        {
            pTransform = GetComponent<ParallelTransform>();
        }
    }
}