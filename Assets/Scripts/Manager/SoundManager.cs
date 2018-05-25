using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SoundManager : NetworkBehaviour
{
    public AudioClip[] sounds;

    public void PlaySound(int connectionId, int index, Vector3 location, float volume)
    {
        PlaySoundHelper(connectionId, index, location, volume);
        CmdPlaySound(connectionId, index, location, volume);
    }

    [Command]
    private void CmdPlaySound(int connectionId, int index, Vector3 location, float volume)
    {
        RpcPlaySound(connectionId, index, location, volume);
    }

    [ClientRpc]
    private void RpcPlaySound(int connectionId, int index, Vector3 location, float volume)
    {
        if (GetComponent<Player_Network>().IsCallingPlayer(connectionId))
        {
            // Don't run on client who called function
            return;
        }
        PlaySoundHelper(connectionId, index, location, volume);
    }

    private void PlaySoundHelper(int connectionId, int index, Vector3 location, float volume)
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
