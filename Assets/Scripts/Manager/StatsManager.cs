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
    [SyncVar]
    public int headshots = 0;
    [SyncVar]
    public float shotsFired = 0;
    [SyncVar]
    public float shotsHit = 0;
    [SyncVar]
    public float accuracy = 0;

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

    public void AddKillCount()
    {
        CmdAddKillCount();
    }

    public void AddHeadshots()
    {
        CmdAddHeadshots();
    }

    public void AddShotsFired()
    {
        CmdAddShotsFired();
    }

    public void AddShotsHit()
    {
        CmdAddShotsHit();
    }

    public void CalculateAccuracy()
    {
        CmdCalculateAccuracy();
    }

    /*************************** Cmd Functions ***************************/
    [Command]
    void CmdAddCurrency(int currency)
    {
        this.currency += currency;
    }

    [Command]
    public void CmdAddKillCount()
    {
        this.killCount += 1;
    }

    [Command]
    public void CmdAddHeadshots()
    {
        this.headshots += 1;
    }

    [Command]
    public void CmdAddShotsFired()
    {
        this.shotsFired += 1;
    }

    [Command]
    public void CmdAddShotsHit()
    {
        this.shotsHit += 1;
    }

    [Command]
    public void CmdCalculateAccuracy()
    {
        this.accuracy = this.shotsHit / this.shotsFired;
    }

}