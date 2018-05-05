using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance = null;

    [ServerCallback]
    void Awake()
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
        InitGame();
    }
    
    void InitGame()
    {
        Debug.Log("Game initializing");
    }
}
