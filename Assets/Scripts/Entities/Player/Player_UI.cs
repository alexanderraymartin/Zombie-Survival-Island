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
    private GameObject currencyBackground;
    private GameObject currencyText;

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
        currencyBackground = instance.transform.Find("CurrencyBackground").gameObject;
        currencyText = currencyBackground.transform.Find("CurrencyText").gameObject;
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        SetAmmoText();
        SetWaveText();
        SetCurrencyText();
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

    void SetWaveText()
    {
        GameObject tempUI = GameObject.FindGameObjectWithTag("UI");
        GameObject tempWaveBackground = tempUI.transform.Find("WaveBackground").gameObject;
        GameObject tempWaveText = tempWaveBackground.transform.Find("WaveText").gameObject;
        GameObject tempRemainingText = tempWaveBackground.transform.Find("RemainingText").gameObject;
        GameObject spawner = GameObject.FindGameObjectWithTag("Spawner");

        int wave = spawner.GetComponent<Spawner>().wave;
        int remaining = spawner.GetComponent<Spawner>().zombiesAlive;

        tempWaveText.GetComponent<Text>().text = "Wave: " + wave;
        tempRemainingText.GetComponent<Text>().text = "Remaining: " + remaining;
    }

    void SetCurrencyText()
    {
        int currency = GetComponent<StatsManager>().currency;

        currencyText.GetComponent<Text>().text = "$" + currency.ToString();
    }
}
