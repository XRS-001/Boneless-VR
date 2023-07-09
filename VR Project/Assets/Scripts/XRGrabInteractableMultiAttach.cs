using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractableMultiAttach : XRGrabInteractable
{
    public GrabHandPose grabHandPose;
    private float primaryDistanceLeft;
    private float primaryDistanceRight;
    private float altDistanceLeft;
    private float altDistanceRight;
    public Transform leftAttach;
    public Transform rightAttach;
    public Transform altLeftAttach;
    public Transform altRightAttach;
    public HandData leftAttachPose;
    public HandData rightAttachPose;
    public HandData altLeftAttachPose;
    public HandData altRightAttachPose;
    private Coroutine leftHandCoroutine;
    private Coroutine rightHandCoroutine;

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("LeftHand"))
        {
            leftHandCoroutine = StartCoroutine(UpdateLeftAttachTransform(args.interactorObject.transform));
        }
        else if (args.interactorObject.transform.CompareTag("RightHand"))
        {
            rightHandCoroutine = StartCoroutine(UpdateRightAttachTransform(args.interactorObject.transform));
        }
        base.OnHoverEntered(args);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("LeftHand") && leftHandCoroutine != null)
        {
            StopCoroutine(leftHandCoroutine);
            leftHandCoroutine = null;
        }
        else if (args.interactorObject.transform.CompareTag("RightHand") && rightHandCoroutine != null)
        {
            StopCoroutine(rightHandCoroutine);
            rightHandCoroutine = null;
        }
        base.OnHoverExited(args);
    }

    private IEnumerator UpdateLeftAttachTransform(Transform interactor)
    {
        while (true)
        {
            float primaryDistance = Vector3.Distance(interactor.position, leftAttach.position);
            float altDistance = Vector3.Distance(interactor.position, altLeftAttach.position);
            if (primaryDistance < altDistance)
            {
                attachTransform = leftAttach;
                grabHandPose.leftHandPose = leftAttachPose;
            }
            else
            {
                attachTransform = altLeftAttach;
                grabHandPose.leftHandPose = altLeftAttachPose;
            }
            yield return null;
        }
    }

    private IEnumerator UpdateRightAttachTransform(Transform interactor)
    {
        while (true)
        {
            float primaryDistance = Vector3.Distance(interactor.position, rightAttach.position);
            float altDistance = Vector3.Distance(interactor.position, altRightAttach.position);
            if (primaryDistance < altDistance)
            {
                attachTransform = rightAttach;
                grabHandPose.rightHandPose = rightAttachPose;
            }
            else
            {
                attachTransform = altRightAttach;
                grabHandPose.rightHandPose = altRightAttachPose;
            }
            yield return null;
        }
    }

}
