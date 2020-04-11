using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start : MonoBehaviour
{
    public GameObject respawn;

    // Start is called before the first frame update
    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<Player>().setRespawn(respawn);
        other.GetComponent<Player>().startTimer();
    }

    //void Start()
    //{
    //    
    //}

    // Update is called once per frame
    //void Update()
    //{
    //    
    //}
}
