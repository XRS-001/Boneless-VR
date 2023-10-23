using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabDynamic : XRGrabInteractableTwoAttach
{
    public ControllerInteractors leftController;
    public ControllerInteractors rightController;
    public Transform leftPresence;
    public Transform rightPresence;
    public float maxAttachDistance;
    [Header("For NPC's Only:")]
    public BehaviourPuppet puppet;
    public float collisionResistanceGrabbing;
    public float knockOutDistanceGrabbing;

    public float collisionResistance;
    public float knockOutDistance;

    // Update is called once per frame
    void Update()
    {
        if(!rightController.isGrabbing)
        {
            Vector3 directionRight = transform.position - rightPresence.gameObject.transform.position;
            RaycastHit rightHit;
            Physics.Raycast(rightPresence.position, directionRight, out rightHit, LayerMask.GetMask("Interactable"));
            Vector3 positionRight = rightHit.collider.ClosestPoint(rightPresence.position);
            rightAttach.position = positionRight;
            Vector3 localPositionRight = rightAttach.localPosition;
            localPositionRight.x = Mathf.Clamp(localPositionRight.x, -maxAttachDistance, maxAttachDistance);
            localPositionRight.y = Mathf.Clamp(localPositionRight.y, -maxAttachDistance, maxAttachDistance);
            localPositionRight.z = Mathf.Clamp(localPositionRight.z, -maxAttachDistance, maxAttachDistance);
            rightAttach.localPosition = localPositionRight;
            rightAttach.rotation = rightPresence.rotation;
        }

        if (!leftController.isGrabbing)
        {
            Vector3 directionLeft = transform.position - leftPresence.gameObject.transform.position;
            RaycastHit leftHit;
            Physics.Raycast(leftPresence.position, directionLeft, out leftHit, LayerMask.GetMask("Interactable"));
            Vector3 positionLeft = leftHit.collider.ClosestPoint(leftPresence.position);
            leftAttach.position = positionLeft;
            Vector3 localPositionLeft = leftAttach.localPosition;
            localPositionLeft.x = Mathf.Clamp(localPositionLeft.x, -maxAttachDistance, maxAttachDistance);
            localPositionLeft.y = Mathf.Clamp(localPositionLeft.y, -maxAttachDistance, maxAttachDistance);
            localPositionLeft.z = Mathf.Clamp(localPositionLeft.z, -maxAttachDistance, maxAttachDistance);
            leftAttach.localPosition = localPositionLeft;
            leftAttach.rotation = leftPresence.rotation;
        }
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (puppet)
        {
            puppet.collisionResistance.floatValue = collisionResistanceGrabbing;
            puppet.defaults.knockOutDistance = knockOutDistanceGrabbing;
            puppet.canGetUp = false;
        }
    }
    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        if (isGrabbing)
        {
            GetComponent<Rigidbody>().mass /= 2;
        }
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (puppet && !leftController.isGrabbing && !rightController.isGrabbing)
        {
            puppet.collisionResistance.floatValue = collisionResistance;
            puppet.defaults.knockOutDistance = knockOutDistance;
            puppet.canGetUp = true;
        }
        base.OnSelectExited(args);
        if (leftController.isGrabbing || rightController.isGrabbing)
        {
            GetComponent<Rigidbody>().mass *= 2;
            isGrabbing = true;
        }
    }
}
