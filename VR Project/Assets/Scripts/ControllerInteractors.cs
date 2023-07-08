using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerInteractors : XRDirectInteractor
{
    public HandData handRig;
    public GameObject colliders;
    private Rigidbody rb;
    private bool isSelecting;
    private Transform attach;
    public float neoMass;
    public GameObject handPresence;
    private ConfigurableJoint joint;
    public bool isGrabbing;
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        rb = args.interactableObject.transform.GetComponent<Rigidbody>();
        neoMass = rb.mass;
        rb.mass = 0.1f;
        attach = args.interactableObject.transform.GetComponent<XRGrabInteractable>().attachTransform.transform;
        colliders.SetActive(false);
        handPresence.transform.position = attach.position;
        handPresence.transform.rotation = attach.rotation;
        colliders.SetActive(true);
        joint = args.interactableObject.transform.AddComponent<ConfigurableJoint>();
        joint.enableCollision = false;
        joint.connectedBody = handPresence.GetComponent<Rigidbody>();

        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;

        joint.massScale = 0.0001f;
        joint.connectedMassScale = 0.0001f;

        isGrabbing = true;
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        rb.mass = neoMass;
        joint.connectedBody.useGravity = true;
        joint.connectedBody = null;
        Destroy(joint);
        isGrabbing = false;
    }
}
