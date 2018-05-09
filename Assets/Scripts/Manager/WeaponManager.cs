using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player_Network))]
public class WeaponManager : NetworkBehaviour
{
    public GameObject weaponHolder;

    [HideInInspector]
    public GameObject primaryWeapon;
    [HideInInspector]
    public GameObject secondaryWeapon;

    private bool primarySelected;
    private bool hasPrimaryWeapon;
    private bool hasSecondaryWeapon;

    [Command]
    public void CmdEquipWeapon(GameObject gun)
    {
        gun.transform.SetParent(weaponHolder.transform);
        gun.transform.localPosition = new Vector3(0, 0, 0);
        gun.transform.localRotation = Quaternion.identity;
        gun.GetComponent<Rigidbody>().isKinematic = true;

        gun.GetComponent<Gun>().cam = gameObject.GetComponent<Player_Network>().firstPersonCharacter;
        if (gun.GetComponent<Gun>().cam != null)
        {
            Debug.Log("Gun equiped");
        }
        else
        {
            Debug.Log("Gun not found");
        }
    }

    [Command]
    public void CmdUnequipWeapon(GameObject gun)
    {
        gun.transform.SetParent(null);
        gun.GetComponent<Gun>().cam = null;
        gun.GetComponent<Rigidbody>().isKinematic = false;
        if (gun.GetComponent<Gun>().cam == null)
        {
            Debug.Log("Gun unequiped");
        }
        else
        {
            Debug.Log("Gun still equiped");
        }
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
