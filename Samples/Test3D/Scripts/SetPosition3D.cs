using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parallel;

namespace Parallel.Sample
{
    public class SetPosition3D : MonoBehaviour, IParallelFixedUpdate
    {
        public float speed = 2.0f;
        FFloat _speed;
        public FFloat hortizontal = FFloat.zero;
        public FFloat vertical = FFloat.zero;

        ParallelTransform _pTransform;

        public void ParallelFixedUpdate(FFloat deltaTime)
        {
            FVector3 delta = new FVector3(
                                    _speed * deltaTime * hortizontal,
                                    _speed * deltaTime * vertical,
                                    FFloat.zero);

            _pTransform.position += delta;

        }

        // Start is called before the first frame update
        void Start()
        {
            _pTransform = GetComponent<ParallelTransform>();
            _speed = (FFloat)speed;
        }

        // Update is called once per frame
        void Update()
        {
            _speed = (FFloat)speed;
            hortizontal = (FFloat)Input.GetAxis("Horizontal");
            vertical = (FFloat)Input.GetAxis("Vertical");
        }
    }
}