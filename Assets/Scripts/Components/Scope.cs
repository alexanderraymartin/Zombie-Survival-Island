using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Gun))]
public class Scope : MonoBehaviour
{
    public GameObject scopeCanvas;

    public float scopedFOV = 15.0f;
    private float normalFOV = 60.0f;

    [HideInInspector]
    public bool isScoped;

    void Update()
    {
        if (!isScoped)
        {
            ScopeOut();
        }
    }

    public void ScopeOut()
    {
        isScoped = false;
        scopeCanvas.SetActive(false);

        Player_Network gunOwner = GetComponent<Gun>().gunOwner;

        if (gunOwner != null)
        {
            gunOwner.weaponCamera.SetActive(true);
            gunOwner.firstPersonCharacter.GetComponent<Camera>().fieldOfView = normalFOV;
        }
    }

    public IEnumerator ScopeIn()
    {
        isScoped = true;
        yield return new WaitForSeconds(0.15f);
        scopeCanvas.SetActive(true);

        Player_Network gunOwner = GetComponent<Gun>().gunOwner;

        if (gunOwner != null)
        {
            gunOwner.weaponCamera.SetActive(false);
            normalFOV = GetComponent<Gun>().gunOwner.firstPersonCharacter.GetComponent<Camera>().fieldOfView;
            gunOwner.firstPersonCharacter.GetComponent<Camera>().fieldOfView = scopedFOV;
        }
    }
}
