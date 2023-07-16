using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractableMultiAttach : XRGrabInteractable
{
    public GrabHandPose grabHandPose;
    public Transform leftAttachPrimary;
    public Transform rightAttachPrimary;
    public Transform leftAttachAlt;
    public Transform rightAttachAlt;
    public HandData leftAttachPrimaryPose;
    public HandData rightAttachPrimaryPose;
    public HandData leftAttachAltPose;
    public HandData rightAttachAltPose;
    private Transform leftAttachTransform;
    private Transform rightAttachTransform;
    private Coroutine leftHandCoroutine;
    private Coroutine rightHandCoroutine;

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("LeftHand"))
        {
            if (leftHandCoroutine == null)
            {
                leftHandCoroutine = StartCoroutine(UpdateAttachTransform(args.interactorObject.transform, true));
            }
        }
        else if (args.interactorObject.transform.CompareTag("RightHand"))
        {
            if (rightHandCoroutine == null)
            {
                rightHandCoroutine = StartCoroutine(UpdateAttachTransform(args.interactorObject.transform, false));
            }
        }
        base.OnHoverEntered(args);
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        Transform interactorTransform = args.interactorObject.transform;
        if (interactorTransform.CompareTag("LeftHand"))
        {
            if (leftAttachTransform != null)
            {
                attachTransform = leftAttachTransform;
                grabHandPose.leftHandPose = leftAttachTransform == leftAttachPrimary ? leftAttachPrimaryPose : leftAttachAltPose;
            }
        }
        else if (interactorTransform.CompareTag("RightHand"))
        {
            if (rightAttachTransform != null)
            {
                attachTransform = rightAttachTransform;
                grabHandPose.rightHandPose = rightAttachTransform == rightAttachPrimary ? rightAttachPrimaryPose : rightAttachAltPose;
            }
        }
        base.OnSelectEntering(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        Transform interactorTransform = args.interactorObject.transform;
        if (interactorTransform.CompareTag("LeftHand"))
        {
            leftAttachTransform = null;
            if (leftHandCoroutine != null)
            {
                StopCoroutine(leftHandCoroutine);
                leftHandCoroutine = null;
            }
        }
        else if (interactorTransform.CompareTag("RightHand"))
        {
            rightAttachTransform = null;
            if (rightHandCoroutine != null)
            {
                StopCoroutine(rightHandCoroutine);
                rightHandCoroutine = null;
            }
        }
        base.OnSelectExited(args);
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

    private IEnumerator UpdateAttachTransform(Transform interactor, bool isLeftHand)
    {
        while (true)
        {
            Transform primaryTransform = isLeftHand ? leftAttachPrimary : rightAttachPrimary;
            Transform altTransform = isLeftHand ? leftAttachAlt : rightAttachAlt;

            float primaryDistance = Vector3.Distance(interactor.position, primaryTransform.position);
            float altDistance = Vector3.Distance(interactor.position, altTransform.position);
            if (primaryDistance < altDistance)
            {
                if (isLeftHand)
                {
                    leftAttachTransform = primaryTransform;
                    grabHandPose.leftHandPose = leftAttachPrimaryPose;
                }
                else
                {
                    rightAttachTransform = primaryTransform;
                    grabHandPose.rightHandPose = rightAttachPrimaryPose;
                }
            }
            else
            {
                if (isLeftHand)
                {
                    leftAttachTransform = altTransform;
                    grabHandPose.leftHandPose = leftAttachAltPose;
                }
                else
                {
                    rightAttachTransform = altTransform;
                    grabHandPose.rightHandPose = rightAttachAltPose;
                }
            }

            yield return null;
        }
    }
}





