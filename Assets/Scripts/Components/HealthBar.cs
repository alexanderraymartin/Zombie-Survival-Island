using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class HealthBar : MonoBehaviour
{
    public GameObject healthBarCanvas;
    public Vector3 position;

    private GameObject instance;
    private Image healthBarImage;
    
    void Start()
    {
        instance = Instantiate(healthBarCanvas);
        instance.transform.SetParent(gameObject.transform);
        instance.transform.localPosition = position;
        healthBarImage = instance.transform.Find("HealthBarImage").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        instance.transform.rotation = Camera.main.transform.rotation;
        healthBarImage.fillAmount = GetComponent<Health>().currentHealth / GetComponent<Health>().maxHealth;
    }
}
