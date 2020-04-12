using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPlayerSetup : NetworkBehaviour
{
    public GameObject player;
    private Player ply;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        ply = player.GetComponent<Player>();
        ply.GetComponent<Rigidbody>().isKinematic = !base.hasAuthority;
        if (isLocalPlayer)
        {

            ply.setup(true);
        } else
        {
            ply.setup(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
