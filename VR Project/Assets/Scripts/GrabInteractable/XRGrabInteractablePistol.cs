using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractablePistol : XRGrabInteractable
{
    public XRGrabJoint slideGrab;
    private PistolFire pistolFire;
    public bool isGrabbing { get; private set; } = false;
    public bool rightHandGrabbing = false;
    public bool leftHandGrabbing = false;
    public Transform rightAttach;
    public Transform leftAttach;
    public Collider secondHandGrabPointCollider;
    public Transform rightAttachSecond;
    public Transform leftAttachSecond;
    public XRSimpleInteractable secondHandGrabPoint;
    private ControllerInteractors secondInteractor;
    private ControllerInteractors interactor;
    private bool secondHandGrabbing;
    // Start is called before the first frame update
    void Start()
    {
        slideGrab.enabled = false;
        pistolFire = GetComponent<PistolFire>();
        secondHandGrabPoint?.selectEntered.AddListener(OnSecondHandGrab);
        secondHandGrabPoint?.selectExited.AddListener(OnSecondHandRelease);
    }
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if (secondInteractor && selectingInteractor)
        {
            if (rightHandGrabbing)
            {
                secondInteractor.handPhysics.transform.position = leftAttachSecond.position;
                secondInteractor.handPhysics.transform.rotation = leftAttachSecond.rotation;
            }
            if (leftHandGrabbing)
            {
                secondInteractor.handPhysics.transform.position = rightAttachSecond.position;
                secondInteractor.handPhysics.transform.rotation = rightAttachSecond.rotation;
            }
        }
        base.ProcessInteractable(updatePhase);
    }
    public void OnSecondHandGrab(SelectEnterEventArgs args)
    {
        secondHandGrabbing = true;
        pistolFire.recoilSpeed /= 2;
        secondInteractor = args?.interactorObject.transform.GetComponent<ControllerInteractors>();
        secondInteractor?.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().handColliderParent.SetActive(false);
    }
    public void OnSecondHandRelease(SelectExitEventArgs args)
    {
        secondHandGrabbing = false;
        pistolFire.recoilSpeed *= 2;
        secondInteractor.GetComponent<ControllerInteractors>()?.handPresence.GetComponent<HandPresencePhysics>().handColliderParent.SetActive(true);
        secondInteractor = null;
    }
    public override bool IsSelectableBy(IXRSelectInteractor selectInteractor)
    {
        bool isAlreadyGrabbed = selectingInteractor && !selectInteractor.Equals(selectingInteractor);
        return base.IsSelectableBy(selectInteractor) && !isAlreadyGrabbed;
    }
    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f);

        secondInteractor = null;
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
        isGrabbing = true;
        base.OnSelectEntering(args);
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        slideGrab.enabled = true;
        interactor = args.interactorObject.transform.GetComponent<ControllerInteractors>();
        secondHandGrabPoint.enabled = true;
        rightAttachSecond.gameObject.SetActive(true);
        leftAttachSecond.gameObject.SetActive(true);
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
            XRGrabInteractablePistol interactable = GetComponent<XRGrabInteractablePistol>();
            XRInteractionManager XRInteractionManager = interactionManager;
            interactor = secondInteractor;
            secondInteractor = interactor;
            if (secondInteractor.transform.CompareTag("LeftHand"))
            {
                GetComponent<XRGrabInteractablePistol>().attachTransform = leftAttach;
            }
            if (secondInteractor.transform.CompareTag("RightHand"))
            {
                GetComponent<XRGrabInteractablePistol>().attachTransform = rightAttach;
            }
            XRInteractionManager.SelectExit(interactor, secondHandGrabPoint);
            XRInteractionManager.SelectEnter(interactor, interactable);
        }
        else
        {
            slideGrab.enabled = false;
            rightHandGrabbing = false;
            leftHandGrabbing = false;
        }
        if (!rightHandGrabbing && !leftHandGrabbing)
        {
            secondHandGrabPoint.enabled = false;
        }
    }
}
