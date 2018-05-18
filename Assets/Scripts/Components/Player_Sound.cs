using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player_Network))]
public class Player_Sound : NetworkBehaviour
{
    [Command]
    public void CmdPlaySound()
    {
        RpcPlaySound();
    }

    [ClientRpc]
    void RpcPlaySound()
    {

    }

    void PlayeSound()
    {

    }

}
