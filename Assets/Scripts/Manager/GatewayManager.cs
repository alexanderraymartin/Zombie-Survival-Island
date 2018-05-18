using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GatewayManager : NetworkBehaviour
{
    public static GatewayManager instance = null;
    public GameObject[] beachGateways;

    [ServerCallback]
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    [ClientRpc]
    public void RpcOpenGateway(GameObject gateway)
    {
        if (gateway == null || !(gateway.tag == "Gateway"))
        {
            return;
        }

        Debug.Log("Opening Gateway!");
        gateway.GetComponent<Gateway>().StartAnimation();
    }


    // Update is called once per frame
    void Update()
    {
    }
}
