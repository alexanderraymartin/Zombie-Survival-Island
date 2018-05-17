using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatewayManager : MonoBehaviour {

    public GatewayManager instance = null;
    public GameObject[] beachGateways;



	// Use this for initialization
	void Awake () {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
	}

    public static void openGateway(GameObject gateway) {
        if (gateway == null || !(gateway.tag == "Gateway")) {
            return; 
        }

        Debug.Log("Opening Gateway!");
        gateway.GetComponent<Gateway>().StartAnimation();
    }
    
	
	// Update is called once per frame
	void Update () {
	}
}
