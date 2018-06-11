using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Gun))]
public class Scope : MonoBehaviour
{
    public GameObject scopeCanvas;

    public bool isScoped;

    private Player_Network gunOwner;
    private GameObject weaponCamera;

    void Update()
    {
        gunOwner = GetComponent<Gun>().gunOwner;

        if (gunOwner != null)
        {
            weaponCamera = gunOwner.weaponCamera;
        }

        if (!isScoped)
        {
            ScopeOut();
        }
    }

    public void ScopeOut()
    {
        isScoped = false;
        scopeCanvas.SetActive(false);
        if (weaponCamera != null)
        {
            weaponCamera.SetActive(true);
        }
    }

    public IEnumerator ScopeIn()
    {
        isScoped = true;
        yield return new WaitForSeconds(0.20f);
        scopeCanvas.SetActive(true);
        if (weaponCamera != null)
        {
            weaponCamera.SetActive(false);
        }
    }
}
