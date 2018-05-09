using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Gun : NetworkBehaviour
{
    public float bulletSpeed;
    public float rateOfFire;
    public float damage;
    public float range;
    public float bulletPenetration; // 0 - 100

    [HideInInspector]
    public GameObject cam;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && cam != null)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit[] hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, range);
        Debug.DrawRay(cam.transform.position, cam.transform.forward * 25, Color.red, 1); // TODO remove later
        {
            // Sort hits
            hits = hits.OrderBy(x => Vector2.Distance(cam.transform.position, x.transform.position)).ToArray();

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.gameObject.tag == "Enemy")
                {
                    float calculatedDamage = damage * (Mathf.Pow(bulletPenetration / 100, i));
                    CmdDealDamage(hits[i].transform.gameObject, calculatedDamage);
                    Debug.Log(hits[i].transform.name + "hit for " + calculatedDamage);
                    Debug.Log(hits[i].transform.gameObject.GetComponent<Health>().currentHealth);
                }
            }
        }
    }

    [Command]
    void CmdDealDamage(GameObject enemy, float damage)
    {
        enemy.GetComponent<Health>().TakeDamage(damage);
    }
}
