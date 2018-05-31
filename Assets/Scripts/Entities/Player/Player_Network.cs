﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(WeaponManager))]
[RequireComponent(typeof(SoundManager))]
[RequireComponent(typeof(Player_Ability))]
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
    public Player_Ability playerAbility;

    private int playerColorID;
    private bool hasDied;


    /*************************** Init Functions ***************************/
    void Awake()
    {
        weaponManager = GetComponent<WeaponManager>();
        soundManager = GetComponent<SoundManager>();

        playerAbility = GetComponent<Player_Ability>();
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

    /*************************** Public Functions ***************************/
    public void TakeDamage(int connectionId, float damage)
    {
        CmdTakeDamage(connectionId, damage);
    }

    public void PlayerDeath()
    {
        CmdPlayerDeath();
    }

    public void PlayerRespawn(Vector3 spawnPosition)
    {
        CmdPlayerRespawn(spawnPosition);
    }

    public void OpenGateway(GameObject gateway)
    {
        CmdOpenGateway(gateway);
    }

    /*************************** Cmd Functions ***************************/
    [Command]
    void CmdTakeDamage(int connectionId, float damage)
    {
        GetComponent<Health>().TakeDamage(damage);
    }

    [Command]
    void CmdPlayerDeath()
    {
        RpcPlayerDeath();
    }

    [Command]
    void CmdPlayerRespawn(Vector3 spawnPosition)
    {
        RpcPlayerRespawn(spawnPosition);
    }

    [Command]
    void CmdOpenGateway(GameObject gateway)
    {
        RpcOpenGateway(gateway);
    }

    [Command]
    void CmdSetPlayerModel()
    {
        int id = GameManager.instance.GetNextPlayerColorID();
        playerColorID = id;
        RpcSetPlayerModel(id);
    }

    /*************************** Rpc Functions ***************************/
    [ClientRpc]
    void RpcPlayerDeath()
    {
        hasDied = true;
    }

    [ClientRpc]
    void RpcPlayerRespawn(Vector3 spawnPosition)
    {
        PlayerRespawnHelper(spawnPosition);
    }

    [ClientRpc]
    void RpcOpenGateway(GameObject gateway)
    {
        OpenGatewayHelper(gateway);
    }

    [ClientRpc]
    void RpcSetPlayerModel(int id)
    {
        playerColorID = id;
        SetPlayerModel();
    }

    /*************************** Helper Functions ***************************/
    void OpenGatewayHelper(GameObject gateway)
    {
        gateway.GetComponent<Gateway>().OpenGateway();
    }

    void SetPlayerModel()
    {
        foreach (GameObject go in characterModels)
        {
            go.SetActive(false);
        }

        characterModels[playerColorID].SetActive(true);
    }

    void PlayerRespawnHelper(Vector3 spawnPosition)
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

    void CheckIfAlive()
    {
        if (!GetComponent<Health>().isAlive && !hasDied)
        {
            CmdPlayerDeath();
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
            weaponManager.ReloadWeapon();
        }
        // Attempt to cycle through weapons
        if (Input.GetButtonDown("Change Weapon"))
        {
            weaponManager.ChangeWeapon();
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
                        weaponManager.SetAmmo(gun, gun.GetComponent<Gun>().clipAmmo, gun.GetComponent<Gun>().reserveAmmo);
                    }
                    weaponManager.EquipWeapon(objHit);
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
                weaponManager.SetAmmo(gun, gun.GetComponent<Gun>().clipAmmo, gun.GetComponent<Gun>().reserveAmmo);
                weaponManager.UnequipWeapon();
            }
        }

        else if (Input.GetButtonDown("Fire2"))
        {
            playerAbility.Teleport();
            // Read input
            //float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            //float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            //Vector2 direction = new Vector2(horizontal, vertical);

            //// normalize input if it exceeds 1 in combined length:
            //if (direction.sqrMagnitude > 1)
            //{
            //    direction.Normalize();
            //}

            //Vector3 desiredMove = transform.forward * direction.y + transform.right * direction.x;

            //characterController.Move(desiredMove * 5);
        }
    }
}
