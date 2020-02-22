using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPlayerSetup : NetworkBehaviour
{
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (!isLocalPlayer)
        {

            player.GetComponent<Player>().setup(false);
        } else
        {
            player.GetComponent<Player>().setup(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
