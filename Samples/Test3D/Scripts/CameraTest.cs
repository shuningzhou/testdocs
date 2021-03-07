using Parallel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraTest : MonoBehaviour
{
    ParallelCamera parallelCamera;
    Camera unityCamera;
    public Image a;
    public Image b;

    ParallelRay parallelRay;
    Ray unityRay;

    public Fix64 range;
    // Start is called before the first frame update
    void Start()
    {
        parallelCamera = GetComponent<ParallelCamera>();
        unityCamera = GetComponent<Camera>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine((Vector3)parallelRay.origin, (Vector3)(parallelRay.direction * range));

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(unityRay.origin, unityRay.direction * (float)range);
    }

    // Update is called once per frame
    void Update()
    {
        a.rectTransform.position = Input.mousePosition;

        Vector3 viewportPoint = unityCamera.ScreenToViewportPoint(Input.mousePosition);
        Fix64Vec3 parallelViewPoint = parallelCamera.GetParallelViewPortPointFromUnityViewPortPoint(viewportPoint);

        Debug.Log("viewportPoint: " + viewportPoint.ToString("F3"));
        Debug.Log("parallelViewPoint: " + parallelViewPoint);

        parallelRay = parallelCamera.ViewportPointToRay(parallelViewPoint);
        unityRay = unityCamera.ViewportPointToRay(viewportPoint);

        //Debug.Log("Parallel Ray: " + parallelRay);
        //Debug.Log("Unity Ray: " + unityRay);
    }
}
