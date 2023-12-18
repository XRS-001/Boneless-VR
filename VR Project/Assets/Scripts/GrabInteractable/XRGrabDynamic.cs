using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabDynamic : XRGrabInteractable
{
    public Transform rightAttach;
    public Transform leftAttach;
    public bool rightHandGrabbing;
    public bool leftHandGrabbing;
    public bool isGrabbing;
    public ControllerInteractors leftController;
    public ControllerInteractors rightController;
    public Transform leftPresence;
    public Transform rightPresence;
    public Vector3 normalOffset;
    public Vector3 surfaceClamp;
    private bool isHovering;
    [Header("For NPC's Only:")]

    public PuppetMaster puppetMaster;
    public BehaviourPuppet puppetFall;
    public float collisionResistanceGrabbing;
    public float knockOutDistanceGrabbing;

    private float collisionResistance;
    private float knockOutDistance;
    private void Start()
    {
        if (puppetFall)
        {
            collisionResistance = puppetFall.collisionResistance.floatValue;
            knockOutDistance = puppetFall.defaults.knockOutDistance;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!rightController.isGrabbing && isHovering)
        {
            Vector3 directionRight = transform.position - rightPresence.gameObject.transform.position;
            RaycastHit rightHit;
            RaycastHit rightHit2;
            if (!GetComponent<Collider>())
            {
                GetComponentInChildren<Collider>().Raycast(new Ray(rightPresence.position, directionRight), out rightHit, float.PositiveInfinity);
                GetComponentInChildren<Collider>().Raycast(new Ray(rightPresence.parent.position, transform.position - rightPresence.parent.position), out rightHit2, float.PositiveInfinity);
            }
            else
            {
                GetComponent<Collider>().Raycast(new Ray(rightPresence.position, directionRight), out rightHit, float.PositiveInfinity);
                GetComponent<Collider>().Raycast(new Ray(rightPresence.parent.position, transform.position - rightPresence.parent.position), out rightHit2, float.PositiveInfinity);
            }
            if (rightHit.collider)
            {
                Vector3 positionRight;
                if (!rightHit.transform.CompareTag("Pierceable"))
                {
                    positionRight = rightHit.collider.ClosestPoint(rightPresence.position);
                }
                else
                {
                    positionRight = rightHit.collider.ClosestPoint(rightPresence.position);
                }
                rightAttach.position = positionRight;
                Vector3 localPositionRight = rightAttach.localPosition;

                localPositionRight.x = Mathf.Clamp(localPositionRight.x, -surfaceClamp.x, surfaceClamp.x);
                localPositionRight.y = Mathf.Clamp(localPositionRight.y, -surfaceClamp.y, surfaceClamp.y);
                localPositionRight.z = Mathf.Clamp(localPositionRight.z, -surfaceClamp.z, surfaceClamp.z);
                localPositionRight += transform.InverseTransformDirection(new Vector3(rightHit2.normal.x * normalOffset.x, rightHit2.normal.y * normalOffset.y, rightHit2.normal.z * normalOffset.z));

                rightAttach.localPosition = localPositionRight;
                rightAttach.rotation = Quaternion.LookRotation(-rightHit2.normal, rightPresence.up) * Quaternion.Euler(0, 90, 0);
            }
        }

        if (!leftController.isGrabbing && isHovering)
        {
            Vector3 directionLeft = transform.position - leftPresence.gameObject.transform.position;
            RaycastHit leftHit;
            RaycastHit leftHit2;
            if (!GetComponent<Collider>())
            {
                GetComponentInChildren<Collider>().Raycast(new Ray(leftPresence.position, directionLeft), out leftHit, float.PositiveInfinity);
                GetComponentInChildren<Collider>().Raycast(new Ray(leftPresence.parent.position, transform.position - leftPresence.parent.position), out leftHit2, float.PositiveInfinity);
            }
            else
            {
                GetComponent<Collider>().Raycast(new Ray(leftPresence.position, directionLeft), out leftHit, float.PositiveInfinity);
                GetComponent<Collider>().Raycast(new Ray(leftPresence.parent.position, transform.position - leftPresence.parent.position), out leftHit2, float.PositiveInfinity);
            }
            if (leftHit.collider)
            {
                Vector3 positionLeft;
                if (!leftHit.transform.CompareTag("Pierceable"))
                {
                    positionLeft = leftHit.collider.ClosestPoint(leftPresence.position);
                }
                else
                {
                    positionLeft = leftHit.collider.ClosestPoint(leftPresence.position);
                }
                leftAttach.position = positionLeft;
                Vector3 localPositionLeft = leftAttach.localPosition;

                localPositionLeft.x = Mathf.Clamp(localPositionLeft.x, -surfaceClamp.x, surfaceClamp.x);
                localPositionLeft.y = Mathf.Clamp(localPositionLeft.y, -surfaceClamp.y, surfaceClamp.y);
                localPositionLeft.z = Mathf.Clamp(localPositionLeft.z, -surfaceClamp.z, surfaceClamp.z);
                localPositionLeft -= transform.InverseTransformDirection(-new Vector3(leftHit2.normal.x * normalOffset.x, leftHit2.normal.y * normalOffset.y, leftHit2.normal.z * normalOffset.z));

                leftAttach.localPosition = localPositionLeft;
                leftAttach.rotation = Quaternion.LookRotation(leftHit2.normal, leftPresence.up) * Quaternion.Euler(0, 90, 0);
            }
        }
        if(leftHandGrabbing)
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
            puppetMaster.muscles[7].props.mappingWeight *= 4;
            puppetMaster.muscleSpring /= 2;
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
            GetComponent<Rigidbody>().mass /= 2;
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
            GetComponent<Rigidbody>().mass *= 2;
            isGrabbing = true;
        }
    }
}
