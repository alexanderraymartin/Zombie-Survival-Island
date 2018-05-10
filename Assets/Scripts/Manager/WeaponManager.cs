using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player_Network))]
[RequireComponent(typeof(NetworkTransform))]
public class WeaponManager : NetworkBehaviour
{
    public GameObject weaponHolder;
    [SyncVar]
    public int maxWeapons = 2;

    [SyncVar]
    private int currentWeaponIndex = 0;
    [SyncVar]
    private int weaponCount = 0;

    private NetworkTransform networkTransform;

    public GameObject GetActiveWeapon()
    {
        if(weaponCount != 0)
        {
            return weaponHolder.transform.GetChild(currentWeaponIndex).gameObject;
        }
        return null;
    }

    [Command]
    public void CmdEquipWeapon(GameObject gun)
    {
        EquipWeapon(gun.GetComponent<NetworkIdentity>().netId);
        RpcEquipWeapon(gun.GetComponent<NetworkIdentity>().netId);
    }

    [Command]
    public void CmdUnequipWeapon(GameObject gun)
    {
        UnequipWeapon(gun.GetComponent<NetworkIdentity>().netId);
        RpcUnequipWeapon(gun.GetComponent<NetworkIdentity>().netId);
    }

    [Command]
    public void CmdSelectWeapon()
    {
        SelectWeapon();
        RpcSelectWeapon();
    }

    [ClientRpc]
    void RpcEquipWeapon(NetworkInstanceId weaponId)
    {
        EquipWeapon(weaponId);
    }

    void EquipWeapon(NetworkInstanceId weaponId)
    {
        GameObject weapon = ClientScene.FindLocalObject(weaponId);
        weapon.transform.SetParent(weaponHolder.transform);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.GetComponent<Rigidbody>().isKinematic = true;
        weapon.GetComponent<Gun>().cam = gameObject.GetComponent<Player_Network>().firstPersonCharacter;
    }

    [ClientRpc]
    void RpcUnequipWeapon(NetworkInstanceId weaponId)
    {
        UnequipWeapon(weaponId);
    }

    void UnequipWeapon(NetworkInstanceId weaponId)
    {
        GameObject weapon = ClientScene.FindLocalObject(weaponId);
        weapon.transform.SetParent(null);
        weapon.GetComponent<Gun>().cam = null;
        weapon.GetComponent<Rigidbody>().isKinematic = false;
    }

 

    [ClientRpc]
    void RpcSelectWeapon()
    {
        SelectWeapon();
    }

    void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in weaponHolder.transform)
        {
            if (i == currentWeaponIndex)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
