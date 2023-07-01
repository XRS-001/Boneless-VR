using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerInteractors : XRDirectInteractor
{
    public HandData handRig;
    public Animator physicsAnimator;
    public GameObject handPresence;
    private FixedJoint joint;
    public bool isGrabbing;
    // Start is called before the first frame update
    void Start()
    {
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        joint = handPresence.AddComponent<FixedJoint>();
        joint.enableCollision = false;
        joint.connectedBody = args.interactableObject.transform.GetComponent<Rigidbody>();
        base.OnSelectEntered(args);
        isGrabbing = true;
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        joint.connectedBody = null;
        Destroy(joint);
        base.OnSelectExited(args);
        isGrabbing = false;
    }
}
