using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
    [SyncVar]
    public float currentHealth;

    [SyncVar]
    [HideInInspector]
    public float maxHealth;

    [SyncVar]
    [HideInInspector]
    public bool isAlive;

    [ServerCallback]
    public void Revive()
    {
        if (!isServer)
        {
            return;
        }

        currentHealth = maxHealth;
        isAlive = true;
    }

    [ServerCallback]
    public void GainHealth(float amount)
    {
        if (!isServer)
        {
            return;
        }

        currentHealth += amount;

        isAlive = currentHealth > 0;
    }

    [ServerCallback]
    public void TakeDamage(float damage)
    {
        if (!isServer)
        {
            return;
        }

        currentHealth -= damage;

        isAlive = currentHealth > 0;
    }

    [ServerCallback]
    void Start()
    {
        isAlive = true;
        maxHealth = currentHealth;
    }
}
