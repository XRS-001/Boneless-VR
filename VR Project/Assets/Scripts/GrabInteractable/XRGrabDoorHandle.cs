using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabDoorHandle : XRGrabJoint
{
    public GrabHandPose grabHandPoseDoor;
    public Transform leftAttachPrimary;
    public Transform rightAttachPrimary;
    public Transform leftAttachAlt;
    public Transform rightAttachAlt;
    public HandData leftAttachPrimaryPose;
    public HandData rightAttachPrimaryPose;
    private Transform leftAttachTransform;
    private Transform rightAttachTransform;
    private Coroutine leftHandCoroutine;
    private Coroutine rightHandCoroutine;
    private float threshold = 2f;
    public Transform target;
    public GameObject door;
    public Transform doorTarget;
    public bool locked = true;
    public float distance;
    public float doorDistance;
    private AudioSource audioSource;
    public AudioClip handleClick;
    private bool hasClicked;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void FixedUpdate()
    {
        distance = Quaternion.Angle(transform.rotation, target.rotation);
        doorDistance = Quaternion.Angle(door.transform.rotation, doorTarget.rotation);
        if (distance < threshold && locked && hasClicked == false)
        {
            locked = false;
            if(!audioSource.isPlaying) 
            {
                audioSource.PlayOneShot(handleClick);
            }
            hasClicked = true;
        }
        if (distance >= threshold && doorDistance < threshold)
        {
            locked = true;
            hasClicked = false;
        }
        if(locked == true)
        {
            door.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            door.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

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
        foreach (Collider handCollider in args.interactorObject.transform.GetComponent<ControllerInteractors>().colliders)
        {
            handCollider.enabled = false;
        }
        Rigidbody VRRig = GameObject.Find("XR Origin").GetComponent<Rigidbody>();
        VRRig.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        Transform interactorTransform = args.interactorObject.transform;
        if (interactorTransform.CompareTag("LeftHand"))
        {
            if (leftAttachTransform != null)
            {
                attachTransform = leftAttachTransform;
                grabHandPoseDoor.leftHandPose = leftAttachTransform == leftAttachPrimary ? leftAttachPrimaryPose : leftAttachPrimaryPose;
            }
        }
        else if (interactorTransform.CompareTag("RightHand"))
        {
            if (rightAttachTransform != null)
            {
                attachTransform = rightAttachTransform;
                grabHandPoseDoor.rightHandPose = rightAttachTransform == rightAttachPrimary ? rightAttachPrimaryPose : rightAttachPrimaryPose;
            }
        }
        base.OnSelectEntering(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        Rigidbody VRRig = GameObject.Find("XR Origin").GetComponent<Rigidbody>();
        VRRig.constraints = RigidbodyConstraints.FreezeRotation;
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
                    grabHandPoseDoor.leftHandPose = leftAttachPrimaryPose;
                }
                else
                {
                    rightAttachTransform = primaryTransform;
                    grabHandPoseDoor.rightHandPose = rightAttachPrimaryPose;
                }
            }
            else
            {
                if (isLeftHand)
                {
                    leftAttachTransform = altTransform;
                    grabHandPoseDoor.leftHandPose = leftAttachPrimaryPose;
                }
                else
                {
                    rightAttachTransform = altTransform;
                    grabHandPoseDoor.rightHandPose = rightAttachPrimaryPose;
                }
            }

            yield return null;
        }
    }
}
