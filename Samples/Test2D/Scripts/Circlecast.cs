using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parallel;

namespace Parallel.Sample
{
    public class Circlecast : MonoBehaviour
    {
        FVector2 castPoint;
        FVector2 castNormal;
        FVector2 circleHitPosition;
        PShapecastHit2D hitInfo;

        public float rotateSpeed = 30;
        public float castRange = 10.0f;
        public float circleRadius = 0.5f;
        public float gizmoSize = 0.1f;
        public LayerMask layerMask;

        bool started;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(transform.position, new Vector3(gizmoSize, gizmoSize, gizmoSize));

            if (started)
            {
                Gizmos.DrawLine(transform.position, (Vector2)circleHitPosition);
                Gizmos.DrawSphere((Vector2)circleHitPosition, circleRadius);

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere((Vector2)castPoint, gizmoSize);

                Vector2 direction = (Vector2)castNormal * 1f;
                Gizmos.DrawRay((Vector2)castPoint, direction);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            started = true;
            hitInfo = new PShapecastHit2D();
        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);

            bool hit = false;

            FVector3 start = (FVector3)transform.position;
            FVector3 movement = (FFloat)castRange * (FVector3)transform.right;
            FFloat radius = (FFloat)circleRadius;
            FVector3 end = start + movement;

            hit = Parallel2D.CircleCast((FVector2)start, radius, (FVector2)movement, layerMask, ref hitInfo);

            if (hit)
            {
                castPoint = hitInfo.point;
                castNormal = hitInfo.normal;
                circleHitPosition = (FVector2)(start + hitInfo.fraction * movement);
            }
            else
            {
                castPoint = (FVector2)end;
                castNormal = FVector2.zero;
                circleHitPosition = castPoint;
            }
        }
    }
}
