using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(WeaponManager))]
[RequireComponent(typeof(SoundManager))]
public class Player_Network : NetworkBehaviour
{
    public GameObject firstPersonCharacter;
    public GameObject[] characterModels;
    public int pickupRange;
    public Camera fpsCam;

    [HideInInspector]
    public WeaponManager weaponManager;
    [HideInInspector]
    public SoundManager soundManager;

    [HideInInspector]
    public int connectionId;

    private int playerColorID;
    private bool hasDied;

    void Awake()
    {
        weaponManager = GetComponent<WeaponManager>();
        soundManager = GetComponent<SoundManager>();
    }

    public override void OnStartClient()
    {
        connectionId = GetComponent<NetworkIdentity>().connectionToClient.connectionId;
        Debug.Log(connectionId);
    }

    /*
     * Returns true if the client called the Cmd.
     */
    [Client]
    public bool IsCallingPlayer(int id)
    {
        return connectionId == id;
    }

    public void TakeDamage(int connectionId, float damage)
    {
        CmdTakeDamage(connectionId, damage);
    }

    [Command]
    private void CmdTakeDamage(int connectionId, float damage)
    {
        GetComponent<Health>().TakeDamage(damage);
    }


    [Command]
    public void CmdPlayerDeath()
    {
        RpcPlayerDeath();
    }

    [Command]
    public void CmdRespawn(Vector3 spawnPosition)
    {
        RpcRespawn(spawnPosition);
    }

    [Command]
    public void CmdSetAmmo(GameObject gun, int clipAmmo, int reserveAmmo)
    {
        gun.GetComponent<Gun>().clipAmmo = clipAmmo;
        gun.GetComponent<Gun>().reserveAmmo = reserveAmmo;
    }

    [Command]
    public void CmdReloadGun(GameObject gun)
    {
        RpcReloadGun(gun);
    }

    [Command]
    public void CmdOpenGateway(GameObject gateway)
    {
        RpcOpenGateway(gateway);
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<FirstPersonController>().enabled = true;
        firstPersonCharacter.GetComponent<Camera>().enabled = true;
        firstPersonCharacter.GetComponent<AudioListener>().enabled = true;
        firstPersonCharacter.GetComponent<FlareLayer>().enabled = true;
        CmdSetPlayerModel();
        hasDied = false;
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
        if (!GetComponent<Health>().isAlive)
        {
            return;
        }

        HandleShootingInput();

        // Attempt to reload
        if (Input.GetButtonDown("Reload"))
        {
            CmdReloadGun(weaponManager.GetActiveWeapon());
        }
        // Attempt to cycle through weapons
        if (Input.GetButtonDown("Change Weapon"))
        {
            weaponManager.ChangeWeapon(connectionId);
        }
        // Attempt to pick up a weapon
        else if (Input.GetButtonDown("Interact"))
        {
            Debug.Log("Attempting to pickup...");
            GameObject objHit = GetItemFromRayCast();

            if (objHit == null)
            {
                return;
            }

            switch (objHit.tag)
            {
                case "Gun":
                    GameObject gun = weaponManager.GetActiveWeapon();
                    if (gun != null)
                    {
                        CmdSetAmmo(gun, gun.GetComponent<Gun>().clipAmmo, gun.GetComponent<Gun>().reserveAmmo);
                    }
                    weaponManager.EquipWeapon(connectionId, objHit);
                    break;
                case "Gateway":
                    CmdOpenGateway(objHit);
                    break;
            }
        }
        // Attempt to drop a weapon
        else if (Input.GetButtonDown("Drop Item"))
        {
            GameObject gun = weaponManager.GetActiveWeapon();
            if (gun != null)
            {
                CmdSetAmmo(gun, gun.GetComponent<Gun>().clipAmmo, gun.GetComponent<Gun>().reserveAmmo);
                weaponManager.UnequipWeapon(connectionId);
            }
        }
    }

    void HandleShootingInput()
    {
        // Attempt to use active weapon
        GameObject gun = weaponManager.GetActiveWeapon();
        if (gun == null)
        {
            return;
        }
        // Gun is automatic
        if (gun.GetComponent<Gun>().isAuto)
        {
            if (Input.GetButton("Fire1"))
            {
                weaponManager.GetActiveWeapon().GetComponent<Gun>().Shoot();
            }
        }
        // Gun is semi-automatic
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                weaponManager.GetActiveWeapon().GetComponent<Gun>().Shoot();
            }
        }
    }

    void CheckIfAlive()
    {
        if (!GetComponent<Health>().isAlive && !hasDied)
        {
            CmdPlayerDeath();
        }
    }

    [ClientRpc]
    void RpcPlayerDeath()
    {
        hasDied = true;
    }

    [ClientRpc]
    void RpcRespawn(Vector3 spawnPosition)
    {
        hasDied = false;
        transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
        GetComponent<Health>().Revive();
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
    void RpcReloadGun(GameObject gun)
    {
        if (gun != null && !gun.GetComponent<Gun>().isReloading)
        {
            StartCoroutine(gun.GetComponent<Gun>().Reload());
        }
    }

    [ClientRpc]
    void RpcOpenGateway(GameObject gateway)
    {
        gateway.GetComponent<Gateway>().OpenGateway();
    }
}
