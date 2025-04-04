using System.Collections;
using RootMotion.Dynamics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NPCJointGrab : XRGrabInteractable
{
    public Transform rightAttach;
    public Transform leftAttach;
    public PuppetMaster puppetMaster;
    public BehaviourPuppet puppetFall;
    public bool rightHandGrabbing;
    public bool leftHandGrabbing;
    public float collisionResistanceGrabbing;
    public float knockOutDistanceGrabbing;

    private float collisionResistance;
    private float knockOutDistance;
    public ControllerInteractors leftController;
    public ControllerInteractors rightController;
    public Transform leftPresence;
    public Transform rightPresence;
    private bool isHovering;
    void Update()
    {
        if (!rightController.isGrabbing)
        {
            Vector3 directionRight = transform.position - rightPresence.gameObject.transform.position;
            RaycastHit rightHit;
            GetComponentInChildren<Collider>().Raycast(new Ray(rightPresence.position, directionRight), out rightHit, float.PositiveInfinity);

            Vector3 position = rightController.transform.position;
            position.x = GetComponent<CapsuleCollider>().ClosestPoint(rightController.transform.position).x;
            position.z = GetComponent<CapsuleCollider>().ClosestPoint(rightController.transform.position).z;
            position += rightPresence.right / 25 + -(rightPresence.forward / 8.5f);
            rightAttach.position = position;

            Vector3 localPosition = rightAttach.localPosition;
            localPosition.y = Mathf.Clamp(localPosition.y, GetComponent<CapsuleCollider>().height / 4, GetComponent<CapsuleCollider>().height);
            rightAttach.localPosition = localPosition;

            rightAttach.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rightPresence.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            Quaternion localRotation = rightAttach.localRotation;
            localRotation *= Quaternion.Euler(40, 15, 195);
            rightAttach.localRotation = localRotation;
        }

        if (!leftController.isGrabbing)
        {
            Vector3 directionLeft = transform.position - leftPresence.gameObject.transform.position;
            RaycastHit leftHit;
            GetComponentInChildren<Collider>().Raycast(new Ray(leftPresence.position, directionLeft), out leftHit, float.PositiveInfinity);

            Vector3 position = leftController.transform.position;
            position.x = GetComponent<CapsuleCollider>().ClosestPoint(leftController.transform.position).x;
            position.z = GetComponent<CapsuleCollider>().ClosestPoint(leftController.transform.position).z;
            position += leftPresence.right / 25 + -(leftPresence.forward / 8.5f);
            leftAttach.position = position;

            Vector3 localPosition = leftAttach.localPosition;
            localPosition.y = Mathf.Clamp(localPosition.y, GetComponent<CapsuleCollider>().height / 4, GetComponent<CapsuleCollider>().height);
            leftAttach.localPosition = localPosition;

            leftAttach.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, leftPresence.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            Quaternion localRotation = leftAttach.localRotation;
            localRotation *= Quaternion.Euler(40, 15, 195);
            leftAttach.localRotation = localRotation;
        }
        if (leftHandGrabbing)
        {
            attachTransform = rightAttach;
        }
        else if (rightHandGrabbing)
        {
            attachTransform = leftAttach;
        }
    }
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        isHovering = true;
        base.OnHoverEntered(args);
    }
    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        isHovering = false;
        base.OnHoverExited(args);
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (leftHandGrabbing || rightHandGrabbing)
        {
            foreach (Collider collider in args.interactorObject.transform.GetComponent<ControllerInteractors>().colliders)
            {
                collider.isTrigger = true;
            }
            rightHandGrabbing = true;
            leftHandGrabbing = true;
        }
        if (args.interactorObject.transform.CompareTag("RightHand"))
        {
            rightHandGrabbing = true;
        }
        else
        {
            leftHandGrabbing = true;
        }
        base.OnSelectEntered(args);
        if (puppetFall)
        {
            puppetMaster.transform.parent.GetComponent<NPC>().isGrabbing = true;
            puppetMaster.muscles[7].props.mappingWeight *= 4;
            if (!rightHandGrabbing || !leftHandGrabbing)
            {
                puppetMaster.muscleSpring /= 2;
            }
            puppetFall.collisionResistance.floatValue = collisionResistanceGrabbing;
            puppetFall.defaults.knockOutDistance = knockOutDistanceGrabbing;
            puppetFall.canGetUp = false;
        }
    }
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
        if (rightHandGrabbing || leftHandGrabbing)
        {
            if (puppetFall)
            {
                if (leftHandGrabbing)
                    leftController.weight *= 3;
                else
                    rightController.weight *= 3;
            }
            rightController.weight /= 2;
            leftController.weight /= 2;
        }
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (!leftHandGrabbing || !rightHandGrabbing)
        {
            rightHandGrabbing = false;
            leftHandGrabbing = false;
        }
        if (args.interactorObject.transform.CompareTag("RightHand"))
        {
            rightHandGrabbing = false;
        }
        else
        {
            leftHandGrabbing = false;
        }
        if (puppetFall && !leftController.isGrabbing && !rightController.isGrabbing)
        {
            puppetMaster.transform.parent.GetComponent<NPC>().isGrabbing = false;
            puppetMaster.angularLimits = false;
            puppetMaster.muscles[7].props.mappingWeight /= 4;
            puppetMaster.muscleSpring *= 2;
            puppetFall.collisionResistance.floatValue = collisionResistance;
            puppetFall.defaults.knockOutDistance = knockOutDistance;
            puppetFall.canGetUp = true;
        }
        base.OnSelectExited(args);
        if (leftController.isGrabbing || rightController.isGrabbing)
        {
            foreach (Collider collider in args.interactorObject.transform.GetComponent<ControllerInteractors>().colliders)
            {
                collider.isTrigger = false;
            }
        }
    }
}
