using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parallel;

namespace Parallel.Sample
{
    public class Force3D : MonoBehaviour, IParallelFixedUpdate
    {
        public float strength = 5;
        FFloat _strength;
        public FFloat hortizontal = FFloat.zero;
        public FFloat vertical = FFloat.zero;

        bool _fire;
        ParallelRigidbody3D _rigidbody;
        ParallelTransform _pTransform;

        public void ParallelFixedUpdate(FFloat deltaTime)
        {
            FVector3 force = new FVector3(
                                    _strength * hortizontal,
                                    FFloat.zero,
                                    _strength * vertical);

            _rigidbody.ApplyForce(force);

        }

        // Start is called before the first frame update
        void Start()
        {
            _pTransform = GetComponent<ParallelTransform>();
            _rigidbody = GetComponent<ParallelRigidbody3D>();
            _strength = (FFloat)strength;
        }

        // Update is called once per frame
        void Update()
        {
            _strength = (FFloat)strength;
            hortizontal = (FFloat)Input.GetAxis("Horizontal");
            vertical = (FFloat)Input.GetAxis("Vertical");
        }
    }
}