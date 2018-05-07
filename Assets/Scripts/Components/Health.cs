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
    public void TakeDamage(float damage)
    {
        if (!isServer)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            isAlive = false;
        }
    }

    [ServerCallback]
    void Start()
    {
        isAlive = true;
        maxHealth = currentHealth;
    }
}
