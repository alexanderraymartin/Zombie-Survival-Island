using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player_Network))]
public class WeaponManager : NetworkBehaviour
{
    public GameObject weaponHolder;

    [Command]
    public void CmdEquipWeapon(GameObject gun)
    {
        if (!isLocalPlayer || gun == null)
        {
            return;
        }

        EquipWeapon(gun.GetComponent<NetworkIdentity>().netId);
        RpcEquipWeapon(gun.GetComponent<NetworkIdentity>().netId);
    }

    [Command]
    public void CmdUnequipWeapon(GameObject gun)
    {
        if (!isLocalPlayer || gun == null)
        {
            return;
        }

        UnequipWeapon(gun.GetComponent<NetworkIdentity>().netId);
        RpcUnequipWeapon(gun.GetComponent<NetworkIdentity>().netId);
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

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetButtonDown("Fire2") && weaponHolder.transform.childCount == 0)
        {
            Debug.Log("Attemping to equip...");
            CmdEquipWeapon(GameObject.Find("Gun"));
        }
        else if (Input.GetButtonDown("Fire2") && weaponHolder.transform.childCount != 0)
        {
            Debug.Log("Attemping to unequip...");
            CmdUnequipWeapon(GameObject.Find("Gun"));
        }
    }
}
