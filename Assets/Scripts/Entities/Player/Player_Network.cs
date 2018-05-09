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
    public GameObject weaponHolder;

    private WeaponManager weaponManager;

    public override void OnStartLocalPlayer()
    {
        GetComponent<FirstPersonController>().enabled = true;
        firstPersonCharacter.SetActive(true);

        foreach (GameObject go in characterModel)
        {
            go.SetActive(false);
        }

        weaponManager = gameObject.GetComponent<WeaponManager>();

        weaponManager.CmdEquipWeapon(GameObject.Find("Gun"));

        Invoke("Helper", 5);
        Invoke("Helper2", 10);
    }

    void Helper()
    {
        weaponManager.CmdUnequipWeapon(GameObject.Find("Gun"));
    }
    void Helper2()
    {
        weaponManager.CmdEquipWeapon(GameObject.Find("Gun"));
    }
}
