using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabJoint : XRGrabInteractable
{
    public bool isGrabbing;
    public Collider[] parentColliders;
    private Collider[] handColliders;
    private Collider[] previousHandColliders;
    public Transform rightAttach;
    public Transform leftAttach;
    public bool rightHandGrabbing;
    public bool leftHandGrabbing;
    [Header("For Dynamic Attaching:")]
    public bool dynamic;
    public enum dynamicAxis { x, y , z}
    public dynamicAxis axis;
    public float angleLeft;
    public float angleRight;
    public Transform leftHandPresence;
    public Transform rightHandPresence;
    private Transform attachRight;
    private Transform attachLeft;
    public Transform altAttachRight;
    public Transform altAttachLeft;
    public HandData altRightPose;
    public HandData altLeftPose;
    private HandData rightPose;
    private HandData leftPose;
    private GrabHandPose grabHandPose;
    private bool isHovering;
    private ControllerInteractors controllerGrabbing;
    private void Start()
    {
        grabHandPose = GetComponent<GrabHandPose>();
        rightPose = grabHandPose.rightHandPose;
        leftPose = grabHandPose.leftHandPose;
        attachRight = rightAttach;
        attachLeft = leftAttach;
    }
    private void Update()
    {
        if(dynamic && !isGrabbing)
        {
            switch(axis)
            {
                case dynamicAxis.x:
                    angleLeft = leftHandPresence.rotation.x - attachLeft.transform.rotation.x;
                    angleRight = rightHandPresence.rotation.x - attachRight.transform.rotation.x;
                    if(angleLeft > 0.3f) 
                    {
                        leftAttach = altAttachLeft;
                        grabHandPose.leftHandPose = altLeftPose;
                    }
                    else
                    {
                        leftAttach = attachLeft;
                        grabHandPose.leftHandPose = leftPose;
                    }
                    if (angleRight > 0.3f)
                    {
                        rightAttach = altAttachRight;
                        grabHandPose.rightHandPose = altRightPose;
                    }
                    else
                    {
                        rightAttach = attachRight;
                        grabHandPose.rightHandPose = rightPose;
                    }
                    break;

                case dynamicAxis.y:
                    angleLeft = leftHandPresence.rotation.x - attachLeft.transform.rotation.x;
                    angleRight = rightHandPresence.rotation.x - attachRight.transform.rotation.x;
                    if (angleLeft > 0.3f)
                    {
                        leftAttach = altAttachLeft;
                        grabHandPose.leftHandPose = altLeftPose;
                    }
                    else
                    {
                        leftAttach = attachLeft;
                        grabHandPose.leftHandPose = leftPose;
                    }
                    if (angleRight > 0.3f)
                    {
                        rightAttach = altAttachRight;
                        grabHandPose.rightHandPose = altRightPose;
                    }
                    else
                    {
                        rightAttach = attachRight;
                        grabHandPose.rightHandPose = rightPose;
                    }
                    break;

                case dynamicAxis.z:
                    angleLeft = leftHandPresence.rotation.x - attachLeft.transform.rotation.x;
                    angleRight = rightHandPresence.rotation.x - attachRight.transform.rotation.x;
                    if (angleLeft > 0.3f)
                    {
                        leftAttach = altAttachLeft;
                        grabHandPose.leftHandPose = altLeftPose;
                    }
                    else
                    {
                        leftAttach = attachLeft;
                        grabHandPose.leftHandPose = leftPose;
                    }
                    if (angleRight > 0.3f)
                    {
                        rightAttach = altAttachRight;
                        grabHandPose.rightHandPose = altRightPose;
                    }
                    else
                    {
                        rightAttach = attachRight;
                        grabHandPose.rightHandPose = rightPose;
                    }
                    break;
            }
            if(isHovering)
            {
                if (controllerGrabbing.CompareTag("LeftHand"))
                {
                    attachTransform = leftAttach;
                }
                else if (controllerGrabbing.CompareTag("RightHand"))
                {
                    attachTransform = rightAttach;
                }
            }
        }
    }
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        isHovering = true;
        controllerGrabbing = args.interactorObject.transform.GetComponent<ControllerInteractors>();
        if (!dynamic)
        {
            if (controllerGrabbing.CompareTag("LeftHand"))
            {
                attachTransform = leftAttach;
            }
            else if (controllerGrabbing.CompareTag("RightHand"))
            {
                attachTransform = rightAttach;
            }
        }
        base.OnHoverEntered(args);
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        foreach(Collider collider in parentColliders) 
        {
            Physics.IgnoreCollision(collider, args.interactorObject.transform.GetComponent<ControllerInteractors>().forearmCollider, true);
        }
        if(dynamic)
        {
            if (args.interactorObject.transform.CompareTag("RightHand"))
            {
                leftHandGrabbing = false;
                rightHandGrabbing = true;
                foreach (Collider collider in leftHandPresence.GetComponent<HandPresencePhysics>().handColliders)
                {
                    collider.isTrigger = true;
                }
            }
            else
            {
                rightHandGrabbing = false;
                leftHandGrabbing = true;
                foreach (Collider collider in rightHandPresence.GetComponent<HandPresencePhysics>().handColliders)
                {
                    collider.isTrigger = true;
                }
            }
        }
        isGrabbing = true;
        args.interactorObject.transform.GetComponent<ControllerInteractors>().handPresence.transform.position = attachTransform.position;
        args.interactorObject.transform.GetComponent<ControllerInteractors>().handPresence.transform.rotation = attachTransform.rotation;
        args.interactorObject.transform.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().target = attachTransform;
        args.interactorObject.transform.GetComponent<ControllerInteractors>().bodyRb.isKinematic = true;
        base.OnSelectEntered(args);
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        foreach (Collider collider in parentColliders)
        {
            Physics.IgnoreCollision(collider, args.interactorObject.transform.GetComponent<ControllerInteractors>().forearmCollider, false);
        }
        if (dynamic)
        {
            if (args.interactorObject.transform.CompareTag("RightHand"))
            {
                foreach (Collider collider in leftHandPresence.GetComponent<HandPresencePhysics>().handColliders)
                {
                    collider.isTrigger = false;
                }
            }
            else
            {
                foreach (Collider collider in rightHandPresence.GetComponent<HandPresencePhysics>().handColliders)
                {
                    collider.isTrigger = false;
                }
            }
        }
        previousHandColliders = handColliders;
        isGrabbing = false;
        StartCoroutine(Delay());
        args.interactorObject.transform.GetComponent<ControllerInteractors>().handPresence.transform.position = args.interactorObject.transform.GetComponent<ControllerInteractors>().transform.position;
        args.interactorObject.transform.GetComponent<ControllerInteractors>().handPresence.transform.rotation = args.interactorObject.transform.GetComponent<ControllerInteractors>().transform.rotation;
        args.interactorObject.transform.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().target
            = args.interactorObject.transform.GetComponent<ControllerInteractors>().handPhysics.transform;
        args.interactorObject.transform.GetComponent<ControllerInteractors>().bodyRb.isKinematic = false;
        base.OnSelectExited(args);
    }
    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.5f);

        if (!leftHandGrabbing && !rightHandGrabbing && handColliders != null)
        {
            foreach (Collider collider in handColliders)
            {
                collider.gameObject.SetActive(true);
            }
        }
        else if (previousHandColliders != null)
        {
            foreach (Collider collider in previousHandColliders)
            {
                collider.gameObject.SetActive(true);
            }
        }
    }
}