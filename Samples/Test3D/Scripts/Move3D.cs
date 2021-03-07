using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parallel;

public class Move3D : MonoBehaviour, IParallelFixedUpdate
{
    ParallelRigidbody3D rigidbody3D;

    public Fix64Vec3 linearVelocity;

    public Fix64 time;

    public Fix64 elapsed;


    public void ParallelFixedUpdate(Fix64 deltaTime)
    {
        rigidbody3D.LinearVelocity = linearVelocity;
        elapsed += deltaTime;

        if(elapsed >= time)
        {
            Reverse();
        }
    }

    void Reverse()
    {
        elapsed = Fix64.zero;

        linearVelocity = -linearVelocity;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody3D = GetComponent<ParallelRigidbody3D>();
    }
}
