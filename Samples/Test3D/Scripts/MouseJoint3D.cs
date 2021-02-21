using Parallel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseJoint3D : MonoBehaviour
{
    PJoint3D mouseJoint = null;
    public Fix64 distance;
    public Fix64Vec3 jointPosition;
    public Fix64 force = Fix64.FromDivision(100, 1);
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector3)jointPosition, 0.1f);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (mouseJoint == null)
            {
                PRaycastHit3D result;

                bool hit = false;

                Vector3 mousePosition = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);

                Vector3 start = ray.origin;
                Vector3 end = ray.origin + ray.direction * 1000.0f;

                hit = Parallel3D.RayCast((Fix64Vec3)start, (Fix64Vec3)end, out result);

                if (hit)
                {
                    ParallelRigidbody3D rb = (ParallelRigidbody3D)result.rigidbody;
                    mouseJoint = Parallel3D.CreateMouseJoint(rb, result.point, force);
                    distance = result.fraction * (Fix64)1000.0f;
                    jointPosition = result.point;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (mouseJoint != null)
            {
                Parallel3D.DestroyJoint(mouseJoint);
                mouseJoint = null;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (mouseJoint != null)
            {
                Vector3 mousePosition = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);

                Vector3 end = ray.origin + ray.direction * (float)distance;

                jointPosition = (Fix64Vec3)end;
                Parallel3D.MoveMouseJoint(mouseJoint, jointPosition);
            }
        }

    }
}
