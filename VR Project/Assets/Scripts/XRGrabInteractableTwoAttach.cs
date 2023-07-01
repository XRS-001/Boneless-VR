using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractableTwoAttach : XRGrabInteractable
{
    public Transform rightAttach;
    public Transform leftAttach;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {

        if (args.interactorObject.transform.CompareTag("LeftHand"))
        {
            attachTransform = leftAttach;
            transform.position = leftAttach.position;
        }
        else if (args.interactorObject.transform.CompareTag("RightHand"))
        {
            attachTransform = rightAttach;
            transform.position = rightAttach.position;
        }

        base.OnSelectEntered(args);
    }
}



