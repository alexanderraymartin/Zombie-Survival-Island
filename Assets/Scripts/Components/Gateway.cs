using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gateway : MonoBehaviour
{
    public bool isOpen = false;

    private bool isOpening = false;

    public void OpenGateway()
    {
        Debug.Log("Opening Gateway!");
        GetComponent<Gateway>().StartAnimation();
    }

    void StartAnimation()
    {
        gameObject.SetActive(false);
        //StartCoroutine(AnimateGateway());
    }

    IEnumerator AnimateGateway()
    {
        if (isOpen)
        {
            yield break;
        }

        isOpen = true;
        isOpening = true;

        MeshRenderer mesh = GetComponent<MeshRenderer>();
        Color oldColor = mesh.material.color;

        float t = 0.0f;

        while (t < 1.0f)
        {
            Debug.Log("Still Opening");
            Debug.Log(oldColor.a);
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / 1.0f);
            Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, 0.0f);
            gameObject.GetComponent<MeshRenderer>().material.color = newColor;
            yield return null;
        }

        Debug.Log("Finished Gateway Animation");

    }
}
