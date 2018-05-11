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

    public WeaponManager weaponManager;
    public Camera fpsCam;

    [Command]
    public void CmdDealDamage(GameObject enemy, float damage)
    {
        enemy.GetComponent<Health>().TakeDamage(damage);
    }


    [Command]
    public void CmdMuzzleFlash()
    {
        RpcMuzzleFlash();
    }

    [Command]
    public void CmdHitEffect(Vector3 position, Vector3 normal)
    {
        RpcHitEffect(position, normal);
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<FirstPersonController>().enabled = true;
        firstPersonCharacter.GetComponent<Camera>().enabled = true;
        firstPersonCharacter.GetComponent<AudioListener>().enabled = true;
        firstPersonCharacter.GetComponent<FlareLayer>().enabled = true;

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

    [ClientRpc]
    void RpcMuzzleFlash()
    {
        weaponManager.GetActiveWeapon().GetComponent<Gun>().gameObject.GetComponent<WeaponGraphics>().muzzleFlash.Play();
    }

    [ClientRpc]
    void RpcHitEffect(Vector3 position, Vector3 normal)
    {
        // Replace with object pooling
        GameObject instance = Instantiate(weaponManager.GetActiveWeapon().GetComponent<Gun>().gameObject.GetComponent<WeaponGraphics>().hitEffectPrefab, position, Quaternion.LookRotation(normal));
        Destroy(instance, 2f);
    }
}
