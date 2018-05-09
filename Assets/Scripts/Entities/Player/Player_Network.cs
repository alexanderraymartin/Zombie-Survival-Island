using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(Health))]
public class Player_Network : NetworkBehaviour
{
    public GameObject firstPersonCharacter;
    public GameObject[] characterModel;
    public GameObject weaponHolder;

    public override void OnStartLocalPlayer()
    {
        GetComponent<FirstPersonController>().enabled = true;
        firstPersonCharacter.SetActive(true);

        foreach (GameObject go in characterModel)
        {
            go.SetActive(false);
        }

        CmdEquipWeapon(GameObject.Find("Gun"));
    }

    [Command]
    public void CmdEquipWeapon(GameObject gun)
    {
        gun.transform.SetParent(weaponHolder.transform);
        gun.transform.localPosition = new Vector3(0, 0, 0);
        gun.transform.localRotation = Quaternion.identity;
        
        gun.GetComponent<Gun>().cam = firstPersonCharacter;
        if (gun.GetComponent<Gun>().cam != null)
        {
            Debug.Log("Gun Equiped");
        }
        else
        {
            Debug.Log("Gun not found");
        }
    }

    public void Unequip(Gun gun)
    {
        gun.transform.SetParent(null);
        gun.cam = null;
    }
}
