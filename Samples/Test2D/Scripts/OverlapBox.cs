using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parallel;

namespace Parallel.Sample
{
    public class OverlapBox : MonoBehaviour
    {
        public float x = 1f;
        public float y = 1f;
        public float angle = 30;

        public float gizmoSize = 0.1f;
        public LayerMask layerMask;

        PShapeOverlapResult2D result;
        bool _started;
        // Start is called before the first frame update
        void Start()
        {
            _started = true;
            result = new PShapeOverlapResult2D();
        }

        private void OnDrawGizmos()
        {
            if (_started)
            {
                if (result.count > 0)
                {
                    Gizmos.color = Color.magenta;
                    for (int i = 0; i < result.count; i++)
                    {
                        ParallelRigidbody2D rigidBody2D = result.rigidbodies[i] as ParallelRigidbody2D;
                        Gizmos.DrawWireSphere(rigidBody2D.transform.position, gizmoSize);
                    }
                }
            }
        }

        void Update()
        {
            FVector2 center = (FVector2)transform.position;
            FFloat w = (FFloat)x;
            FFloat h = (FFloat)y;
            FFloat a = (FFloat)angle;

            Parallel2D.OverlapBox(center, w, h, a, layerMask, result);
        }
    }
}