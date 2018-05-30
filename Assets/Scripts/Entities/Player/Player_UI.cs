using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[RequireComponent(typeof(Player_Network))]
public class Player_UI : NetworkBehaviour
{
    public GameObject UI;
    private int frames = 0;
    private int curRemaining = 0;
    private int curWave = 0;

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

        //used to prevent sync issues, only update the UI once all players are in
        frames++;

        //onyl server will get this object, otherwise it is null
        GameObject spawner = GameObject.FindGameObjectWithTag("Spawner");
        if (spawner && frames > 500) {
            int wave = spawner.GetComponent<Spawner>().wave;
            int remaining = spawner.GetComponent<Spawner>().zombiesAlive;
            //this check is to be more efficient, running the rpc every frame caused major lag
            if (wave > curWave || curRemaining > remaining)
            {
                curWave = wave;
                curRemaining = remaining;
                CmdWaveText(wave, remaining);
            }
        }
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

    [Command]
    void CmdWaveText(int wave, int remaining)
    {
        RpcWaveText(wave, remaining);
    }

    [ClientRpc]
    void RpcWaveText(int wave, int remaining)
    {
        //run on all clients, otherwise server is not updated
        SetWaveTextHelper(wave, remaining);
    }

    void SetWaveTextHelper(int wave, int remaining)
    {
        //this seems wrong, but I get null refs if doing it through class variables
        GameObject tempUI = GameObject.FindGameObjectWithTag("UI");
        GameObject tempWaveBackground = tempUI.transform.Find("WaveBackground").gameObject;
        GameObject tempWaveText = tempWaveBackground.transform.Find("WaveText").gameObject;
        GameObject tempRemainingText = tempWaveBackground.transform.Find("RemainingText").gameObject;

        tempWaveText.GetComponent<Text>().text = "Wave: " + wave;
        
        tempRemainingText.GetComponent<Text>().text = "Remaining: " + remaining;
        
    }
}
