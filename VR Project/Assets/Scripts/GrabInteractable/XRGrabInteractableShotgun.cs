using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractableShotgun : XRGrabInteractable
{
    private ShotgunFire shotgunFire;
    public Transform rightAttach;
    public Transform leftAttach;
    public Transform rightAttachSecond;
    public Transform leftAttachSecond;
    public XRGrabJoint secondGrabPoint;
    public bool isGrabbing;
    public bool rightHandGrabbing;
    public bool leftHandGrabbing;
    private bool secondHandGrabbing;
    public enum TwoHandRotationType { None, First, Second };
    public TwoHandRotationType twoHandRotationType;
    public bool snapToSecondHand = true;
    private XRBaseInteractor interactor;
    private ControllerInteractors secondInteractor;
    private Quaternion initialRotationOffset;
    public bool offHandGrabbing = false;
    void Start()
    {
        shotgunFire = GetComponent<ShotgunFire>();
        secondGrabPoint.selectEntered.AddListener(OnSecondHandGrab);
        secondGrabPoint.selectExited.AddListener(OnSecondHandRelease);
        secondGrabPoint.enabled = false;
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
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if (secondInteractor && selectingInteractor)
        {
            if (snapToSecondHand)
                selectingInteractor.GetComponent<ControllerInteractors>().handPhysics.GetComponent<Rigidbody>().MoveRotation(GetTwoHandRotation());
            else
                selectingInteractor.GetComponent<ControllerInteractors>().handPhysics.GetComponent<Rigidbody>().MoveRotation(GetTwoHandRotation() * initialRotationOffset);
        }
        base.ProcessInteractable(updatePhase);
    }
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        if(!isGrabbing)
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
        if (!offHandGrabbing)
        {
            secondGrabPoint.enabled = true;
        }
        else
        {
            secondGrabPoint.enabled = false;
        }
        if (offHandGrabbing && !secondHandGrabbing)
        {
            secondGrabPoint.enabled = true;
            XRInteractionManager XRInteractionManager = interactionManager;
            XRInteractionManager.SelectEnter(interactor, secondGrabPoint);
            interactor = secondInteractor;
            interactor = selectingInteractor;
            offHandGrabbing = false;
        }
        isGrabbing = true;
        base.OnSelectEntering(args);
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("RightHand"))
        {
            rightHandGrabbing = true;
        }
        else
        {
            leftHandGrabbing = true;
        }
        isGrabbing = true;
        interactor = args.interactorObject.transform.GetComponent<ControllerInteractors>();
        base.OnSelectEntered(args);
        if (offHandGrabbing && secondHandGrabbing)
        {
            GameObject.Find("XR Origin").GetComponent<Rigidbody>().isKinematic = false;
        }
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (secondHandGrabbing)
        {
            GameObject.Find("XR Origin").GetComponent<Rigidbody>().isKinematic = true;
            offHandGrabbing = true;
            XRGrabInteractableShotgun interactable = GetComponent<XRGrabInteractableShotgun>();
            XRInteractionManager XRInteractionManager = interactionManager;
            interactor = secondInteractor;
            secondInteractor = interactor.GetComponent<ControllerInteractors>();
            if (secondInteractor.transform.CompareTag("LeftHand"))
            {
                attachTransform = leftAttachSecond;
            }
            if (secondInteractor.transform.CompareTag("RightHand"))
            {
                attachTransform = rightAttachSecond;
            }
            XRInteractionManager.SelectExit(interactor, secondGrabPoint);

            HandData leftHandRig = GetComponent<GrabHandPose>().leftHandPose;
            HandData rightHandRig = GetComponent<GrabHandPose>().rightHandPose;

            GetComponent<GrabHandPose>().leftHandPose = secondGrabPoint.GetComponent<GrabHandPose>().leftHandPose;
            GetComponent<GrabHandPose>().rightHandPose = secondGrabPoint.GetComponent<GrabHandPose>().rightHandPose;

            XRInteractionManager.SelectEnter(interactor, interactable);

            GetComponent<GrabHandPose>().leftHandPose = leftHandRig;
            GetComponent<GrabHandPose>().rightHandPose = rightHandRig;
            secondHandGrabbing = false;
            secondInteractor = null;
        }
        else
        {
            isGrabbing = false;
            secondInteractor = null;
            rightHandGrabbing = false;
            leftHandGrabbing = false;
        }
    }
    public void OnSecondHandGrab(SelectEnterEventArgs args)
    {
        shotgunFire.recoilSpeed /= 2;
        JointDrive zDrive = secondGrabPoint.GetComponent<ConfigurableJoint>().zDrive;
        zDrive.positionDamper = 5;
        secondGrabPoint.GetComponent<ConfigurableJoint>().zDrive = zDrive;
        secondHandGrabbing = true;
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
        if (!offHandGrabbing)
        {
            secondHandGrabbing = false;
        }
        shotgunFire.recoilSpeed *= 2;
        JointDrive zDrive = secondGrabPoint.GetComponent<ConfigurableJoint>().zDrive;
        zDrive.positionDamper = 10000;
        secondGrabPoint.GetComponent<ConfigurableJoint>().zDrive = zDrive;
        StartCoroutine(Delay());
        foreach (Collider collider in colliders)
        {
            Physics.IgnoreCollision(collider, secondInteractor.GetComponent<ControllerInteractors>().forearmCollider, false);
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
}
