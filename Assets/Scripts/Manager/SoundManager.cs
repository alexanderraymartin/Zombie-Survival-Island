using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SoundManager : NetworkBehaviour
{
    public AudioClip[] sounds;

    [Command]
    public void CmdPlaySound(int index, Vector3 location, float volume)
    {
        RpcPlaySound(index, location, volume);
    }

    [ClientRpc]
    void RpcPlaySound(int index, Vector3 location, float volume)
    {
        PlaySound(index, location, volume);
    }

    void PlaySound(int index, Vector3 location, float volume)
    {
        if (index < sounds.Length)
        {
            AudioSource.PlayClipAtPoint(sounds[index], location, volume);
        }
        else
        {
            Debug.LogError("Sound clip index out of range!");
        }
    }

}
