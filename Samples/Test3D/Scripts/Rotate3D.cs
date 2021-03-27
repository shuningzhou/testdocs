using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parallel;

public class Rotate3D : MonoBehaviour, IParallelFixedUpdate
{
    ParallelTransform pTransform;
    ParallelRigidbody3D parallelRigidbody;

    public FVector3 angularVelocity = new FVector3(FFloat.zero, FFloat.zero, FFloat.zero);
    public void ParallelFixedUpdate(FFloat deltaTime)
    {
        parallelRigidbody.AngularVelocity = angularVelocity;
    }

    // Start is called before the first frame update
    void Start()
    {
        pTransform = GetComponent<ParallelTransform>();
        parallelRigidbody = GetComponent<ParallelRigidbody3D>();
    }
}
