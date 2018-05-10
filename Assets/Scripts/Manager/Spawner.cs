using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spawner : NetworkBehaviour
{
    public static Spawner instance = null;

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

        SpawnZombie(new Vector3(0, 0, 0));
        SpawnZombie(new Vector3(5, 0, 10));
        SpawnZombie(new Vector3(15, 0, 0));

        SpawnGun(new Vector3(0, 0, 0));
        SpawnGun(new Vector3(5, 0, 0));
        SpawnGun(new Vector3(10, 0, 0));
        SpawnGun(new Vector3(15, 0, 0));
        SpawnGun(new Vector3(20, 0, 0));
        SpawnGun(new Vector3(25, 0, 0));
    }

    void Update()
    {

    }

    void SpawnZombie(Vector3 position)
    {
        GameObject instance = Instantiate(zombiePrefab, position, Quaternion.identity);
        NetworkServer.Spawn(instance);
    }

    void SpawnGun(Vector3 position)
    {
        GameObject instance = Instantiate(gunPrefab, position, Quaternion.identity);
        NetworkServer.Spawn(instance);
    }
}
