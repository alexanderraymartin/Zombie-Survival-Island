using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance = null;

    private const int maxPlayers = 6;

    [HideInInspector]
    public List<int> playerColorIDs;

    public int GetNextPlayerColorID()
    {
        // Just to be safe - return first colorID if out of colorIDs
        if(playerColorIDs.Count == 0)
        {
            return 0;
        }
        int id = playerColorIDs[0];
        playerColorIDs.RemoveAt(0);
        return id;
    }

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
        
        InitGame();
    }

    void InitGame()
    {
        Debug.Log("Game initializing");
        SetupPlayerColorIDs();
    }

    void SetupPlayerColorIDs()
    {
        playerColorIDs = new List<int>();

        for (int i = 0; i < maxPlayers; i++)
        {
            playerColorIDs.Add(i);
        }
    }

}
