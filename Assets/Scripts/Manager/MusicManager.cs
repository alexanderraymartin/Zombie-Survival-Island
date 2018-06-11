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

    public void PlayNewRoundMusic(float duration)
    {
        PlayMusic(gameVolume, gameClip, duration, false);
    }

    void Start()
    {
        lobbyManager = GetComponent<LobbyManager>();
        source = GetComponent<AudioSource>();
        isInGame = false;

        PlayMusic(lobbyVolume, lobbyClip, 0.0f, true);
    }

    void Update()
    {
        if (lobbyManager.topPanel.isInGame && !isInGame)
        {
            PlayNewRoundMusic(5.0f);
            isInGame = true;
        }
        else if (!lobbyManager.topPanel.isInGame && isInGame)
        {
            PlayMusic(lobbyVolume, lobbyClip, 0.0f, true);
            isInGame = false;
        }
    }

    void PlayMusic(float volume, AudioClip clip, float duration, bool loop)
    {
        source = GetComponent<AudioSource>();
        source.Stop();
        source.volume = volume;
        source.clip = clip;
        source.loop = loop;
        source.time = 0.0f;
        source.Play();
        if (!loop)
        {
            StartCoroutine(StopMusic(duration));
        }
    }

    IEnumerator StopMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        source.Stop();
    }
}