using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour
{
    public GameObject primaryWeapon;
    public GameObject secondaryWeapon;

    public GameObject weaponHolder;

    private bool primarySelected;
    private bool hasPrimaryWeapon;
    private bool hasSecondaryWeapon;

    [Command]
    public void CmdDealDamage(GameObject enemy, float damage)
    {
        enemy.GetComponent<Health>().TakeDamage(damage);
    }

    void EquipWeapon(GameObject weapon)
    {
        //TODO
        CmdCreateWeapon(weapon);
    }

    [Command]
    void CmdCreateWeapon(GameObject weapon)
    {
        GameObject instance = Instantiate(weapon, weaponHolder.transform.position, weaponHolder.transform.rotation);
        NetworkServer.Spawn(instance);
        instance.transform.parent = weaponHolder.transform;
    }

    void CheckForWeapons()
    {
        hasPrimaryWeapon = primaryWeapon != null;
        hasSecondaryWeapon = secondaryWeapon != null;

        if (!hasPrimaryWeapon && hasSecondaryWeapon)
        {
            primaryWeapon = secondaryWeapon;
            primarySelected = true;
            secondaryWeapon = null;
        }
    }

    void ToggleSelectedWeapon()
    {
        primarySelected = !primarySelected;
    }

    // Use this for initialization
    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        primarySelected = true;
        CheckForWeapons();
        //EquipWeapon(primaryWeapon);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        CheckForWeapons();
    }
}
