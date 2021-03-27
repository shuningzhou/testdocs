using UnityEngine;
using Parallel;

public class Spawner : MonoBehaviour
{
    public GameObject[] prefabs;
    public bool auto;
    public float interval = 1;
    float _interval;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Spawn();
        }

        if(auto)
        {
            _interval += Time.deltaTime;

            if(_interval > interval)
            {
                _interval = 0;
                Spawn();
            }
        }
    }

    void Spawn()
    {
        int size = prefabs.Length;
        int index = Random.Range(0, size);
        GameObject go = Instantiate(prefabs[index], transform.position, Quaternion.identity);
        ParallelTransform pTransform = go.GetComponent<ParallelTransform>();
        int randomSize = Random.Range(5, 30);
        pTransform.localScale = pTransform.localScale * FFloat.FromDivision(randomSize, 10);
        pTransform.position = (FVector3)transform.position;

        ParallelCollider3D collider = go.GetComponent<ParallelCollider3D>();
        collider.UpdateShape(go);
    }
}
