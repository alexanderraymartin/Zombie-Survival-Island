using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Loader : NetworkBehaviour
{
    public GameObject gameManager;
    public GameObject spawner;

    [ServerCallback]
    void Awake()
    {
        GameObject gameManagerInstance = Instantiate(gameManager);
        NetworkServer.Spawn(gameManagerInstance);

        GameObject spawnerInstance = Instantiate(spawner);
        NetworkServer.Spawn(spawnerInstance);
    }

}
