using Parallel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseJoint : MonoBehaviour
{
    PShapeOverlapResult2D result = new PShapeOverlapResult2D();
    PJoint2D mouseJoint = null;
    public Vector3 mousePosition;
    public FVector2 fixedMousePosition;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(mousePosition, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        fixedMousePosition = (FVector2)mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            if (mouseJoint == null)
            {
                bool hit = Parallel2D.OverlapCircle(fixedMousePosition, FFloat.FromDivision(1, 10), result);
                if(hit)
                {
                    ParallelRigidbody2D rb = (ParallelRigidbody2D)result.rigidbodies[0];
                    mouseJoint= Parallel2D.CreateMouseJoint(rb, fixedMousePosition, FFloat.FromDivision(1000, 1));
                }
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            if(mouseJoint != null)
            {
                Parallel2D.DestroyJoint(mouseJoint);
                mouseJoint = null;
            }
        }
        else if(Input.GetMouseButton(0))
        {
            if (mouseJoint != null)
            {
                Parallel2D.MoveMouseJoint(mouseJoint, fixedMousePosition);
            }
        }
    }
}
