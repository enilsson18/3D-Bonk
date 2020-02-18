using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPlayerSetup : NetworkBehaviour
{
    public GameObject player;
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer)
        {

            player.GetComponent<Player>().setup(cam, false);
        } else
        {
            player.GetComponent<Player>().setup(cam, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
