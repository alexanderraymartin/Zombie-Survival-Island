using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player_Network))]
[RequireComponent(typeof(NetworkTransform))]
public class AbilityManager : NetworkBehaviour
{
    public GameObject ability;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
