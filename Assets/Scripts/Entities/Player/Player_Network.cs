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

    [HideInInspector]
    [SyncVar]
    public NetworkIdentity playerID; // Is this needed?

    private WeaponManager weaponManager;

    public override void OnStartLocalPlayer()
    {
        GetComponent<FirstPersonController>().enabled = true;
        firstPersonCharacter.GetComponent<Camera>().enabled = true;
        firstPersonCharacter.GetComponent<AudioListener>().enabled = true;
        firstPersonCharacter.GetComponent<FlareLayer>().enabled = true;
        playerID = GetComponent<NetworkIdentity>();
        weaponManager = GetComponent<WeaponManager>();
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
        // Attempt to use active weapon
        if (Input.GetButtonDown("Fire1") && weaponManager.GetActiveWeapon() != null)
        {
            Debug.Log("Attempting to shoot...");
            weaponManager.GetActiveWeapon().GetComponent<Gun>().Shoot();
        }
        // Attempt to cycle through weapons
        else if (Input.GetButtonDown("Change Weapon") && weaponManager.weaponHolder.transform.childCount > 1)
        {
            Debug.Log("Attempting to change weapons...");
            weaponManager.CmdEquipWeapon(GameObject.Find("Gun"));
        }
        // Attempt to pick up a weapon
        else if (Input.GetButtonDown("Pickup Item"))
        {
            Debug.Log("Attempting to pickup...");
            weaponManager.CmdEquipWeapon(GameObject.Find("Gun"));
        }
        // Attempt to drop a weapon
        else if (Input.GetButtonDown("Drop Item") && weaponManager.weaponHolder.transform.childCount != 0)
        {
            Debug.Log("Attempting to drop...");
            weaponManager.CmdUnequipWeapon();
        }
    }
}
