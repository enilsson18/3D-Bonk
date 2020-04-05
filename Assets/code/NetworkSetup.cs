using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkSetup : MonoBehaviour
{
    private string ip;

    // Start is called before the first frame update
    void Start()
    {
        ip = IPManager.GetIP(ADDRESSFAM.IPv4);
        gameObject.GetComponent<NetworkManager>().networkAddress = ip;

        print(ip);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
