using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractablePistol : XRGrabInteractable
{
    private PistolFire pistolFire;
    public bool isGrabbing { get; private set; } = false;
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
    private ControllerInteractors secondInteractor;
    // Start is called before the first frame update
    void Start()
    {
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
                secondInteractor.handPresence.transform.position = leftAttachSecond.position;
                secondInteractor.handPresence.transform.rotation = leftAttachSecond.rotation;
            }
            if (leftHandGrabbing)
            {
                secondInteractor.handPresence.transform.position = rightAttachSecond.position;
                secondInteractor.handPresence.transform.rotation = rightAttachSecond.rotation;
            }
        }
        base.ProcessInteractable(updatePhase);
    }
    public void OnSecondHandGrab(SelectEnterEventArgs args)
    {
        pistolFire.recoilSpeed /= 2;
        Debug.Log("SECOND HAND GRAB");
        secondInteractor = args?.interactorObject.transform.GetComponent<ControllerInteractors>();
        secondInteractor?.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().handColliderParent.SetActive(false);
    }
    public void OnSecondHandRelease(SelectExitEventArgs args)
    {
        pistolFire.recoilSpeed *= 2;
        secondInteractor.GetComponent<ControllerInteractors>()?.handPresence.GetComponent<HandPresencePhysics>().handColliderParent.SetActive(true);
        Debug.Log("SECOND HAND RELEASE");
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
        try
        {
            foreach (Collider collider in handColliders)
            {
                Physics.IgnoreCollision(collider, slideCollider, true);
            }
        }
        catch { }
        base.OnSelectEntering(args);
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
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
        rightAttachSecond.gameObject.SetActive(false);
        leftAttachSecond.gameObject.SetActive(false);
        secondHandGrabPoint.enabled = false;
        secondHandGrabPoint.enabled = true;
        Debug.Log("FIRST HAND RELEASE");
        StartCoroutine(Delay());
        rightHandGrabbing = false;
        leftHandGrabbing = false;
        isGrabbing = false;
        StartCoroutine(DelaySetActive());
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
