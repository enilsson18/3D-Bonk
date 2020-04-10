using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkHandler : NetworkBehaviour
{
    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        if (base.hasAuthority && base.isClient)
        {
            CmdSpawn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command]
    private void CmdSpawn()
    {
        wait(2f);

        GameObject result = Instantiate(Player);

        NetworkServer.Spawn(result, base.connectionToClient);
    }

    private IEnumerator wait(float time)
    {
        WaitForSeconds wait = new WaitForSeconds(time);
        while (true)
        {
            yield return wait;
        }
    }
}
