using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

public class Player_Ability : MonoBehaviour {

    private CharacterController characterController;
    private Player_UI playerUI;

    private bool teleporting = false;
    private float teleportTime = 0.5f;
    private float timeRemaining = 0.0f;

    /*************************** Init Functions ***************************/
    void Awake()
    {

    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerUI = GetComponent<Player_UI>();

    }

    void Update()
    {
        if (teleporting)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining > 0.0f)
            {
                
            }
        }
    }

    public void Teleport()
    {
        // Read input
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        float vertical = CrossPlatformInputManager.GetAxis("Vertical");

        Vector2 direction = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (direction.sqrMagnitude > 1)
        {
            direction.Normalize();
        }

        Vector3 desiredMove = transform.forward * direction.y + transform.right * direction.x;

        characterController.Move(desiredMove * 5);

        teleporting = true;
        timeRemaining = teleportTime;
    }

    /*************************** Public Functions ***************************/


    /*************************** Cmd Functions ***************************/
    //[Command]


    /*************************** Rpc Functions ***************************/
    //[ClientRpc]


    /*************************** Helper Functions ***************************/

}
