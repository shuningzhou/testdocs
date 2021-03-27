using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parallel;

namespace Parallel.Sample
{
    public class ShapeCast : MonoBehaviour
    {
        FVector3 castPoint;
        FVector3 castNormal;
        FVector3 sphereHitPosition;
        PShapecastHit3D hitInfo;

        public float rotateSpeed = 30;
        public float castRange = 10.0f;
        public float gizmoSize = 0.1f;
        public float normalLength = 3f;
        public LayerMask layerMask;

        public ParallelConvexCollider3D collider;
        public PShape3D shape;

        bool started;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(transform.position, new Vector3(gizmoSize, gizmoSize, gizmoSize));

            if (started)
            {
                Gizmos.DrawLine(transform.position, (Vector3)sphereHitPosition);
                //Gizmos.DrawSphere((Vector3)sphereHitPosition, sphereRadius);

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere((Vector3)castPoint, gizmoSize);

                Vector3 direction = (Vector3)castNormal * normalLength;
                Gizmos.DrawRay((Vector3)castPoint, direction);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            started = true;
            hitInfo = new PShapecastHit3D();
            shape = collider.CreateShape(gameObject);
        }

        private void OnDestroy()
        {
            Parallel3D.DestroyShape(shape);
        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

            bool hit = false;

            FVector3 pos = (FVector3)transform.position;
            FQuaternion rot = (FQuaternion)transform.rotation;

            FVector3 movement = (FFloat)castRange * (FVector3)transform.forward;
            FVector3 end = pos + movement;

            hit = Parallel3D.ShapeCast(shape, pos, rot, movement, layerMask, ref hitInfo, null);

            if (hit)
            {
                castPoint = hitInfo.point;
                castNormal = hitInfo.normal;
                sphereHitPosition = pos + hitInfo.fraction * movement;
            }
            else
            {
                castPoint = end;
                castNormal = FVector3.zero;
                sphereHitPosition = end;
            }
        }
    }
}
