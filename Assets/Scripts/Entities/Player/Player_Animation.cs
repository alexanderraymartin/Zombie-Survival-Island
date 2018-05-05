using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player_Animation : NetworkBehaviour
{

    public Animator playerAnimator;

    // Update is called once per frame
    void Update()
    {
        GetPlayerInput();
    }

    void GetPlayerInput()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
        {
            playerAnimator.SetBool("Moving", true);
        }
        else
        {
            playerAnimator.SetBool("Moving", false);
        }
    }
}
