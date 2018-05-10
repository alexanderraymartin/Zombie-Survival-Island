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
    private int currentWeaponIndex = -1;

    private NetworkTransform networkTransform;

    [Command]
    public void CmdChangeWeapons()
    {
        if (weaponHolder.transform.childCount > 1)
        {
            if (currentWeaponIndex >= transform.childCount - 1)
            {
                currentWeaponIndex = 0;
            }
            else
            {
                currentWeaponIndex++;
            }
        }
        SelectWeapon();
    }

    public GameObject GetActiveWeapon()
    {
        if (weaponHolder.transform.childCount != 0)
        {
            return weaponHolder.transform.GetChild(currentWeaponIndex).gameObject;
        }
        return null;
    }

    [Command]
    public void CmdEquipWeapon(GameObject gun)
    {
        if (gun == null)
        {
            return;
        }

        // Check if weapon slots are full
        if (weaponHolder.transform.childCount >= maxWeapons)
        {
            GameObject oldWeapon = GetActiveWeapon();
            // Unequip old weapon
            UnequipWeapon(oldWeapon.GetComponent<NetworkIdentity>().netId);
            RpcUnequipWeapon(oldWeapon.GetComponent<NetworkIdentity>().netId);
            // Update index of current weapon
            currentWeaponIndex--;
        }
        // Equip new weapon
        EquipWeapon(gun.GetComponent<NetworkIdentity>().netId);
        RpcEquipWeapon(gun.GetComponent<NetworkIdentity>().netId);
        // Update index of current weapon
        currentWeaponIndex++;
        // Update current weapon
        SelectWeapon();
        RpcSelectWeapon();
    }

    [Command]
    public void CmdUnequipWeapon()
    {
        GameObject gun = GetActiveWeapon();
        if (gun != null)
        {
            // Update index of current weapon
            currentWeaponIndex--;
            // Unequip old weapon
            UnequipWeapon(gun.GetComponent<NetworkIdentity>().netId);
            RpcUnequipWeapon(gun.GetComponent<NetworkIdentity>().netId);
            // Update current weapon
            SelectWeapon();
            RpcSelectWeapon();
        }
    }

    [ClientRpc]
    void RpcEquipWeapon(NetworkInstanceId weaponId)
    {
        EquipWeapon(weaponId);
    }

    [ClientRpc]
    void RpcUnequipWeapon(NetworkInstanceId weaponId)
    {
        UnequipWeapon(weaponId);
    }

    [ClientRpc]
    void RpcSelectWeapon()
    {
        SelectWeapon();
    }

    void EquipWeapon(NetworkInstanceId weaponId)
    {
        GameObject weapon = ClientScene.FindLocalObject(weaponId);
        weapon.transform.SetParent(weaponHolder.transform);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.GetComponent<Rigidbody>().isKinematic = true;
        weapon.GetComponent<Gun>().cam = gameObject.GetComponent<Player_Network>().firstPersonCharacter;
        weapon.GetComponent<Gun>().gunOwner = gameObject.GetComponent<Player_Network>();
    }

    void UnequipWeapon(NetworkInstanceId weaponId)
    {
        GameObject weapon = ClientScene.FindLocalObject(weaponId);
        weapon.transform.SetParent(null);
        weapon.GetComponent<Gun>().cam = null;
        weapon.GetComponent<Rigidbody>().isKinematic = false;
        weapon.GetComponent<Gun>().gunOwner = null;
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

        // Just to be safe
        if (weaponHolder.transform.childCount == 1)
        {
            currentWeaponIndex = 0;
            weaponHolder.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
