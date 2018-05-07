using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class HealthBar : MonoBehaviour
{
    public GameObject healthBarPrefab;

    private GameObject UI;
    private GameObject healthBar;
    private Health health;

    void Start()
    {
        UI = GameObject.FindGameObjectWithTag("UI");
        health = GetComponent<Health>();
        healthBar = Instantiate(healthBarPrefab);
        healthBar.transform.SetParent(UI.transform, false);
    }

    void Update()
    {
        if (!health.isAlive)
        {
            Destroy(gameObject);
        }

        healthBar.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
        //healthBar.fillAmount = GetComponent<Health>().currentHealth / GetComponent<Health>().maxHealth;
    }
}
