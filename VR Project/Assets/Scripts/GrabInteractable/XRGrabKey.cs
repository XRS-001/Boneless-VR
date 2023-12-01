using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabKey : XRGrabInteractableTwoAttach
{
    public FixedJoint joint { get; private set; }
    public bool isInLock = false;
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if(isInLock)
        {
            Rigidbody VRRig = GameObject.Find("XR Origin").GetComponent<Rigidbody>();
            VRRig.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = selectingInteractor.GetComponent<ControllerInteractors>().handPhysics.GetComponent<Rigidbody>();
        }
        base.OnSelectEntered(args);
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        DestroyJoint();
        base.OnSelectExited(args);
    }
    public void CreateJoint()
    {
        if (isGrabbing)
        {
            Rigidbody VRRig = GameObject.Find("XR Origin").GetComponent<Rigidbody>();
            VRRig.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = selectingInteractor.GetComponent<ControllerInteractors>().handPhysics.GetComponent<Rigidbody>();
        }

    }
    public void DestroyJoint()
    {
        Rigidbody VRRig = GameObject.Find("XR Origin").GetComponent<Rigidbody>();
        VRRig.constraints = RigidbodyConstraints.FreezeRotation;
        Destroy(joint);
    }
}
