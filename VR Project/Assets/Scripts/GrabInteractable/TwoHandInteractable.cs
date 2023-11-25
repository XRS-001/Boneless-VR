using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoHandInteractable : XRGrabInteractableTwoAttach
{
    public Collider secondHandGrabPointCollider;
    public XRSimpleInteractable secondHandGrabPoint;
    private XRBaseInteractor secondInteractor;
    private Quaternion attachInitialRotation; 
    public enum TwoHandRotationType { None,First,Second};
    public TwoHandRotationType twoHandRotationType;
    public bool snapToSecondHand = true;
    private Quaternion initialRotationOffset;
    private XRBaseInteractor interactor;
    bool secondHandGrabbing;
    private Rigidbody rb;
    public Transform leftPresence;
    public Transform rightPresence;
    public enum dynamicAxisEnum {dynamicX, dynamicY, dynamicZ }
    public dynamicAxisEnum dynamicAxis;
    public float handleLength;
    private Collider[] previousColliders;
    // Start is called before the first frame update
    void Start()
    {
        secondHandGrabPoint.selectEntered.AddListener(OnSecondHandGrab);
        secondHandGrabPoint.selectExited.AddListener(OnSecondHandRelease);
    }

    // Update is called once per frame
    void Update()
    {
        if (!leftHandGrabbing && !secondHandGrabbing)
        {
            Vector3 originalPosition = leftAttach.localPosition;
            Vector3 position = leftPresence.position;
            leftAttach.position = position;
            Vector3 localPosition = leftAttach.localPosition;
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
            leftAttach.localPosition = localPosition;
        }
        if (!rightHandGrabbing && !secondHandGrabbing)
        {
            Vector3 originalPosition = rightAttach.localPosition;
            Vector3 position = rightPresence.position;
            rightAttach.position = position;
            Vector3 localPosition = rightAttach.localPosition;
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
            rightAttach.localPosition = localPosition;
        }
        if (secondHandGrabbing == false && interactor)
        {
            try
            {
                interactor.attachTransform.rotation = interactor.transform.rotation;
            }
            catch { }
        }
    }
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if(secondInteractor && interactor)
        {
            if (rightHandGrabbing)
            {
                secondInteractor.GetComponent<ControllerInteractors>().handPhysics.transform.position = leftAttach.position;
                secondInteractor.GetComponent<ControllerInteractors>().handPhysics.transform.rotation = leftAttach.rotation;
            }
            if (leftHandGrabbing)
            {
                secondInteractor.GetComponent<ControllerInteractors>().handPhysics.transform.position = rightAttach.position;
                secondInteractor.GetComponent<ControllerInteractors>().handPhysics.transform.rotation = rightAttach.rotation;
            }
            if (snapToSecondHand)
                interactor.GetComponent<ControllerInteractors>().handPhysics.transform.rotation = GetTwoHandRotation();
            else
                interactor.GetComponent<ControllerInteractors>().handPhysics.transform.rotation = GetTwoHandRotation() * initialRotationOffset;
        }
        base.ProcessInteractable(updatePhase);
    }
    public Quaternion GetTwoHandRotation()
    {
        Quaternion targetRotation;
        if(twoHandRotationType == TwoHandRotationType.None) 
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
        secondInteractor = args.interactorObject.transform.GetComponent<ControllerInteractors>();
        secondInteractor.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().handColliderParent.SetActive(false);
        initialRotationOffset = Quaternion.Inverse(GetTwoHandRotation()) * interactor.attachTransform.rotation;
        foreach(Collider collider in colliders)
        {
            Physics.IgnoreCollision(collider, secondInteractor.GetComponent<ControllerInteractors>().forearmCollider, true);
        }
        StartCoroutine(DelayKinematic(args.interactorObject.transform.GetComponent<ControllerInteractors>().bodyRb));
    }
    public void OnSecondHandRelease(SelectExitEventArgs args)
    {
        secondHandGrabbing = false;
        secondInteractor.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().handColliderParent.SetActive(true);
        StartCoroutine(Delay());
        foreach (Collider collider in colliders)
        {
            Physics.IgnoreCollision(collider, secondInteractor.GetComponent<ControllerInteractors>().forearmCollider, false);
        }
        StartCoroutine(DelayKinematic(args.interactorObject.transform.GetComponent<ControllerInteractors>().bodyRb));
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        interactor = selectingInteractor;
        secondHandGrabPointCollider.enabled = true;
        secondHandGrabPoint.enabled = true;
        base.OnSelectEntered(args);
        attachInitialRotation = args.interactorObject.transform.GetComponent<ControllerInteractors>().attachTransform.localRotation;
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        secondHandGrabPointCollider.enabled = false;
        base.OnSelectExited(args);
        if (secondHandGrabbing)
        {
            TwoHandInteractable interactable = GetComponent<TwoHandInteractable>();
            XRInteractionManager XRInteractionManager = interactionManager;
            interactor = secondInteractor;
            secondInteractor = interactor;
            previousColliders = secondInteractor.GetComponent<ControllerInteractors>().colliders;
            StartCoroutine(DelayColliders());
            if (secondInteractor.transform.CompareTag("LeftHand"))
            {
                GetComponent<TwoHandInteractable>().attachTransform = leftAttach;
            }
            if (secondInteractor.transform.CompareTag("RightHand"))
            {
                GetComponent<TwoHandInteractable>().attachTransform = rightAttach;
            }
            XRInteractionManager.SelectExit(interactor, secondHandGrabPoint);
            XRInteractionManager.SelectEnter(interactor, interactable);
            initialRotationOffset = Quaternion.Inverse(GetTwoHandRotation()) * interactor.transform.GetComponent<ControllerInteractors>().attachTransform.rotation;
        }
        else
        {
            rightHandGrabbing = false;
            leftHandGrabbing = false;
        }
        if (!rightHandGrabbing && !leftHandGrabbing)
        {
            secondHandGrabPoint.enabled = false;
        }
        args.interactorObject.transform.GetComponent<ControllerInteractors>().attachTransform.localRotation = attachInitialRotation;
    }
    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        bool isAlreadyGrabbed = selectingInteractor && !interactor.Equals(selectingInteractor);
        return base.IsSelectableBy(interactor) && !isAlreadyGrabbed;
    }
    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.05f);

        if (!rightHandGrabbing || !leftHandGrabbing)
        {
            secondInteractor = null;
        }
    }
    public IEnumerator DelayColliders()
    {
        foreach (Collider collider in previousColliders)
        {
            collider.gameObject.SetActive(false);
        }
        foreach (Collider collider in previousColliders)
        {
            collider.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(1f);

        foreach (Collider collider in previousColliders)
        {
            collider.gameObject.SetActive(true);
        }
        foreach (Collider collider in previousColliders)
        {
            collider.gameObject.SetActive(true);
        }
    }
    public IEnumerator DelayKinematic(Rigidbody rb)
    {
        rb.isKinematic = true;
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(1f);
        rb.isKinematic = false;
    }
}         
