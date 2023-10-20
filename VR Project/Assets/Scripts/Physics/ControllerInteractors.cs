using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerInteractors : XRDirectInteractor
{
    public float weight;
    public Rigidbody bodyRb;
    public Transform handTargetHandPresence;
    public Collider[] interactableColliders;
    public Collider forearmCollider;
    public HandData handRig;
    public GameObject[] colliders;
    private Rigidbody rb;
    private Transform attach;
    public GameObject handPresence;
    public GameObject handPhysics;
    private FixedJoint configJoint;
    public bool isGrabbing;
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        isGrabbing = true;
        interactableColliders = args.interactableObject.transform.GetComponents<Collider>();
        interactableColliders = args.interactableObject.transform.GetComponentsInParent<Collider>();
        interactableColliders = args.interactableObject.transform.GetComponentsInChildren<Collider>();
        foreach (Collider collider in interactableColliders)
        {
            Physics.IgnoreCollision(collider, forearmCollider, true);
        }
        if (args.interactableObject is XRGrabInteractableTwoAttach 
            || args.interactableObject is XRGrabInteractableMultiAttach
            || args.interactableObject is XRGrabInteractablePistol
            || args.interactableObject is XRGrabInteractableRifle
            || args.interactableObject is TwoHandInteractable)
        {
            rb = args.interactableObject.transform.GetComponent<Rigidbody>();
            weight = rb.mass;
            handPhysics.GetComponent<Rigidbody>().mass = rb.mass;
            handPhysics.GetComponent<Rigidbody>().drag = rb.mass * 3;
            if (weight > 5)
            {
                JointDrive slerp = handPhysics.GetComponent<ConfigurableJoint>().slerpDrive;
                slerp.positionSpring /= weight / 4;
                slerp.positionDamper *= weight / 4;
                handPhysics.GetComponent<ConfigurableJoint>().slerpDrive = slerp;
            }
            attach = args.interactableObject.transform.GetComponent<XRGrabInteractable>().attachTransform.transform;
            StartCoroutine(DelayEnter());
            configJoint = handPhysics.AddComponent<FixedJoint>();
            configJoint.connectedBody = rb;
            configJoint.autoConfigureConnectedAnchor = false;
            configJoint.connectedAnchor = rb.transform.InverseTransformPoint(handPhysics.transform.position);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        handPresence.transform.position = transform.position;
        handPresence.transform.rotation = transform.rotation;
        if (weight > 5)
        {
            JointDrive slerp = handPhysics.GetComponent<ConfigurableJoint>().slerpDrive;
            slerp.positionSpring = 10000;
            slerp.positionDamper = 300;
            handPhysics.GetComponent<ConfigurableJoint>().slerpDrive = slerp;
        }
        weight = 0;
        Destroy(configJoint);
        handPhysics.GetComponent<Rigidbody>().mass = 1;
        foreach (Collider collider in interactableColliders)
        {
            Physics.IgnoreCollision(collider, forearmCollider, false);
        }
        interactableColliders = null;
        StartCoroutine(DelayExit());
    }
    public void ReleaseInteractable()
    {
        if (selectTarget != null)
        {
            enabled = false;
            enabled = true;
        }
    }
    public IEnumerator DelayEnter()
    {
        foreach (Collider collider in handPresence.GetComponent<HandPresencePhysics>().handColliders)
        {
            collider.isTrigger = true;
        }
        handPhysics.transform.position = attach.position;
        handPhysics.transform.rotation = attach.rotation;
        handPresence.transform.position = attach.position;
        handPresence.transform.rotation = attach.rotation;
        yield return new WaitForSeconds(0f);
    }
    public IEnumerator DelayExit()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (Collider collider in handPresence.GetComponent<HandPresencePhysics>().handColliders)
        {
            collider.isTrigger = false;
        }
    }
}
