using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int ID;
    public GameObject respawn;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Player>().setRespawn(respawn.transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
