using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public WeaponManager weaponManager;

    public float bulletSpeed;
    public float rateOfFire;
    public float damage;
    public float range;
    public float bulletPenetration; // 0 - 100
    
    public GameObject firePoint;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Debug.Log("Shooting!");
        RaycastHit[] hits = Physics.RaycastAll(firePoint.transform.position, firePoint.transform.forward, range);
        {
            // Sort hits
            hits = hits.OrderBy(x => Vector2.Distance(firePoint.transform.position, x.transform.position)).ToArray();

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.gameObject.tag == "Enemy")
                {
                    float calculatedDamage = damage * (Mathf.Pow(bulletPenetration / 100, i));
                    weaponManager.CmdDealDamage(hits[i].transform.gameObject, calculatedDamage);
                    Debug.Log(hits[i].transform.name + "hit for " + calculatedDamage);
                    Debug.Log(hits[i].transform.gameObject.GetComponent<Health>().currentHealth);
                }
            }
        }
    }
}
