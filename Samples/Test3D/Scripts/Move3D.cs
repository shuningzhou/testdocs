using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parallel;

public class Move3D : MonoBehaviour, IParallelFixedUpdate
{
    ParallelRigidbody3D rigidbody3D;

    public FVector3 linearVelocity;

    public FFloat time;

    public FFloat elapsed;


    public void ParallelFixedUpdate(FFloat deltaTime)
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
        elapsed = FFloat.zero;

        linearVelocity = -linearVelocity;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody3D = GetComponent<ParallelRigidbody3D>();
    }
}
