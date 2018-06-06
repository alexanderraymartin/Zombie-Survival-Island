using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player_Network))]
public class SoundManager : NetworkBehaviour
{
    public AudioClip[] sounds;

    /*************************** Public Functions ***************************/
    public void PlaySound(int index, Vector3 location, float volume)
    {
        PlaySoundHelper(index, location, volume);
        CmdPlaySound(index, location, volume);
    }

    public void PlaySoundLocal(int index, Vector3 location, float volume)
    {
        PlaySoundHelper(index, location, volume);
    }

    /*************************** Cmd Functions ***************************/
    [Command]
    void CmdPlaySound(int index, Vector3 location, float volume)
    {
        RpcPlaySound(index, location, volume);
    }

    /*************************** Rpc Functions ***************************/
    [ClientRpc]
    void RpcPlaySound(int index, Vector3 location, float volume)
    {
        if (isLocalPlayer)
        {
            // Don't run on client who called function
            return;
        }
        PlaySoundHelper(index, location, volume);
    }

    /*************************** Helper Functions ***************************/
    void PlaySoundHelper(int index, Vector3 location, float volume)
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
