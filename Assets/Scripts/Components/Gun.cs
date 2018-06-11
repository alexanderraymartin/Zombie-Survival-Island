using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponGraphics))]
public class Gun : NetworkBehaviour
{
    public GameObject firstPersonCharacter;
    [HideInInspector]
    public int currencyGainOnHit = 10;

    public int reloadingSoundIndex;
    public int shootingSoundIndex;
    public bool isAuto;
    public float rateOfFire;
    public float damage;
    public float range;
    public float bulletPenetration; // 0 - 100
    public float reloadTime;
    public float maxRecoil_x;
    public float recoilSpeed;
    public float recoilDelta;
    public float sidewaysRecoil;

    public Vector3 hipFireLoc;
    public Vector3 sightFireLoc;
    public int aimSpeed;
    private bool isAiming = false;

    [SyncVar]
    public int clipMaxAmmo;
    [SyncVar]
    public int reserveMaxAmmo;

    [HideInInspector]
    [SyncVar]
    public int clipAmmo;

    [HideInInspector]
    [SyncVar]
    public int reserveAmmo;

    [HideInInspector]
    [SyncVar]
    public bool isReloading = false;

    private float nextTimeToFire = 0;

    [HideInInspector]
    public GameObject cam;

    [HideInInspector]
    public Player_Network gunOwner;

    private Camera playerCamera;
    private Transform playerTransform;
    private float recoil = 0.0f;


    Quaternion beforeRecoil;
    Quaternion nextRecoil;

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

        //playerCamera = gunOwner.transform.Find("FirstPersonCharacter").GetComponent<Camera>();
        playerCamera = Camera.main;


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

        // Increment shots fired by 1 bullet
        gunOwner.statsManager.AddShotsFired();
        Debug.Log("Shots Fired: " + gunOwner.statsManager.shotsFired);

        // Play shooting sound
        gunOwner.soundManager.PlaySound(shootingSoundIndex, transform.position, 0.15f);


        // Show muzzle flash
        gunOwner.weaponManager.MuzzleFlash();
        RaycastHit[] hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, range);

        // Sort hits
        hits = hits.OrderBy(x => Vector2.Distance(cam.transform.position, x.transform.position)).ToArray();

        for (int i = 0; i < hits.Length; i++)
        {
            gunOwner.weaponManager.HitEffect(hits[i].point, hits[i].normal);
            if (hits[i].transform.gameObject.tag == "Enemy" && hits[i].transform.gameObject.GetComponent<Health>().isAlive)
            {
                // Count the first hit and not any piercing hits afterwards
                if (i == 0)
                {
                    gunOwner.statsManager.AddShotsHit();
                    Debug.Log("Shots Hit: " + gunOwner.statsManager.shotsHit);
                }

                //75% damge for a body shot
                float headshotMult = 0.75f;
                if (hits[i].collider == (hits[i].transform.GetComponent<Zombie_Network>().headShotBoxCollider))
                {
                    headshotMult = 1;

                    if (i == 0)
                    {
                        gunOwner.statsManager.AddHeadshots();
                        Debug.Log("Player Headshots: " + gunOwner.statsManager.headshots);
                    }
                    
                }
                float calculatedDamage = damage * headshotMult * (Mathf.Pow(bulletPenetration / 100, i));
                gunOwner.weaponManager.DealDamage(hits[i].transform.gameObject, calculatedDamage);

                Debug.Log(hits[i].transform.name + "hit for " + calculatedDamage);
                Debug.Log(hits[i].transform.gameObject.GetComponent<Health>().currentHealth);
            }
        }
        // calculate accuracy
        gunOwner.statsManager.CalculateAccuracy();
        Debug.Log("Player Accuracy: " + gunOwner.statsManager.accuracy);

        if (recoil < 0)
        {
            recoil = recoilDelta;
            beforeRecoil = playerCamera.transform.localRotation;
        }
        else
        {
            recoil += recoilDelta;
        }
        nextRecoil = beforeRecoil * Quaternion.Euler(0, Random.value * sidewaysRecoil - sidewaysRecoil / 2, 0);

        CheckForReload();
    }

    public void Update()
    {
        playerCamera = Camera.main;



        if (recoil > 0)
        {
            if (isAiming)
            {
                var maxRecoil = nextRecoil * Quaternion.Euler(maxRecoil_x, 0, 0);
                playerCamera.transform.localRotation = Quaternion.Slerp(playerCamera.transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed / 14);
            }
            else
            {
                var maxRecoil = nextRecoil * Quaternion.Euler(maxRecoil_x, 0, 0);
                playerCamera.transform.localRotation = Quaternion.Slerp(playerCamera.transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed / 4);
            }

        }
        else if (recoil > -recoilDelta * 3)
        {
            var minRecoil = playerCamera.transform.localRotation;
            playerCamera.transform.localRotation = Quaternion.Slerp(playerCamera.transform.localRotation, beforeRecoil, Time.deltaTime * recoilSpeed / 2);
        }

        recoil -= Time.deltaTime;
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
        gunOwner.soundManager.PlaySoundLocal(reloadingSoundIndex, transform.position, 0.15f);

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
            gunOwner.weaponManager.ReloadWeapon();
            return true;
        }
        return false;
    }

    public void AimDownSights()
    {
        if (!isAiming)
        {
            isAiming = true;
            StartCoroutine(AimDownSightsHelper());
        }
    }

    public void AimHipFire()
    {
        transform.localPosition = hipFireLoc;
        isAiming = false;
    }

    IEnumerator AimDownSightsHelper()
    {
        while (isAiming && transform.localPosition != sightFireLoc)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, sightFireLoc, aimSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
