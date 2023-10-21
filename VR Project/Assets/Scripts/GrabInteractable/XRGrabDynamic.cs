using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabDynamic : XRGrabInteractableTwoAttach
{
    public Transform leftPresence;
    public Transform rightPresence;
    public float maxAttachDistance;
    // Update is called once per frame
    void Update()
    {
        Vector3 directionRight = transform.position - rightPresence.gameObject.transform.position;
        RaycastHit rightHit;
        Physics.Raycast(rightPresence.position, directionRight, out rightHit, LayerMask.GetMask("Interactable"));
        Vector3 positionRight = rightHit.point;
        rightAttach.position = positionRight;
        Vector3 localPositionRight = rightAttach.localPosition;
        localPositionRight.x = Mathf.Clamp(localPositionRight.x, -maxAttachDistance, maxAttachDistance);
        localPositionRight.y = Mathf.Clamp(localPositionRight.y, -maxAttachDistance, maxAttachDistance);
        localPositionRight.z = Mathf.Clamp(localPositionRight.z, -maxAttachDistance, maxAttachDistance);
        rightAttach.localPosition = localPositionRight;
        rightAttach.rotation = rightPresence.rotation;

        Vector3 directionLeft = transform.position - leftPresence.gameObject.transform.position;
        RaycastHit leftHit;
        Physics.Raycast(leftPresence.position, directionLeft, out leftHit, LayerMask.GetMask("Interactable"));
        Vector3 positionLeft = leftHit.point;
        leftAttach.position = positionLeft;
        Vector3 localPositionLeft = leftAttach.localPosition;
        localPositionLeft.x = Mathf.Clamp(localPositionLeft.x, -maxAttachDistance, maxAttachDistance);
        localPositionLeft.y = Mathf.Clamp(localPositionLeft.y, -maxAttachDistance, maxAttachDistance);
        localPositionLeft.z = Mathf.Clamp(localPositionLeft.z, -maxAttachDistance, maxAttachDistance);
        leftAttach.localPosition = localPositionLeft;
        leftAttach.rotation = leftPresence.rotation;
    }
}
