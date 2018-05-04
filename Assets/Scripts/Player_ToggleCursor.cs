using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

public class Player_ToggleCursor : NetworkBehaviour
{
    public FirstPersonController fpsController;

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetButtonUp("Cancel"))
        {
            ToggleCursor();
        }
    }

    void ToggleCursor()
    {
        fpsController.enabled = !fpsController.enabled;
    }
}
