using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Loader : NetworkBehaviour
{
    public GameObject gameManager;

    [ServerCallback]
    void Awake()
    {
        Instantiate(gameManager);
    }

}
