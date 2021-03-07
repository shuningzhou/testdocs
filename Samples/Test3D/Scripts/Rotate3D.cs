using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parallel;

public class Rotate3D : MonoBehaviour, IParallelFixedUpdate
{
    ParallelTransform pTransform;
    ParallelRigidbody3D parallelRigidbody;

    public Fix64Vec3 angularVelocity = new Fix64Vec3(Fix64.zero, Fix64.zero, Fix64.zero);
    public void ParallelFixedUpdate(Fix64 deltaTime)
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
