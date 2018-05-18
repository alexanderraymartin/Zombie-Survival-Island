using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NetworkAnimator))]
public class Animate : NetworkBehaviour
{

    private Animator animator;
    private NetworkAnimator networkAnimator;

    public void SetAnimatorTrigger(string name)
    {
        animator.SetTrigger(name);
        networkAnimator.SetTrigger(name);
    }

    public void SetAnimatorBool(string name, bool value)
    {
        ClearAnimator();
        animator.SetBool(name, value);
    }

    public void ClearAnimator()
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            animator.SetBool(parameter.name, false);
        }
    }

    [ServerCallback]
    void Start()
    {
        animator = GetComponent<Animator>();
        networkAnimator = GetComponent<NetworkAnimator>();
    }
}
