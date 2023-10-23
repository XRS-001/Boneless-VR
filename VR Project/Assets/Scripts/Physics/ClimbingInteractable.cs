using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClimbingInteractable : XRSimpleInteractable
{
    private GrabHandPose grabHandPose;
    private GrabHandPose duplicateGrabHandPose;
    private XRSimpleInteractable duplicateInteractable;
    private ControllerInteractors controller;
    private GameObject handPhysics;
    private HandPresencePhysics handPresence;
    private FixedJoint fixedJoint;
    public bool isGrabbing;
    private void Start()
    {
        grabHandPose = GetComponent<GrabHandPose>();
        duplicateGrabHandPose = GetComponentInChildren<GrabHandPose>();
        duplicateInteractable = GetComponentInChildren<XRSimpleInteractable>();
        duplicateInteractable.selectEntered.AddListener(OnSelectSecond);
        duplicateInteractable.selectExited.AddListener(OnSelectSecondExit);
        duplicateInteractable.selectEntered.AddListener(duplicateGrabHandPose.SetupPose);
        duplicateInteractable.selectExited.AddListener(duplicateGrabHandPose.UnSetPose);
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        grabHandPose.SetupPose(args);
        controller = args.interactorObject.transform.GetComponent<ControllerInteractors>();
        handPresence = controller.handPresence.GetComponent<HandPresencePhysics>();
        handPhysics = controller.handPhysics;
        isGrabbing = true;
        handPresence.target = null;
        handPresence.transform.position = handPhysics.transform.position;
        handPresence.transform.rotation = handPhysics.transform.rotation;
        handPresence.target = handPhysics.transform.transform;
        fixedJoint = handPhysics.AddComponent<FixedJoint>();
        fixedJoint.connectedAnchor = transform.position;
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        grabHandPose.UnSetPose(args);
        handPresence.target = handPhysics.transform;
        isGrabbing = false;
        Destroy(fixedJoint);
    }
    private void OnSelectSecond(SelectEnterEventArgs args)
    {
        controller = args.interactorObject.transform.GetComponent<ControllerInteractors>();
        handPresence = controller.handPresence.GetComponent<HandPresencePhysics>();
        handPhysics = controller.handPhysics;
        isGrabbing = true;
        handPresence.target = null;
        handPresence.transform.position = handPhysics.transform.position;
        handPresence.transform.rotation = handPhysics.transform.rotation;
        handPresence.GetComponent<Rigidbody>().isKinematic = true;
        fixedJoint = handPhysics.AddComponent<FixedJoint>();
        fixedJoint.connectedAnchor = transform.position;
    }
    private void OnSelectSecondExit(SelectExitEventArgs args)
    {
        handPresence.GetComponent<Rigidbody>().isKinematic = false;
        handPresence.target = handPhysics.transform;
        isGrabbing = false;
        Destroy(fixedJoint);
    }
    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        bool isAlreadyGrabbed = selectingInteractor && !interactor.Equals(selectingInteractor);
        return base.IsSelectableBy(interactor) && !isAlreadyGrabbed;
    }
}
