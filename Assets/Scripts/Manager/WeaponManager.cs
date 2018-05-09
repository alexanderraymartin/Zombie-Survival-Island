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
        if (!isLocalPlayer || gun == null )
        {
            return;
        }
        
        gun.transform.SetParent(weaponHolder.transform);
        gun.transform.localPosition = new Vector3(0, 0, 0);
        gun.transform.localRotation = Quaternion.identity;
        gun.GetComponent<Rigidbody>().isKinematic = true;
        gun.GetComponent<Gun>().cam = gameObject.GetComponent<Player_Network>().firstPersonCharacter;
    }

    [Command]
    public void CmdUnequipWeapon(GameObject gun)
    {
        if (!isLocalPlayer || gun == null)
        {
            return;
        }
        
        gun.transform.SetParent(null);
        gun.GetComponent<Gun>().cam = null;
        gun.GetComponent<Rigidbody>().isKinematic = false;
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
