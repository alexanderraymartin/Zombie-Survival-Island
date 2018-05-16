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
    public GameObject[] characterModels;
    public int pickupRange;

    public WeaponManager weaponManager;
    public Camera fpsCam;

    private int playerColorID;

    [Command]
    public void CmdTakeDamage(float damage)
    {
        GetComponent<Health>().TakeDamage(damage);
    }

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

    [Command]
    public void CmdPlayerDeath()
    {
        PlayerDeath();
        RpcPlayerDeath();
    }

    [Command]
    public void CmdRespawn(Vector3 spawnPosition)
    {
        Respawn(spawnPosition);
        RpcRespawn(spawnPosition);
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<FirstPersonController>().enabled = true;
        firstPersonCharacter.GetComponent<Camera>().enabled = true;
        firstPersonCharacter.GetComponent<AudioListener>().enabled = true;
        firstPersonCharacter.GetComponent<FlareLayer>().enabled = true;
        CmdSetPlayerModel();
    }

    void Start()
    {
        SetPlayerModel();
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        HandleInput();
        CheckIfAlive();
    }

    [Command]
    void CmdSetPlayerModel()
    {
        int id = GameManager.instance.GetNextPlayerColorID();
        playerColorID = id;
        SetPlayerModel();
        RpcSetPlayerModel(id);
    }

    [ClientRpc]
    void RpcSetPlayerModel(int id)
    {
        playerColorID = id;
        SetPlayerModel();
    }

    void SetPlayerModel()
    {
        foreach (GameObject go in characterModels)
        {
            go.SetActive(false);
        }

        characterModels[playerColorID].SetActive(true);
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

    void CheckIfAlive()
    {
        if (!GetComponent<Health>().isAlive)
        {
            CmdPlayerDeath();
        }
    }

    [ClientRpc]
    void RpcPlayerDeath()
    {
        PlayerDeath();
    }

    void PlayerDeath()
    {
        GetComponent<FirstPersonController>().enabled = false;
        gameObject.transform.Find("GFX").gameObject.SetActive(false);
    }

    [ClientRpc]
    void RpcRespawn(Vector3 spawnPosition)
    {
        Respawn(spawnPosition);
    }

    void Respawn(Vector3 spawnPosition)
    {
        GetComponent<Health>().Revive();
        GetComponent<FirstPersonController>().enabled = true;
        gameObject.transform.Find("GFX").gameObject.SetActive(true);
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
