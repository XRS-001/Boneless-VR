using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractableRifle : XRGrabInteractable
{
    public XRGrabJoint slideGrab;
    private RifleFire rifleFire;
    public Collider slideCollider;
    public bool isGrabbing { get; private set; } = false;
    public bool secondHandGrabbing { get; private set; }
    public bool rightHandGrabbing = false;
    public bool leftHandGrabbing = false;
    public Transform rightAttach;
    public Transform leftAttach;
    public Collider secondHandGrabPointCollider;
    public Transform rightAttachSecond;
    public Transform leftAttachSecond;
    public XRSimpleInteractable secondHandGrabPoint;
    public XRBaseInteractor secondInteractor;
    public XRBaseInteractor interactor;
    public enum TwoHandRotationType { None, First, Second };
    public TwoHandRotationType twoHandRotationType;
    public enum dynamicAxisEnum { dynamicX, dynamicY, dynamicZ }
    public dynamicAxisEnum dynamicAxis;
    public bool snapToSecondHand = true;
    private Quaternion initialRotationOffset;
    public Transform leftPresence;
    public Transform rightPresence;
    public float handleLength;
    public bool offHandGrabbing = false;
    private void FixedUpdate()
    {
        if (!leftHandGrabbing && !secondHandGrabbing)
        {
            Vector3 originalPosition = leftAttachSecond.localPosition;
            Vector3 position = leftPresence.position;
            leftAttachSecond.position = position;
            Vector3 localPosition = leftAttachSecond.localPosition;
            if (dynamicAxis != dynamicAxisEnum.dynamicX)
            {
                localPosition.x = originalPosition.x;
            }
            else
            {
                localPosition.x = Mathf.Clamp(localPosition.x, -handleLength, handleLength);
            }
            if (dynamicAxis != dynamicAxisEnum.dynamicY)
            {
                localPosition.y = originalPosition.y;
            }
            else
            {
                localPosition.y = Mathf.Clamp(localPosition.y, -handleLength, handleLength);
            }
            if (dynamicAxis != dynamicAxisEnum.dynamicZ)
            {
                localPosition.z = originalPosition.z;
            }
            else
            {
                localPosition.z = Mathf.Clamp(localPosition.z, -handleLength, handleLength);
            }
            leftAttachSecond.localPosition = localPosition;
        }
        if (!rightHandGrabbing && !secondHandGrabbing)
        {
            Vector3 originalPosition = rightAttachSecond.localPosition;
            Vector3 position = rightPresence.position;
            rightAttachSecond.position = position;
            Vector3 localPosition = rightAttachSecond.localPosition;
            if (dynamicAxis != dynamicAxisEnum.dynamicX)
            {
                localPosition.x = originalPosition.x;
            }
            else
            {
                localPosition.x = Mathf.Clamp(localPosition.x, -handleLength, handleLength);
            }
            if (dynamicAxis != dynamicAxisEnum.dynamicY)
            {
                localPosition.y = originalPosition.y;
            }
            else
            {
                localPosition.y = Mathf.Clamp(localPosition.y, -handleLength, handleLength);
            }
            if (dynamicAxis != dynamicAxisEnum.dynamicZ)
            {
                localPosition.z = originalPosition.z;
            }
            else
            {
                localPosition.z = Mathf.Clamp(localPosition.z, -handleLength, handleLength);
            }
            rightAttachSecond.localPosition = localPosition;
        }
        if (!rightHandGrabbing || !leftHandGrabbing && interactor)
        {
            try
            {
                interactor.attachTransform.rotation = interactor.transform.rotation;
            }
            catch { }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        slideGrab.enabled = false;
        rifleFire = GetComponent<RifleFire>();
        secondHandGrabPoint.selectEntered.AddListener(OnSecondHandGrab);
        secondHandGrabPoint.selectExited.AddListener(OnSecondHandRelease);
    }
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if (secondInteractor && selectingInteractor)
        {
            if (rightHandGrabbing)
            {
                secondInteractor.GetComponent<ControllerInteractors>().handPhysics.transform.position = leftAttachSecond.position;
                secondInteractor.GetComponent<ControllerInteractors>().handPhysics.transform.rotation = leftAttachSecond.rotation;
            }
            if (leftHandGrabbing)
            {
                secondInteractor.GetComponent<ControllerInteractors>().handPhysics.transform.position = rightAttachSecond.position;
                secondInteractor.GetComponent<ControllerInteractors>().handPhysics.transform.rotation = rightAttachSecond.rotation;
            }
            if (snapToSecondHand)
                selectingInteractor.GetComponent<ControllerInteractors>().handPhysics.GetComponent<Rigidbody>().MoveRotation(GetTwoHandRotation());
            else
                selectingInteractor.GetComponent<ControllerInteractors>().handPhysics.GetComponent<Rigidbody>().MoveRotation(GetTwoHandRotation() * initialRotationOffset);
        }
        base.ProcessInteractable(updatePhase);
    }
    public Quaternion GetTwoHandRotation()
    {
        Quaternion targetRotation;
        if (twoHandRotationType == TwoHandRotationType.None)
        {
            targetRotation = Quaternion.LookRotation(secondInteractor.attachTransform.position - interactor.attachTransform.position);
        }
        else if (twoHandRotationType == TwoHandRotationType.First)
        {
            targetRotation = Quaternion.LookRotation(secondInteractor.attachTransform.position - interactor.attachTransform.position, interactor.transform.up);
        }
        else
        {
            targetRotation = Quaternion.LookRotation(secondInteractor.attachTransform.position - interactor.attachTransform.position, secondInteractor.attachTransform.up);
        }
        return targetRotation;
    }
    public void OnSecondHandGrab(SelectEnterEventArgs args)
    {
        secondHandGrabbing = true;
        if (rifleFire != null)
        {
            rifleFire.recoilSpeed /= 2;
        }
        secondInteractor = args.interactorObject.transform.GetComponent<ControllerInteractors>();
        secondInteractor.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().handColliderParent.SetActive(false);
        foreach (Collider collider in colliders)
        {
            Physics.IgnoreCollision(collider, secondInteractor.GetComponent<ControllerInteractors>().forearmCollider, true);
        }
        initialRotationOffset = Quaternion.Inverse(GetTwoHandRotation()) * interactor.attachTransform.rotation;
    }
    public void OnSecondHandRelease(SelectExitEventArgs args)
    {
        if(!offHandGrabbing)
        {
            secondHandGrabbing = false;
        }
        if (rifleFire != null)
        {
            rifleFire.recoilSpeed *= 2;
        }
        foreach (Collider collider in colliders)
        {
            Physics.IgnoreCollision(collider, secondInteractor.GetComponent<ControllerInteractors>().forearmCollider, false);
        }
        StartCoroutine(Delay());
    }
    public override bool IsSelectableBy(IXRSelectInteractor selectInteractor)
    {
        if (!offHandGrabbing)
        {
            bool isAlreadyGrabbed = selectingInteractor && !selectInteractor.Equals(selectingInteractor);
            return base.IsSelectableBy(selectInteractor) && !isAlreadyGrabbed;
        }
        else
        {
            return base.IsSelectableBy(selectInteractor);
        }
    }
    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.05f);
        GameObject handColliderParent = secondInteractor.gameObject.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().handColliderParent;
        secondInteractor = null;
        yield return new WaitForSeconds(0.5f);
        handColliderParent.SetActive(true);
    }
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (!isGrabbing)
        {
            offHandGrabbing = false;
        }
        if (args.interactorObject.transform.CompareTag("LeftHand"))
        {
            attachTransform = leftAttach;
        }
        else if (args.interactorObject.transform.CompareTag("RightHand"))
        {
            attachTransform = rightAttach;
        }
        base.OnHoverEntered(args);
    }
    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        if (offHandGrabbing && !secondHandGrabbing)
        {
            XRInteractionManager XRInteractionManager = interactionManager;
            secondHandGrabPoint.enabled = true;
            secondHandGrabPointCollider.enabled = true;
            XRInteractionManager.SelectEnter(interactor, secondHandGrabPoint);
            interactor = secondInteractor;
            interactor = selectingInteractor;
            offHandGrabbing = false;
        }
        isGrabbing = true;
        base.OnSelectEntering(args);
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        slideGrab.enabled = true;
        isGrabbing = true;
        interactor = selectingInteractor;
        secondHandGrabPoint.enabled = true;
        secondHandGrabPointCollider.enabled = true;
        if (args.interactorObject.transform.CompareTag("RightHand"))
        {
            rightHandGrabbing = true;
            leftHandGrabbing = false;
        }
        else
        {
            leftHandGrabbing = true;
            rightHandGrabbing = false;
        }
        base.OnSelectEntered(args);
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        secondHandGrabPointCollider.enabled = false;
        base.OnSelectExited(args);
        if (secondHandGrabbing)
        {
            offHandGrabbing = true;
            XRGrabInteractableRifle interactable = GetComponent<XRGrabInteractableRifle>();
            XRInteractionManager XRInteractionManager = interactionManager;
            interactor = secondInteractor;
            secondInteractor = interactor;
            if (secondInteractor.transform.CompareTag("LeftHand"))
            {
                attachTransform = leftAttachSecond;
            }
            if (secondInteractor.transform.CompareTag("RightHand"))
            {
                attachTransform = rightAttachSecond;
            }
            XRInteractionManager.SelectExit(interactor, secondHandGrabPoint);

            HandData leftHandRig = GetComponent<GrabHandPose>().leftHandPose;
            HandData rightHandRig = GetComponent<GrabHandPose>().rightHandPose;

            GetComponent<GrabHandPose>().leftHandPose = secondHandGrabPoint.GetComponent<GrabHandPose>().leftHandPose;
            GetComponent<GrabHandPose>().rightHandPose = secondHandGrabPoint.GetComponent<GrabHandPose>().rightHandPose;

            XRInteractionManager.SelectEnter(interactor, interactable);

            GetComponent<GrabHandPose>().leftHandPose = leftHandRig;
            GetComponent<GrabHandPose>().rightHandPose = rightHandRig;
            secondHandGrabbing = false;
            secondHandGrabPointCollider.enabled = false;
            secondInteractor = null;
        }
        else
        {
            slideGrab.enabled = false;
            isGrabbing = false;
            secondInteractor = null;
            rightHandGrabbing = false;
            leftHandGrabbing = false;
        }
        if (!rightHandGrabbing && !leftHandGrabbing)
        {
            secondHandGrabPoint.enabled = false;
        }
    }
}

