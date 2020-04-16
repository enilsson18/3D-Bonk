using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class GameHandler : MonoBehaviour
{
    //0 is standard open style sandbox
    //1 is parkour level map
    //2 is racing map
    public int gameMode = 0;

    private GameObject[] players;
    private GameObject localPlayer;

    private GameObject mainSpawn;
    private List<GameObject> checkpointSpawns;
    private List<GameObject> startSpawns;
    private List<GameObject> spectatorSpawns;

    //sandbox variables


    //level variables


    //race variables
    //0 for startline
    //1 for race
    //2 for finish
    int raceStage;

    GameObject[] participants;

    float timer;


    //setups
    void sandboxSetup(GameObject[] players)
    {

    }

    void levelSetup(GameObject[] players)
    {

    }

    void raceSetup(GameObject[] players)
    {
        raceStage = 0;
        timer = 0;
        participants = players;
    }

    //updates
    void sandboxUpdate(GameObject[] players)
    {

    }

    void levelUpdate(GameObject[] players)
    {

    }

    void raceUpdate(GameObject[] players)
    {

    }

    public GameObject findSpawn(int type)
    {
        GameObject[] spawns = GameObject.FindGameObjectsWithTag("Spawn");

        for (int i = 0; i < spawns.Length; i++)
        {
            if (spawns[i].GetComponent<Spawn>().type == type)
            {
                return spawns[i];
            }
        }

        return null;
    }

    //sees if any of the players have an invalid spawn and then fixes it
    public void checkSpawns(GameObject[] players)
    {
        mainSpawn = findSpawn(0);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponentInChildren<Player>().getSpawn() == null)
            {
                players[i].GetComponentInChildren<Player>().setRespawn(mainSpawn);
                players[i].GetComponentInChildren<Player>().respawn();
                print("setSpawn" + players[i].GetComponentInChildren<Player>().getSpawn().transform.position + mainSpawn.transform.position);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        checkpointSpawns = new List<GameObject>();
        startSpawns = new List<GameObject>();
        spectatorSpawns = new List<GameObject>();

        players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] spawns = GameObject.FindGameObjectsWithTag("Spawn");

        //set local player
        /*
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<NetworkIdentity>().hasAuthority)
            {
                localPlayer = players[i];
                break;
            }
        }*/

        //set list
        for (int i = 0; i < spawns.Length; i++)
        {
            //main spawn
            if (spawns[i].GetComponent<Spawn>().type == 0)
            {
                mainSpawn = spawns[i];
                print("mainspawn set");
            }

            //checkpoint
            if (spawns[i].GetComponent<Spawn>().type == 1)
            {
                checkpointSpawns.Add(spawns[i]);
            }

            //startpoints
            if (spawns[i].GetComponent<Spawn>().type == 2)
            {
                startSpawns.Add(spawns[i]);
            }

            //spectators
            if (spawns[i].GetComponent<Spawn>().type == 3)
            {
                spectatorSpawns.Add(spawns[i]);
            }
        }

        switch (gameMode)
        {
            case 0:
                sandboxSetup(players);
                break;
            case 1:
                levelSetup(players);
                break;
            case 2:
                raceSetup(players);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        checkSpawns(players);

        if (gameMode == 0)
        {
            sandboxUpdate(players);
        }
        else if (gameMode == 1)
        {
            levelUpdate(players);
        }
        else if (gameMode == 2)
        {
            raceUpdate(players);
        }
    }
}
