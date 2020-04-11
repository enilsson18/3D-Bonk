using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpawnOnLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject spawn = GameObject.FindGameObjectWithTag("Spawn");

        //GameObject.FindGameObjectWithTag("PlayerBody").transform.position = spawn.position + Vector3.zero + Vector3.forward * Random.Range(-5f, 5f) + Vector3.left * Random.Range(-5f, 5f);
        //collider.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        GameObject.FindGameObjectWithTag("PlayerBody").GetComponent<Player>().setRespawn(spawn);
        //UIRootObject.transform.rotation = Quaternion.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
