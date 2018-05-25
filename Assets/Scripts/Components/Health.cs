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
        currentHealth = maxHealth;
        isAlive = true;
    }

    [ServerCallback]
    public void GainHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        isAlive = currentHealth > 0;
    }

    [ServerCallback]
    public void TakeDamage(float damage)
    {
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
