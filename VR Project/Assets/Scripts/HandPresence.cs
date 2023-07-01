using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class HandPresence : MonoBehaviour
{
    public InputActionProperty trigger;
    public InputActionProperty grip;
    public Animator handAnimator;
    private Animator physicsAnimator;
    public ControllerInteractors controller;
    void Start()
    {
        physicsAnimator = controller.physicsAnimator;
    }
    void UpdateHandAnimation()
    {
        float triggerValue = trigger.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);

        float gripValue = grip.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);

        physicsAnimator.SetFloat("Trigger", triggerValue);
        physicsAnimator.SetFloat("Grip", gripValue);

        if (controller != null)
        {
            if (controller.isGrabbing)
            {
                physicsAnimator.SetFloat("Trigger", 100.0f);
            }
            else if (!controller.isGrabbing)
            {
                physicsAnimator.SetFloat("Trigger", 0.0f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHandAnimation();
    }
}
