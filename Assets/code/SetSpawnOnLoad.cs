using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpawnOnLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Transform spawn = GameObject.FindGameObjectWithTag("Spawn").transform;

        GameObject.FindGameObjectWithTag("PlayerBody").GetComponent<Rigidbody>().velocity = Vector3.zero;
        GameObject.FindGameObjectWithTag("PlayerBody").transform.position = spawn.position + Vector3.zero + Vector3.forward * Random.Range(-5f, 5f) + Vector3.left * Random.Range(-5f, 5f);
        //collider.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        GameObject.FindGameObjectWithTag("PlayerBody").GetComponent<Player>().setRespawn(spawn.position + Vector3.zero + Vector3.forward * Random.Range(-5f, 5f) + Vector3.left * Random.Range(-5f, 5f));
        GameObject.FindGameObjectWithTag("PlayerBody").gameObject.GetComponent<Player>().respawn();
        //UIRootObject.transform.rotation = Quaternion.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
