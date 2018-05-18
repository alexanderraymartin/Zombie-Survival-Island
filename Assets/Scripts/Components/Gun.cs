using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponGraphics))]
public class Gun : NetworkBehaviour
{
    public float bulletSpeed;
    public float rateOfFire;
    public float damage;
    public float range;
    public float bulletPenetration; // 0 - 100

    [SyncVar]
    public int clipAmmo;
    public int clipMaxAmmo;

    [SyncVar]
    public int reserveAmmo;
    public int maxReserveAmmo;

    [HideInInspector]
    public GameObject cam;
    
    [HideInInspector]
    public Player_Network gunOwner;

    [Client]
    public void Shoot()
    {
        if (gunOwner == null)
        {
            return;
        }

        if (clipAmmo <= 0)
        {
            // Attempt reload here
            // TODO

            // Play reloading sound
            gunOwner.soundManager.CmdPlaySound(1, transform.position, 0.15f);
            return;
        }

        // Play shooting sound
        gunOwner.soundManager.CmdPlaySound(0, transform.position, 0.15f);

        gunOwner.CmdMuzzleFlash();
        RaycastHit[] hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, range);

        // Sort hits
        hits = hits.OrderBy(x => Vector2.Distance(cam.transform.position, x.transform.position)).ToArray();


        for (int i = 0; i < hits.Length; i++)
        {
            gunOwner.CmdHitEffect(hits[i].point, hits[i].normal);
            if (hits[i].transform.gameObject.tag == "Enemy")
            {
                float calculatedDamage = damage * (Mathf.Pow(bulletPenetration / 100, i));
                gunOwner.CmdDealDamage(hits[i].transform.gameObject, calculatedDamage);
                Debug.Log(hits[i].transform.name + "hit for " + calculatedDamage);
                Debug.Log(hits[i].transform.gameObject.GetComponent<Health>().currentHealth);
            }
        }
    }
}
