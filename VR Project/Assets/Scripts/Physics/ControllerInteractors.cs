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
    public AudioClip grabAudio;
    private AudioSource audioSource;
    public GameObject handPresence;
    public GameObject handPhysics;
    private FixedJoint configJoint;
    public bool isGrabbing;
    protected override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
    }
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
            if (weight > 5)
            {
                JointDrive slerp = handPhysics.GetComponent<ConfigurableJoint>().slerpDrive;
                slerp.positionSpring /= weight / 4;
                slerp.positionDamper *= weight / 4;
                handPhysics.GetComponent<ConfigurableJoint>().slerpDrive = slerp;
            }
            if (weight > 1)
            {
                JointDrive drive = handPhysics.GetComponent<ConfigurableJoint>().xDrive;
                drive.positionDamper *= weight / 4;

                handPhysics.GetComponent<ConfigurableJoint>().xDrive = drive;
                handPhysics.GetComponent<ConfigurableJoint>().yDrive = drive;
                handPhysics.GetComponent<ConfigurableJoint>().zDrive = drive;
            }

            attach = args.interactableObject.transform.GetComponent<XRGrabInteractable>().attachTransform.transform;
            StartCoroutine(DelayEnter());
            configJoint = handPhysics.AddComponent<FixedJoint>();
            configJoint.connectedBody = rb;
            configJoint.autoConfigureConnectedAnchor = false;
            configJoint.connectedAnchor = rb.transform.InverseTransformPoint(handPhysics.transform.position);
        }
        if (args.interactableObject is XRBaseInteractable)
        {
            audioSource.pitch = 1.3f;
            audioSource.PlayOneShot(grabAudio);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        handPresence.transform.position = transform.position;
        handPresence.transform.rotation = transform.rotation;
        if (weight > 5)
        {
            JointDrive slerp = handPhysics.GetComponent<ConfigurableJoint>().slerpDrive;
            slerp.positionSpring *= weight / 4;
            slerp.positionDamper /= weight / 4;
            handPhysics.GetComponent<ConfigurableJoint>().slerpDrive = slerp;
        }
        if (weight > 1)
        {
            JointDrive drive = handPhysics.GetComponent<ConfigurableJoint>().slerpDrive;
            drive.positionDamper /= weight / 4;

            handPhysics.GetComponent<ConfigurableJoint>().xDrive = drive;
            handPhysics.GetComponent<ConfigurableJoint>().yDrive = drive;
            handPhysics.GetComponent<ConfigurableJoint>().zDrive = drive;
        }
        isGrabbing = false;
        weight = 0;
        StartCoroutine(DelayExit());
        Destroy(configJoint);
        handPhysics.GetComponent<Rigidbody>().mass = 1;
        foreach (Collider collider in interactableColliders)
        {
            Physics.IgnoreCollision(collider, forearmCollider, false);
        }
        interactableColliders = null;
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
        handPresence.GetComponent<HandPresencePhysics>().handColliderParent.SetActive(false);
        handPhysics.transform.position = attach.position;
        handPhysics.transform.rotation = attach.rotation;
        yield return new WaitForSeconds(0f);
    }
    public IEnumerator DelayExit()
    {
        yield return new WaitForSeconds(1f);

        handPresence.GetComponent<HandPresencePhysics>().handColliderParent.SetActive(true);
    }
}
