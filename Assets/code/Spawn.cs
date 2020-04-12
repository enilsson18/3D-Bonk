using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    //0 is main map spawn
    //1 is checkpoint spawn
    //2 is race start spawn
    //3 is spectator spawn
    public int type;
    public float fieldSpawn = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getSpawn()
    {
        if (fieldSpawn != 0)
        {
            return transform.position + Vector3.zero + Vector3.forward * Random.Range(-fieldSpawn, fieldSpawn) + Vector3.left * Random.Range(-fieldSpawn, fieldSpawn);
        }

        return transform.position;
    }
}
