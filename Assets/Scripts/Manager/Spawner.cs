using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spawner : NetworkBehaviour
{
    public int maxZombies = 0;
    public int downtime = 5;
    public int betweenSpawnsTime = 3;
    [SyncVar]
    public int zombiesAlive = 0;
    [SyncVar]
    public int wave = 0;

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
    void Update()
    {
        if (zombiesAlive == 0)
        {
            wave++;
            SpawnNewWave();
        }
    }

    [ServerCallback]
    void SpawnNewWave()
    {
        maxZombies += (int)Mathf.Log(wave * 10, 2f);
        zombiesAlive = maxZombies;
        Debug.Log("Starting Wave: " + wave);
        Debug.Log("Max Zombies: " + maxZombies);
        RespawnPlayers();
        StartCoroutine(SpawnWave());
    }

    [ServerCallback]
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

    [ServerCallback]
    IEnumerator SpawnWave()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Player_Network[] playerModels = new Player_Network[players.Length];

        for (var playerIndex = 0; playerIndex < players.Length; playerIndex++)
        {
            playerModels[playerIndex] = players[playerIndex].GetComponent<Player_Network>();
        }

        GameObject.Find("LobbyManager").GetComponent<MusicManager>().PlayNewRoundMusic(downtime);
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

    [ServerCallback]
    GameObject SpawnEntity(Vector3 position, GameObject prefab)
    {
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        instance.name = instance.name + position;
        NetworkServer.Spawn(instance);
        return instance;
    }

    [ServerCallback]
    List<Vector3> GetAvailableEnemySpawnPoints(Player_Network[] players)
    {
        List<Vector3> spawns = new List<Vector3>();

        foreach (var player in players)
        {
            spawns.AddRange(player.closestSpawnPoints);
        }

        return spawns;
    }
}
