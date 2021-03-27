using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMove3D : MonoBehaviour
{
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = new Vector3(Time.deltaTime * speed * Input.GetAxis("Horizontal"), 0, Time.deltaTime * speed * Input.GetAxis("Vertical"));
        transform.position += delta;
    }
}
