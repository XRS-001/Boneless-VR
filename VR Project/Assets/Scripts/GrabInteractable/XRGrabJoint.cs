using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabJoint : XRGrabInteractable
{
    public bool isGrabbing;
    private Collider[] handColliders;
    private Collider[] previousHandColliders;
    public Transform rightAttach;
    public Transform leftAttach;
    public bool rightHandGrabbing;
    public bool leftHandGrabbing;
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
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("RightHand"))
        {
            leftHandGrabbing = false;
            rightHandGrabbing = true;
        }
        else
        {
            rightHandGrabbing = false;
            leftHandGrabbing = true;
        }
        isGrabbing = true;
        handColliders = args.interactorObject.transform.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().handColliders;
        foreach (Collider collider in handColliders)
        {
            collider.gameObject.SetActive(false);
        }
        args.interactorObject.transform.GetComponent<ControllerInteractors>().handPresence.transform.position = attachTransform.position;
        args.interactorObject.transform.GetComponent<ControllerInteractors>().handPresence.transform.rotation = attachTransform.rotation;
        args.interactorObject.transform.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().target = attachTransform;
        base.OnSelectEntered(args);
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        previousHandColliders = handColliders;
        isGrabbing = false;
        StartCoroutine(Delay());
        args.interactorObject.transform.GetComponent<ControllerInteractors>().handPresence.transform.position = args.interactorObject.transform.GetComponent<ControllerInteractors>().transform.position;
        args.interactorObject.transform.GetComponent<ControllerInteractors>().handPresence.transform.rotation = args.interactorObject.transform.GetComponent<ControllerInteractors>().transform.rotation;
        args.interactorObject.transform.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().target
            = args.interactorObject.transform.GetComponent<ControllerInteractors>().handPhysics.transform;
        base.OnSelectExited(args);
    }
    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.5f);

        if (!leftHandGrabbing && !rightHandGrabbing)
        {
            foreach (Collider collider in handColliders)
            {
                collider.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Collider collider in previousHandColliders)
            {
                collider.gameObject.SetActive(true);
            }
        }
    }
}