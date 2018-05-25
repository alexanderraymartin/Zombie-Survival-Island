using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SoundManager : NetworkBehaviour
{
    public AudioClip[] sounds;

    public void PlaySound(int index, Vector3 location, float volume)
    {
        PlaySoundHelper(index, location, volume);
        CmdPlaySound(index, location, volume);
    }

    [Command]
    private void CmdPlaySound(int index, Vector3 location, float volume)
    {
        RpcPlaySound(index, location, volume);
    }

    [ClientRpc]
    private void RpcPlaySound(int index, Vector3 location, float volume)
    {
        if (isLocalPlayer)
        {
            // Don't run on client who called function
            return;
        }
        PlaySoundHelper(index, location, volume);
    }

    private void PlaySoundHelper(int index, Vector3 location, float volume)
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
