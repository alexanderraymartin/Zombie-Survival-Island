using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class HealthBar : MonoBehaviour
{
    public Image healthBar;

    void Update()
    {
        Vector3 healthPosition = Camera.main.WorldToScreenPoint(this.transform.position);
        healthBar.transform.position = healthPosition;
        healthBar.fillAmount = GetComponent<Health>().currentHealth / GetComponent<Health>().maxHealth;
    }
}
