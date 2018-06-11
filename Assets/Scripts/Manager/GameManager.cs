using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    private const int maxPlayers = 6;

    private bool isGameOver = false;

    [HideInInspector]
    public List<int> playerColorIDs;

    public int GetNextPlayerColorID()
    {
        // Just to be safe - return first colorID if out of colorIDs
        if (playerColorIDs.Count == 0)
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
        InitGame();
    }

    [ServerCallback]
    void InitGame()
    {
        Debug.Log("Game initializing");
        SetupPlayerColorIDs();
    }

    [ServerCallback]
    void SetupPlayerColorIDs()
    {
        playerColorIDs = new List<int>();

        for (int i = 0; i < maxPlayers; i++)
        {
            playerColorIDs.Add(i);
        }
    }

    void Update()
    {
        if (isGameOver) {
            return; 
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            if (!player.GetComponent<Player_Network>().hasDied)
            {
                return;
            }
        }

        isGameOver = true;

        StartCoroutine(EndGame());
        
    }

    private IEnumerator EndGame()
    {
        GameObject imageGameObject = GameObject.FindGameObjectWithTag("Fade");
        Image image = imageGameObject.GetComponent<Image>();
        Animator animator = image.GetComponent<Animator>();
        animator.gameObject.SetActive(true);
        animator.SetBool("fade", true);
        yield return new WaitUntil(() => image.color.a == 1);
        SceneManager.LoadScene("EndGame");
        
    }

}
