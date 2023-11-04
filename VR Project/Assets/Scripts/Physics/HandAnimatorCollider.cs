using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class HandAnimatorCollider : MonoBehaviour
{
    public InputActionProperty trigger;
    public ControllerInteractors controllerInteractors;
    public InputActionProperty grip;
    public Animator handAnimator;
    void Start()
    {
    }
    void UpdateHandAnimation()
    {
        if (!controllerInteractors.isGrabbing)
        {
            float triggerValue = trigger.action.ReadValue<float>();
            handAnimator.SetFloat("Trigger", triggerValue);

            float gripValue = grip.action.ReadValue<float>();
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 1);

            handAnimator.SetFloat("Grip", 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHandAnimation();
    }
}