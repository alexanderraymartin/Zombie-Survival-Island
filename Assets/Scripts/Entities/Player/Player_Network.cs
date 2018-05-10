using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(WeaponManager))]
public class Player_Network : NetworkBehaviour
{
    public GameObject firstPersonCharacter;
    public GameObject[] characterModel;
    public int pickupRange;

    [HideInInspector]
    [SyncVar]
    public NetworkIdentity playerID; // Is this needed?

    private WeaponManager weaponManager;
    private Camera fpsCam;

    [Command]
    public void CmdDealDamage(GameObject enemy, float damage)
    {
        enemy.GetComponent<Health>().TakeDamage(damage);
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<FirstPersonController>().enabled = true;
        firstPersonCharacter.GetComponent<Camera>().enabled = true;
        firstPersonCharacter.GetComponent<AudioListener>().enabled = true;
        firstPersonCharacter.GetComponent<FlareLayer>().enabled = true;
        playerID = GetComponent<NetworkIdentity>();
        weaponManager = GetComponent<WeaponManager>();
        fpsCam = firstPersonCharacter.GetComponent<Camera>();

        foreach (GameObject go in characterModel)
        {
            go.SetActive(false);
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        HandleInput();
    }

    void HandleInput()
    {
        // Attempt to use active weapon
        if (Input.GetButtonDown("Fire1") && weaponManager.GetActiveWeapon() != null)
        {
            Debug.Log("Attempting to shoot...");
            weaponManager.GetActiveWeapon().GetComponent<Gun>().Shoot();
        }
        // Attempt to cycle through weapons
        else if (Input.GetButtonDown("Change Weapon"))
        {
            Debug.Log("Attempting to change weapons...");
            weaponManager.CmdChangeWeapons();
        }
        // Attempt to pick up a weapon
        else if (Input.GetButtonDown("Pickup Item"))
        {
            Debug.Log("Attempting to pickup...");
            weaponManager.CmdEquipWeapon(GetItemFromRayCast());
        }
        // Attempt to drop a weapon
        else if (Input.GetButtonDown("Drop Item"))
        {
            Debug.Log("Attempting to drop...");
            weaponManager.CmdUnequipWeapon();
        }
    }

    GameObject GetItemFromRayCast()
    {
        RaycastHit hit;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, pickupRange))
        {
            GameObject objectHit = hit.transform.gameObject;

            if (objectHit.GetComponent<Gun>() != null && objectHit.GetComponent<Gun>().gunOwner == null)
            {
                return objectHit;
            }

        }

        return null;
    }
}
