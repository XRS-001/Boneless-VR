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
    private ConfigurableJoint joint;
    private ConfigurableJoint configJoint;
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
            attach = args.interactableObject.transform.GetComponent<XRGrabInteractable>().attachTransform.transform;
            StartCoroutine(DelayEnter());
            joint = handPresence.AddComponent<ConfigurableJoint>();
            joint.enableCollision = false;
            joint.connectedBody = rb;

            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;

            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;

            joint.massScale = 0.0001f;
            joint.connectedMassScale = 0.0001f;

            configJoint = handPhysics.AddComponent<ConfigurableJoint>();
            configJoint.xMotion = ConfigurableJointMotion.Locked;
            configJoint.yMotion = ConfigurableJointMotion.Locked;
            configJoint.zMotion = ConfigurableJointMotion.Locked;

            configJoint.angularXMotion = ConfigurableJointMotion.Locked;
            configJoint.angularYMotion = ConfigurableJointMotion.Locked;
            configJoint.angularZMotion = ConfigurableJointMotion.Locked;
            configJoint.connectedBody = rb;
            configJoint.autoConfigureConnectedAnchor = false;
            configJoint.connectedAnchor = rb.transform.InverseTransformPoint(handPhysics.transform.position);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        weight = 0;
        handPresence.transform.position = transform.position;
        handPresence.transform.rotation = transform.rotation;
        Destroy(configJoint);
        handPhysics.GetComponent<Rigidbody>().mass = 1;
        handPresence.GetComponent<HandPresencePhysics>().handColliderParent.SetActive(false);
        foreach (Collider collider in interactableColliders)
        {
            Physics.IgnoreCollision(collider, forearmCollider, false);
        }
        interactableColliders = null;
        if (args.interactableObject is XRGrabInteractableTwoAttach 
            || args.interactableObject is XRGrabInteractableMultiAttach
            || args.interactableObject is XRGrabInteractablePistol
            || args.interactableObject is XRGrabInteractableRifle
            || args.interactableObject is TwoHandInteractable)
        {
            DestroyJoint();
        }
        StartCoroutine(DelayExit());
    }
    public void DestroyJoint()
    {
        joint.connectedBody.useGravity = true;
        joint.connectedBody = null;
        Destroy(joint);
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
        yield return new WaitForSeconds(1f);

        foreach (Collider collider in handPresence.GetComponent<HandPresencePhysics>().handColliders)
        {
            collider.isTrigger = false;
        }
    }
    public IEnumerator DelayExit()
    {
        yield return new WaitForSeconds(0.5f);

        handPresence.GetComponent<HandPresencePhysics>().handColliderParent.SetActive(true);
    }
}
