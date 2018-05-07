using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Health))]
public class Gun : NetworkBehaviour
{
    public float bulletSpeed;
    public float rateOfFire;
    public float damage;
    public float range;
    public float bulletPenetration; // 0 - 1

    public Camera cam;

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit[] hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, range);
        {
            // Sort hits
            hits = hits.OrderBy(x => Vector2.Distance(cam.transform.position, x.transform.position)).ToArray();

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.gameObject.tag == "Enemy")
                {
                    float calculatedDamage = damage * (Mathf.Pow(bulletPenetration, i));
                    CmdDealDamage(hits[i].transform.gameObject, calculatedDamage);
                    Debug.Log(hits[i].transform.name + "hit for " + calculatedDamage);
                    Debug.Log(hits[i].transform.gameObject.GetComponent<Health>().currentHealth);
                }
            }
        }
    }

    [Command]
    public void CmdDealDamage(GameObject enemy, float damage)
    {
        enemy.GetComponent<Health>().TakeDamage(damage);
    }
}
