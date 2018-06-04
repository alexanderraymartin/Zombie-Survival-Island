using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

    // Beach Zombie Spawn Points
    public Transform[] beachZombieSpawns;
    // Town Zombie Spawn Points
    public Transform[] townZombieSpawns;

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

        SpawnEntity(new Vector3(-85, 0, 75), ak47Prefab);
        SpawnEntity(new Vector3(-85, 0, 80), L96SniperRiflePrefab);
        SpawnEntity(new Vector3(-85, 0, 85), M4CarbinePrefab);
        SpawnEntity(new Vector3(-85, 0, 90), PistolPrefab);
        SpawnEntity(new Vector3(-85, 0, 95), ShotgunPrefab);

        SpawnEntity(new Vector3(-90, 0, 75), ak47Prefab);
        SpawnEntity(new Vector3(-90, 0, 80), L96SniperRiflePrefab);
        SpawnEntity(new Vector3(-90, 0, 85), M4CarbinePrefab);
        SpawnEntity(new Vector3(-90, 0, 90), PistolPrefab);
        SpawnEntity(new Vector3(-90, 0, 95), ShotgunPrefab);
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
                players[i].GetComponent<Player_Network>().PlayerRespawn(new Vector3(-325f, 100f, 10.0f));
            }
        }
    }

    IEnumerator SpawnWave()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Player_Network[] playerModels = new Player_Network[players.Length];

        for (var playerIndx = 0; playerIndx < players.Length; playerIndx++) {
            playerModels[playerIndx] = players[playerIndx].GetComponent<Player_Network>();
        }

        yield return new WaitForSeconds(downtime);
        for (int i = 0; i < maxZombies; i++)
        {
            List<Vector3> spawnPoints = GetAvailableEnemySpawnPoints(playerModels);
            int index = Random.Range(0, spawnPoints.Count);
            Vector3 spawnLoc = spawnPoints[index];

            SpawnEntity(spawnLoc, zombiePrefab);
            yield return new WaitForSeconds(betweenSpawnsTime);
        }
    }

    GameObject SpawnEntity(Vector3 position, GameObject prefab)
    {
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        instance.name = instance.name + position;
        NetworkServer.Spawn(instance);
        return instance;
    }

    private List<Vector3> GetAvailableEnemySpawnPoints(Player_Network[] players)
    {
        List<Vector3> spawns = new List<Vector3>();

        foreach (var player in players)
        {
            spawns.AddRange(player.closestSpawnPoints);
        }

        return spawns;
    }
}
