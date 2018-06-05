using Prototype.NetworkLobby;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip lobbyClip;
    public AudioClip gameClip;

    public float lobbyVolume;
    public float gameVolume;

    private AudioSource source;
    private LobbyManager lobbyManager;

    private bool isInGame;
    // Use this for initialization
    void Start()
    {
        lobbyManager = GetComponent<LobbyManager>();
        source = GetComponent<AudioSource>();
        source.volume = lobbyVolume;
        source.clip = lobbyClip;
        source.loop = true;
        source.Play();
        isInGame = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (lobbyManager.topPanel.isInGame && !isInGame)
        {
            source.Stop();
            source.volume = gameVolume;
            source.clip = gameClip;
            source.Play();
            isInGame = true;
        }
        else if (!lobbyManager.topPanel.isInGame && isInGame)
        {
            source.Stop();
            source.volume = lobbyVolume;
            source.clip = lobbyClip;
            source.Play();
            isInGame = false;
        }
    }
}