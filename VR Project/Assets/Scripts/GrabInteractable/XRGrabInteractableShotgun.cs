using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractableShotgun : XRGrabInteractable
{
    private ShotgunFire shotgunFire;
    public Transform rightAttach;
    public Transform leftAttach;
    private Collider[] handColliders;
    private Collider[] previousHandColliders;
    public XRGrabJoint secondGrabPoint;
    public bool isGrabbing;
    public bool rightHandGrabbing;
    public bool leftHandGrabbing;
    private bool secondHandGrabbing;
    public enum TwoHandRotationType { None, First, Second };
    public TwoHandRotationType twoHandRotationType;
    public bool snapToSecondHand = true;
    private ControllerInteractors interactor;
    private ControllerInteractors secondInteractor;
    private Quaternion initialRotationOffset;
    void Start()
    {
        shotgunFire = GetComponent<ShotgunFire>();
        secondGrabPoint.selectEntered.AddListener(OnSecondHandGrab);
        secondGrabPoint.selectExited.AddListener(OnSecondHandRelease);
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
                selectingInteractor.GetComponent<ControllerInteractors>().handPhysics.transform.rotation = GetTwoHandRotation();
            else
                selectingInteractor.GetComponent<ControllerInteractors>().handPhysics.transform.rotation = GetTwoHandRotation() * initialRotationOffset;
        }
        base.ProcessInteractable(updatePhase);
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
        handColliders = interactor.colliders;
        foreach (Collider collider in handColliders)
        {
            collider.isTrigger = true;
        }
        base.OnSelectEntered(args);
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        previousHandColliders = handColliders;
        StartCoroutine(DelaySetActive());
        rightHandGrabbing = false;
        leftHandGrabbing = false;
        isGrabbing = false;
        interactor = null;
        base.OnSelectExited(args);
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
        shotgunFire.recoilSpeed *= 2;
        JointDrive zDrive = secondGrabPoint.GetComponent<ConfigurableJoint>().zDrive;
        zDrive.positionDamper = 100;
        secondGrabPoint.GetComponent<ConfigurableJoint>().zDrive = zDrive;
        secondInteractor.GetComponent<ControllerInteractors>().handPresence.GetComponent<HandPresencePhysics>().handColliderParent.SetActive(true);
        foreach (Collider collider in colliders)
        {
            Physics.IgnoreCollision(collider, secondInteractor.GetComponent<ControllerInteractors>().forearmCollider, false);
        }
        secondInteractor = null;
    }
    IEnumerator DelaySetActive()
    {
        yield return new WaitForSeconds(0.1f);

        if (!leftHandGrabbing && !rightHandGrabbing)
        {
            foreach (Collider collider in handColliders)
            {
                collider.isTrigger = false;
            }
        }
        else
        {
            foreach (Collider collider in previousHandColliders)
            {
                collider.isTrigger = false;
            }
        }
    }
}
