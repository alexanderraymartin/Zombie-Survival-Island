using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player_Network))]
public class WeaponManager : NetworkBehaviour
{
    public GameObject primaryWeapon;
    public GameObject secondaryWeapon;

    public GameObject weaponHolder;

    private bool primarySelected;
    private bool hasPrimaryWeapon;
    private bool hasSecondaryWeapon;

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
