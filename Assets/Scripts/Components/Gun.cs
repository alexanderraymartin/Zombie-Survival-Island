using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponGraphics))]
public class Gun : NetworkBehaviour
{
    public int reloadingSoundIndex;
    public int shootingSoundIndex;

    [SyncVar]
    public float rateOfFire;
    public bool isAuto;

    [SyncVar]
    public float damage;
    [SyncVar]
    public float range;
    [SyncVar]
    public float bulletPenetration; // 0 - 100

    [SyncVar]
    public float reloadTime;

    [SyncVar]
    public int clipMaxAmmo;
    [SyncVar]
    public int reserveMaxAmmo;

    [SyncVar]
    private int clipAmmo;
    [SyncVar]
    private int reserveAmmo;

    [HideInInspector]
    [SyncVar]
    public bool isReloading = false;

    private float nextTimeToFire = 0;

    [HideInInspector]
    public GameObject cam;

    [HideInInspector]
    public Player_Network gunOwner;


    [ServerCallback]
    void Awake()
    {
        clipAmmo = clipMaxAmmo;
        reserveAmmo = reserveMaxAmmo;
    }

    void OnEnable()
    {
        isReloading = false;
    }

    [Client]
    public void Shoot()
    {
        if (gunOwner == null || isReloading)
        {
            return;
        }

        // Limit rate of fire
        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1.0f / rateOfFire;
        }
        else
        {
            return;
        }

        if (CheckForReload())
        {
            return;
        }

        // Decrease clip ammo by 1 bullet
        clipAmmo -= 1;

        // Play shooting sound
        gunOwner.soundManager.CmdPlaySound(shootingSoundIndex, transform.position, 0.15f);

        // Show muzzle flash
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

        CheckForReload();
    }

    public IEnumerator Reload()
    {
        // If no ammo
        if (reserveAmmo <= 0)
        {
            Debug.Log("Out of ammo");
            yield break;
        }

        // If full ammo
        if (clipAmmo == clipMaxAmmo)
        {
            Debug.Log("Already full ammo");
            yield break;
        }

        isReloading = true;
        // Play reloading sound
        gunOwner.soundManager.PlaySound(reloadingSoundIndex, transform.position, 0.15f);

        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        int oldAmmoCount = clipAmmo;

        clipAmmo = Mathf.Min(clipMaxAmmo, reserveAmmo + oldAmmoCount);
        reserveAmmo = Mathf.Max(reserveAmmo + oldAmmoCount - clipMaxAmmo, 0);
        isReloading = false;
    }

    bool CheckForReload()
    {
        // Out of clip ammo
        if (clipAmmo <= 0)
        {
            // Attempt reload here
            gunOwner.CmdReloadGun(gameObject);
            return true;
        }
        return false;
    }
}
