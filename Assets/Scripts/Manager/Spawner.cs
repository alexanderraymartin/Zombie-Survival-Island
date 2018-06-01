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
                players[i].GetComponent<Player_Network>().PlayerRespawn(new Vector3(-90.0f, 2.0f, 70.0f));
            }
        }
    }

    IEnumerator SpawnWave()
    {
        yield return new WaitForSeconds(downtime);
        for (int i = 0; i < maxZombies; i++)
        {
            SpawnEntity(new Vector3(-40, 0, 70), zombiePrefab);
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
}
