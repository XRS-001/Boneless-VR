using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoHandInteractable : XRGrabInteractableTwoAttach
{
    public Collider secondHandGrabPointCollider;
    public Transform rightAttachSecond;
    public Transform leftAttachSecond;
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
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        secondHandGrabPoint.selectEntered.AddListener(OnSecondHandGrab);
        secondHandGrabPoint.selectExited.AddListener(OnSecondHandRelease);
    }

    // Update is called once per frame
    void Update()
    {
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
                secondInteractor.GetComponent<ControllerInteractors>().handPresence.transform.position = leftAttachSecond.position;
                secondInteractor.GetComponent<ControllerInteractors>().handPresence.transform.rotation = leftAttachSecond.rotation;
            }
            if (leftHandGrabbing)
            {
                secondInteractor.GetComponent<ControllerInteractors>().handPresence.transform.position = rightAttachSecond.position;
                secondInteractor.GetComponent<ControllerInteractors>().handPresence.transform.rotation = rightAttachSecond.rotation;
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
        trackPosition = true;
        trackRotation = true;
        Debug.Log("SECOND HAND GRAB");
        secondInteractor = args.interactorObject.transform.GetComponent<ControllerInteractors>();
        secondInteractor.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().handColliderParent.SetActive(false);
        secondInteractor.GetComponent<ControllerInteractors>().bodyRb.isKinematic = true;
        initialRotationOffset = Quaternion.Inverse(GetTwoHandRotation()) * interactor.attachTransform.rotation;
    }
    public void OnSecondHandRelease(SelectExitEventArgs args)
    {
        secondInteractor.GetComponent<ControllerInteractors>().bodyRb.isKinematic = false;
        secondHandGrabbing = false;
        trackPosition = false;
        trackRotation = false;
        secondInteractor.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().handColliderParent.SetActive(true);
        StartCoroutine(Delay());
        Debug.Log("SECOND HAND RELEASE");
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        interactor = selectingInteractor;
        secondHandGrabPointCollider.enabled = true;
        Debug.Log("FIRST HAND GRAB");
        base.OnSelectEntered(args);
        attachInitialRotation = args.interactorObject.transform.GetComponent<ControllerInteractors>().attachTransform.localRotation;
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        secondHandGrabPointCollider.enabled = false;
        Debug.Log("FIRST HAND RELEASE");
        base.OnSelectExited(args);
        if (rightHandGrabbing || leftHandGrabbing)
        {
            secondHandGrabPoint.enabled = false;
            secondHandGrabPoint.enabled = true;
            TwoHandInteractable interactable = GetComponent<TwoHandInteractable>();
            XRInteractionManager XRInteractionManager = interactionManager;
            interactor = secondInteractor;
            secondInteractor = interactor;
            try
            {
                if (rightHandGrabbing)
                {
                    GetComponent<TwoHandInteractable>().attachTransform = leftAttach;
                    GetComponent<TwoHandInteractable>().attachTransform = leftAttach;
                }
                else
                {
                    GetComponent<TwoHandInteractable>().attachTransform = rightAttach;
                    GetComponent<TwoHandInteractable>().attachTransform = rightAttach;
                }
                XRInteractionManager.SelectEnter(interactor, interactable);
                initialRotationOffset = Quaternion.Inverse(GetTwoHandRotation()) * interactor.transform.GetComponent<ControllerInteractors>().attachTransform.rotation;
            }
            catch { }
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
}         
