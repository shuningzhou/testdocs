using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parallel;

namespace Parallel.Sample
{
    public class Impulse : MonoBehaviour, IParallelFixedUpdate
    {
        public float strength;
        FFloat _strength;
        FVector2 _mousePos;
        bool _fire;
        ParallelRigidbody2D _rigidbody;
        ParallelTransform _transform;
        public GameObject cursor;
        Transform _cursorTransform;

        public void ParallelFixedUpdate(FFloat deltaTime)
        {
            if (_fire)
            {
                _fire = false;

                FVector2 direction = _mousePos - (FVector2)_transform.position;
                direction = direction.normalized;
                FVector2 impulse = direction * _strength;
                _rigidbody.ApplyLinearImpulse(impulse);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            _rigidbody = GetComponent<ParallelRigidbody2D>();
            _transform = GetComponent<ParallelTransform>();
            _cursorTransform = Instantiate(cursor, Vector3.zero, Quaternion.identity).transform;
        }

        // Update is called once per frame
        void Update()
        {
            _strength = (FFloat)strength;

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                                                                    Input.mousePosition.y,
                                                                    -Camera.main.transform.position.z));
                Vector3 newPos = new Vector3(mouse.x, mouse.y, 0);
                _cursorTransform.position = newPos;
                _mousePos = (FVector2)newPos;
                _fire = true;
            }
        }
    }
}