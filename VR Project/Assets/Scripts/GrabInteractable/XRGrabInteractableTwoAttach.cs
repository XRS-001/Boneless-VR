using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractableTwoAttach : XRGrabInteractable
{
    public Transform rightAttach;
    public Transform leftAttach;
    public bool rightHandGrabbing;
    public bool leftHandGrabbing;
    public bool isGrabbing;
    public ControllerInteractors controllerGrabbing;
    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("LeftHand"))
        {
            attachTransform = leftAttach;
        }
        else if (args.interactorObject.transform.CompareTag("RightHand"))
        {
            attachTransform = rightAttach;
        }
        base.OnSelectEntering(args);
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
        controllerGrabbing = args.interactorObject.transform.GetComponent<ControllerInteractors>();
        base.OnSelectEntered(args);
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        leftHandGrabbing = false;
        rightHandGrabbing = false;
        isGrabbing = false;
        base.OnSelectExited(args);
    }
}



