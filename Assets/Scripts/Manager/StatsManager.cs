using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player_Network))]
public class StatsManager : NetworkBehaviour
{
    [SyncVar]
    public int currency = 0;
    [SyncVar]
    public int killCount = 0;

    /*************************** Init Functions ***************************/
    void Awake()
    {
        Debug.Log("StatsManager initialized.");
    }

    /*************************** Public Functions ***************************/
    public void AddCurrency(int currency)
    {
        CmdAddCurrency(currency);
    }

    [Server]
    public void AddCurrencyServer(int currency)
    {
        this.currency += currency;
    }

    /*************************** Cmd Functions ***************************/
    [Command]
    void CmdAddCurrency(int currency)
    {
        this.currency += currency;
    }

}