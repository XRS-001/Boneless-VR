using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class HandPresence : MonoBehaviour
{
    public InputActionProperty trigger;
    public InputActionProperty grip;
    public ClimbingPhysics handPhysics;
    public Animator handAnimator;
    void Start()
    {
    }
    void UpdateHandAnimation()
    {
        float triggerValue = trigger.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);

        float gripValue = grip.action.ReadValue<float>();

        if (handPhysics.isGrabbing)
        {
            handAnimator.SetBool("Climbing", true);
        }
        else
        {
            handAnimator.SetBool("Climbing", false);
            handAnimator.SetFloat("Grip", gripValue);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHandAnimation();
    }
}
