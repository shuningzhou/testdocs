using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parallel;

public class KinematicMove3D : MonoBehaviour, IParallelFixedUpdate
{
    public ParallelTransform parallelTransform;
    public FFloat speed;

    public void ParallelFixedUpdate(FFloat deltaTime)
    {
        FVector3 delta = new FVector3(deltaTime * speed * (FFloat)Input.GetAxis("Horizontal"), FFloat.zero, deltaTime * speed * (FFloat)Input.GetAxis("Vertical"));
        parallelTransform.position += delta;       
    }

    // Start is called before the first frame update
    void Start()
    {
        parallelTransform = GetComponent<ParallelTransform>();
    }
}
