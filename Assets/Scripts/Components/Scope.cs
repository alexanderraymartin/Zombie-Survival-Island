using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Gun))]
public class Scope : MonoBehaviour
{
    public GameObject scopeCanvas;

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
    }

    public IEnumerator ScopeIn()
    {
        isScoped = true;
        yield return new WaitForSeconds(0.20f);
        scopeCanvas.SetActive(true);
    }
}
