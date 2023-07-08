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
    void Start()
    {
    }
    void UpdateHandAnimation()
    {
        float triggerValue = trigger.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);

        float gripValue = grip.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHandAnimation();
    }
}
