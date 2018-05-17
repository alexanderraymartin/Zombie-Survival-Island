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
    }

    [Command]
    void CmdSetPlayerModel()
    {
        int id = GameManager.instance.GetNextPlayerColorID();
        playerColorID = id;
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
        else if (Input.GetButtonDown("Interact"))
        {
            Debug.Log("Attempting to pickup...");
            GameObject objHit = GetItemFromRayCast();

            if (objHit == null) {
                return;
            }

            switch (objHit.tag) {
                case "Gun":
                    weaponManager.CmdEquipWeapon(objHit);
                    break;
                case "Gateway":
                    GatewayManager.openGateway(objHit);
                    break;
            }
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
            return objectHit;
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
