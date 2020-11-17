using Parallel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour
{
    ParallelCamera parallelCamera;
    Camera unityCamera;

    // Start is called before the first frame update
    void Start()
    {
        parallelCamera = GetComponent<ParallelCamera>();
        unityCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 viewportPoint = unityCamera.ScreenToViewportPoint(Input.mousePosition);
        parallelCamera.ViewportToRay((Fix64Vec3)viewportPoint);

    }
}
