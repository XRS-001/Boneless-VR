using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClimbingInteractable : XRSimpleInteractable
{
    private GrabHandPose grabHandPose;
    private GrabHandPose duplicateGrabHandPose;
    private XRSimpleInteractable duplicateInteractable;
    private ControllerInteractors controller;
    private GameObject handPhysics;
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
        handPhysics = controller.handPhysics;
        if (controller.CompareTag("LeftHand"))
        {
            handPhysics.transform.position = GetComponent<Collider>().ClosestPoint(controller.handCenter.position) + (-controller.transform.right / 25);
        }
        else
        {
            handPhysics.transform.position = GetComponent<Collider>().ClosestPoint(controller.handCenter.position) + (controller.transform.right / 25);
        }
        foreach(Collider collider in controller.colliders)
        {
            collider.enabled = false;
        }
        isGrabbing = true;
        fixedJoint = handPhysics.AddComponent<FixedJoint>();
        fixedJoint.connectedAnchor = transform.position;
        controller.isClimbing = true;
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        foreach (Collider collider in controller.colliders)
        {
            collider.enabled = true;
        }
        controller = args.interactorObject.transform.GetComponent<ControllerInteractors>();
        controller.isClimbing = false;
        grabHandPose.UnSetPose(args);
        isGrabbing = false;
        Destroy(fixedJoint);
    }
    private void OnSelectSecond(SelectEnterEventArgs args)
    {
        controller = args.interactorObject.transform.GetComponent<ControllerInteractors>();
        foreach (Collider collider in controller.colliders)
        {
            collider.enabled = false;
        }
        handPhysics = controller.handPhysics;
        if (controller.CompareTag("LeftHand"))
        {
            handPhysics.transform.position = GetComponent<Collider>().ClosestPoint(controller.handCenter.position) + (-controller.transform.right / 25);
        }
        else
        {
            handPhysics.transform.position = GetComponent<Collider>().ClosestPoint(controller.handCenter.position) + (controller.transform.right / 25);
        }
        isGrabbing = true;
        fixedJoint = handPhysics.AddComponent<FixedJoint>();
        fixedJoint.connectedAnchor = transform.position;
        controller.isClimbing = true;
    }
    private void OnSelectSecondExit(SelectExitEventArgs args)
    {
        controller = args.interactorObject.transform.GetComponent<ControllerInteractors>();
        foreach (Collider collider in controller.colliders)
        {
            collider.enabled = true;
        }
        controller.isClimbing = false;
        isGrabbing = false;
        Destroy(fixedJoint);
    }
    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        bool isAlreadyGrabbed = selectingInteractor && !interactor.Equals(selectingInteractor);
        return base.IsSelectableBy(interactor) && !isAlreadyGrabbed;
    }
}
