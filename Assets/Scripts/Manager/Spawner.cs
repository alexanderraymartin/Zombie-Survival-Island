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

    // Player
    public GameObject playerPrefab;
    // Zombie
    public GameObject zombiePrefab;
    // Guns
    public GameObject ak47Prefab;
    public GameObject L96SniperRiflePrefab;
    public GameObject M4CarbinePrefab;
    public GameObject PistolPrefab;
    public GameObject ShotgunPrefab;

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

        SpawnEntity(new Vector3(0, 15, 0), ak47Prefab);
        SpawnEntity(new Vector3(5, 15, 0), L96SniperRiflePrefab);
        SpawnEntity(new Vector3(10, 15, 0), M4CarbinePrefab);
        SpawnEntity(new Vector3(15, 15, 0), PistolPrefab);
        SpawnEntity(new Vector3(20, 15, 0), ShotgunPrefab);

        SpawnEntity(new Vector3(0, 10, 0), ak47Prefab);
        SpawnEntity(new Vector3(5, 10, 0), L96SniperRiflePrefab);
        SpawnEntity(new Vector3(10, 10, 0), M4CarbinePrefab);
        SpawnEntity(new Vector3(15, 10, 0), PistolPrefab);
        SpawnEntity(new Vector3(20, 10, 0), ShotgunPrefab);
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
            SpawnEntity(new Vector3(0, 0, 0), zombiePrefab);
            yield return new WaitForSeconds(betweenSpawnsTime);
        }
    }

    void SpawnEntity(Vector3 position, GameObject prefab)
    {
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        instance.name = instance.name + position;
        NetworkServer.Spawn(instance);
    }
}
