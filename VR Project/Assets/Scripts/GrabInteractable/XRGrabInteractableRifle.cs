using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractableRifle : XRGrabInteractable
{
    private RifleFire rifleFire;
    public bool isGrabbing { get; private set; } = false;
    private bool secondHandGrabbing;
    public Collider slideCollider;
    private Collider[] handColliders;
    private Collider[] previousHandColliders;
    public bool rightHandGrabbing = false;
    public bool leftHandGrabbing = false;
    public Transform rightAttach;
    public Transform leftAttach;
    public Collider secondHandGrabPointCollider;
    public Transform rightAttachSecond;
    public Transform leftAttachSecond;
    public XRSimpleInteractable secondHandGrabPoint;
    private XRBaseInteractor secondInteractor;
    private Quaternion attachInitialRotation;
    private XRBaseInteractor interactor;
    public enum TwoHandRotationType { None, First, Second };
    public TwoHandRotationType twoHandRotationType;
    public bool snapToSecondHand = true;
    private Quaternion initialRotationOffset;
    public Transform leftPresence;
    public Transform rightPresence;
    public bool dynamicY;
    public bool dynamicX;
    public bool dynamicZ;
    public float handleLength;
    private void Update()
    {
        if (!leftHandGrabbing && !secondHandGrabbing)
        {
            Vector3 originalPosition = leftAttachSecond.localPosition;
            Vector3 position = leftPresence.position;
            leftAttachSecond.position = position;
            Vector3 localPosition = leftAttachSecond.localPosition;
            if (!dynamicX)
            {
                localPosition.x = originalPosition.x;
            }
            else
            {
                localPosition.x = Mathf.Clamp(localPosition.x, -handleLength, handleLength);
            }
            if (!dynamicY)
            {
                localPosition.y = originalPosition.y;
            }
            else
            {
                localPosition.y = Mathf.Clamp(localPosition.y, -handleLength, handleLength);
            }
            if (!dynamicZ)
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
            if (!dynamicX)
            {
                localPosition.x = originalPosition.x;
            }
            else
            {
                localPosition.x = Mathf.Clamp(localPosition.x, -handleLength, handleLength);
            }
            if (!dynamicY)
            {
                localPosition.y = originalPosition.y;
            }
            else
            {
                localPosition.y = Mathf.Clamp(localPosition.y, -handleLength, handleLength);
            }
            if (!dynamicZ)
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
                secondInteractor.GetComponent<ControllerInteractors>().handPresence.transform.position = leftAttachSecond.position;
                secondInteractor.GetComponent<ControllerInteractors>().handPresence.transform.rotation = leftAttachSecond.rotation;
            }
            if (leftHandGrabbing)
            {
                secondInteractor.GetComponent<ControllerInteractors>().handPresence.transform.position = rightAttachSecond.position;
                secondInteractor.GetComponent<ControllerInteractors>().handPresence.transform.rotation = rightAttachSecond.rotation;
            }
            if (snapToSecondHand)
                selectingInteractor.GetComponent<ControllerInteractors>().handPhysics.transform.rotation = GetTwoHandRotation();
            else
                selectingInteractor.GetComponent<ControllerInteractors>().handPhysics.transform.rotation = GetTwoHandRotation() * initialRotationOffset;
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
        rifleFire.recoilSpeed /= 2;
        secondInteractor = args.interactorObject.transform.GetComponent<ControllerInteractors>();
        secondInteractor.GetComponent<ControllerInteractors>().bodyRb.isKinematic = true;
        secondInteractor.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().handColliderParent.SetActive(false);
        foreach (Collider collider in colliders)
        {
            Physics.IgnoreCollision(collider, secondInteractor.GetComponent<ControllerInteractors>().forearmCollider, true);
        }
        initialRotationOffset = Quaternion.Inverse(GetTwoHandRotation()) * interactor.attachTransform.rotation;
    }
    public void OnSecondHandRelease(SelectExitEventArgs args)
    {
        secondHandGrabbing = false;
        secondInteractor.GetComponent<ControllerInteractors>().bodyRb.isKinematic = false;
        rifleFire.recoilSpeed *= 2;
        secondInteractor.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().handColliderParent.SetActive(true);
        foreach (Collider collider in colliders)
        {
            Physics.IgnoreCollision(collider, secondInteractor.GetComponent<ControllerInteractors>().forearmCollider, false);
        }
        secondInteractor = null;
    }
    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        bool isAlreadyGrabbed = selectingInteractor && !interactor.Equals(selectingInteractor);
        return base.IsSelectableBy(interactor) && !isAlreadyGrabbed;
    }
    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f);

        secondInteractor = null;
    }
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
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
        isGrabbing = true;
        handColliders = args.interactorObject.transform.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().handColliders;
        foreach (Collider collider in handColliders)
        {
            Physics.IgnoreCollision(collider, slideCollider, true);
        }
        base.OnSelectEntering(args);
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        interactor = selectingInteractor;
        secondHandGrabPointCollider.enabled = true;
        attachInitialRotation = args.interactorObject.transform.GetComponent<ControllerInteractors>().attachTransform.localRotation;
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
        secondHandGrabPoint.enabled = false;
        secondHandGrabPoint.enabled = true;
        secondHandGrabPointCollider.enabled = false;
        StartCoroutine(Delay());
        rightHandGrabbing = false;
        leftHandGrabbing = false;
        isGrabbing = false;
        StartCoroutine(DelaySetActive());
        args.interactorObject.transform.GetComponent<ControllerInteractors>().attachTransform.localRotation = attachInitialRotation;
        previousHandColliders = handColliders;
        base.OnSelectExited(args);
    }
    IEnumerator DelaySetActive()
    {
        yield return new WaitForSeconds(0.1f);

        if (!leftHandGrabbing && !rightHandGrabbing)
        {
            foreach (Collider collider in handColliders)
            {
                Physics.IgnoreCollision(collider, slideCollider, false);
            }
        }
        else
        {
            foreach (Collider collider in previousHandColliders)
            {
                Physics.IgnoreCollision(collider, slideCollider, false);
            }
        }
    }
}

