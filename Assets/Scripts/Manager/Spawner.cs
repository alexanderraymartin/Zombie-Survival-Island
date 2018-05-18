using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spawner : NetworkBehaviour
{
    public static Spawner instance = null;

    public int wave = 0;
    public int maxZombies = 0;
    public int downtime = 5;
    public int betweenSpawnsTime = 3;
    public int zombiesAlive = 0;

    public GameObject playerPrefab;
    public GameObject zombiePrefab;
    public GameObject gunPrefab;

    [ServerCallback]
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        SpawnGun(new Vector3(0, 10, 0));
        SpawnGun(new Vector3(5, 10, 0));
        SpawnGun(new Vector3(10, 10, 0));
        SpawnGun(new Vector3(15, 10, 0));
        SpawnGun(new Vector3(20, 10, 0));
        SpawnGun(new Vector3(25, 10, 0));
    }

    [ServerCallback]
    void Update()
    {
        if (zombiesAlive == 0)
        {
            incrementWave();
        }
    }

    [ServerCallback]
    void incrementWave()
    {
        wave = wave + 1;
        maxZombies += (int)Mathf.Log(wave * 10, 2f);
        zombiesAlive = maxZombies;
        Debug.Log("Starting Wave: " + wave);
        Debug.Log("Max Zombies: " + maxZombies);
        RespawnPlayers();
        StartCoroutine(SpawnWave());
    }

    void RespawnPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            if (!players[i].GetComponent<Health>().isAlive)
            {
                Debug.Log("Respawning player: " + players[i].name);
                // TODO: add respawn points
                players[i].GetComponent<Player_Network>().CmdRespawn(new Vector3(10.0f, 10.0f, 10.0f));
            }
        }
    }

    IEnumerator SpawnWave()
    {
        yield return new WaitForSeconds(downtime);
        for (int i = 0; i < maxZombies; i++)
        {
            SpawnZombie(new Vector3(0, 0, 0));
            yield return new WaitForSeconds(betweenSpawnsTime);
        }
    }

    void SpawnZombie(Vector3 position)
    {
        GameObject instance = Instantiate(zombiePrefab, position, Quaternion.identity);
        instance.name = instance.name + position;
        NetworkServer.Spawn(instance);
    }

    void SpawnGun(Vector3 position)
    {
        GameObject instance = Instantiate(gunPrefab, position, Quaternion.identity);
        instance.name = instance.name + position;
        NetworkServer.Spawn(instance);
    }
}
