using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractableKnife : XRGrabInteractableTwoAttach
{
    public Transform rightController;
    public Transform leftController;
    public Transform rightAttachSecond;
    public Transform leftAttachSecond;
    public bool rightAlt = false;
    public bool leftAlt = false;
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);

    }
    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        if (args.interactorObject.transform.CompareTag("LeftHand") && leftAlt)
        {
            attachTransform = leftAttachSecond;
        }
        else if (args.interactorObject.transform.CompareTag("RightHand") && rightAlt)
        {
            attachTransform = rightAttachSecond;
        }
    }
    // Update is called once per frame
    void Update()
    {
        float leftAngle = Mathf.DeltaAngle(leftController.transform.localRotation.eulerAngles.x, transform.rotation.eulerAngles.x);
        Debug.Log(leftAngle);

        if (leftAngle < 0)
        {
            leftAlt = true;
        }
        else
        {
            leftAlt = false;
        }

        float rightAngle = Mathf.DeltaAngle(rightController.transform.localRotation.eulerAngles.x, transform.rotation.eulerAngles.x);
        if (rightAngle < 0)
        {
            rightAlt = true;
        }
        else
        {
            rightAlt = false;
        }
    }
}
