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
        if (!rightController.isGrabbing)
        {
            Vector3 directionRight = transform.position - rightPresence.gameObject.transform.position;
            RaycastHit rightHit;
            if (!GetComponent<Collider>())
            {
                GetComponentInChildren<Collider>().Raycast(new Ray(rightPresence.position, directionRight), out rightHit, float.PositiveInfinity);
            }
            else
            {
                GetComponent<Collider>().Raycast(new Ray(rightPresence.position, directionRight), out rightHit, float.PositiveInfinity);
            }
            if (rightHit.collider)
            {
                Vector3 positionRight = rightHit.collider.ClosestPoint(rightPresence.position) + (rightPresence.right / 25);
                rightAttach.position = positionRight;
                Vector3 localPositionRight = rightAttach.localPosition;
                rightAttach.localPosition = localPositionRight;
                rightAttach.rotation = rightPresence.rotation;
            }
        }

        if (!leftController.isGrabbing)
        {
            Vector3 directionLeft = transform.position - leftPresence.gameObject.transform.position;
            RaycastHit leftHit;
            if (!GetComponent<Collider>())
            {
                GetComponentInChildren<Collider>().Raycast(new Ray(leftPresence.position, directionLeft), out leftHit, float.PositiveInfinity);
            }
            else
            {
                GetComponent<Collider>().Raycast(new Ray(leftPresence.position, directionLeft), out leftHit, float.PositiveInfinity);
            }
            if (leftHit.collider)
            {
                Vector3 positionLeft = leftHit.collider.ClosestPoint(leftPresence.position) - (leftPresence.right / 25);
                leftAttach.position = positionLeft;
                Vector3 localPositionLeft = leftAttach.localPosition;
                leftAttach.localPosition = localPositionLeft;
                leftAttach.rotation = leftPresence.rotation;
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
            puppetMaster.angularLimits = true;
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
        if (isGrabbing)
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
            puppetMaster.angularLimits = false;
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
