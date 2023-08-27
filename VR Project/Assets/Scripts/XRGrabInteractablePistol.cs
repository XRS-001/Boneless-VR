using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractablePistol : XRGrabInteractable
{
    public bool isGrabbing { get; private set; } = false;
    public Collider slideCollider;
    private Collider[] handColliders;
    private Collider[] previousHandColliders;
    public bool rightHandGrabbing = false;
    public bool leftHandGrabbing = false;
    public Transform rightAttach;
    public Transform leftAttach;
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("LeftHand"))
        {
            attachTransform = leftAttach;
        }
        else if (args.interactorObject.transform.CompareTag("RightHand"))
        {
            attachTransform = rightAttach;
        }
        base.OnHoverEntered(args);
    }
    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        isGrabbing = true;
        handColliders = args.interactorObject.transform.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().handColliders;
        foreach (Collider collider in handColliders)
        {
            Physics.IgnoreCollision(collider, slideCollider, true);
        }
        base.OnSelectEntering(args);
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("RightHand"))
        {
            rightHandGrabbing = true;
            leftHandGrabbing = false;
        }
        else
        {
            leftHandGrabbing = true;
            rightHandGrabbing = false;
        }
        base.OnSelectEntered(args);
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        rightHandGrabbing = false;
        leftHandGrabbing = false;
        isGrabbing = false;
        StartCoroutine(DelaySetActive());
        previousHandColliders = handColliders;
        base.OnSelectExited(args);
    }
    IEnumerator DelaySetActive()
    {
        yield return new WaitForSeconds(0.05f);

        if (!leftHandGrabbing && !rightHandGrabbing)
        {
            foreach (Collider collider in handColliders)
            {
                Physics.IgnoreCollision(collider, slideCollider, false);
            }
        }
        else
        {
            foreach (Collider collider in previousHandColliders)
            {
                Physics.IgnoreCollision(collider, slideCollider, false);
            }
        }
    }
}
