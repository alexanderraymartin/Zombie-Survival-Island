using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
    [SyncVar]
    [HideInInspector]
    public float currentHealth;

    [SyncVar]
    public float healthRegen;

    [SyncVar]
    public float healthRegenTimerPerHit;

    [SyncVar]
    public float maxHealth;

    [SyncVar]
    [HideInInspector]
    public bool isAlive;

    [SyncVar]
    private float healthRegenTimer = 0;

    private float healthRegenTickTimer = 0;

    [HideInInspector]
    public int currencyGainOnKill = 100;

    [HideInInspector]
    public int currencyGainOnHit = 10;

    /*************************** Init Functions ***************************/
    void Start()
    {
        isAlive = true;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (healthRegen > 0 && currentHealth < maxHealth && currentHealth > 0)
        {
            healthRegenTimer = Mathf.Max(healthRegenTimer - Time.deltaTime, 0);

            // Gain health every second
            if (healthRegenTickTimer <= Time.time)
            {
                healthRegenTickTimer = Time.time + 1.0f;

                // Only when you haven't been damaged in a certain amount of time
                if (healthRegenTimer == 0)
                {
                    GainHealth();
                }
            }
        }
    }

    /*************************** Public Functions ***************************/
    public void Revive()
    {
        CmdRevive();
    }

    public void GainHealth()
    {
        CmdGainHealth();
    }

    public void TakeDamage(float damage, GameObject damageDealer)
    {
        CmdTakeDamage(damage, damageDealer);
    }

    /*************************** Cmd Functions ***************************/
    [Command]
    void CmdRevive()
    {
        currentHealth = maxHealth;
        isAlive = true;
    }

    [Command]
    public void CmdGainHealth()
    {
        currentHealth += healthRegen;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        isAlive = currentHealth > 0;
    }

    [Command]
    public void CmdTakeDamage(float damage, GameObject damageDealer)
    {
        currentHealth -= damage;
        isAlive = currentHealth > 0;
        healthRegenTimer += healthRegenTimerPerHit;

        if(damageDealer.transform.tag == "Player")
        {
            if(currentHealth < 0)
            {
                damageDealer.GetComponent<Player_Network>().statsManager.AddCurrency(currencyGainOnKill);
                Debug.Log("Currency: " + damageDealer.GetComponent<Player_Network>().statsManager.currency);

                damageDealer.GetComponent<Player_Network>().statsManager.AddKillCount();
                Debug.Log("Player killCount: " + damageDealer.GetComponent<Player_Network>().statsManager.killCount);
            }
            else
            {
                damageDealer.GetComponent<Player_Network>().statsManager.AddCurrency(currencyGainOnHit);
                Debug.Log("Currency: " + damageDealer.GetComponent<Player_Network>().statsManager.currency);  
            }
        }
    }
}
