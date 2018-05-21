using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[RequireComponent(typeof(Player_Network))]
public class Player_UI : NetworkBehaviour
{
    public GameObject UI;

    private GameObject crossHair;
    private GameObject ammoBackground;
    private GameObject ammoText;

    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        GameObject instance = Instantiate(UI);
        
        crossHair = instance.transform.Find("Crosshair").gameObject;
        ammoBackground = instance.transform.Find("AmmoBackground").gameObject;
        ammoText = ammoBackground.transform.Find("AmmoText").gameObject;
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        SetAmmoText();
    }

    void SetAmmoText()
    {
        GameObject activeWeapon = GetComponent<WeaponManager>().GetActiveWeapon();
        string ammo = "";

        if (activeWeapon == null)
        {
            ammo = "0 / 0";
        }
        else
        {
            ammo = activeWeapon.GetComponent<Gun>().clipAmmo + " | " + activeWeapon.GetComponent<Gun>().reserveAmmo;
        }

        ammoText.GetComponent<Text>().text = ammo;
    }
}
