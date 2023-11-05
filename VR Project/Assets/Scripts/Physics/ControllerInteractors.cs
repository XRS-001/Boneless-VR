using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Collider forearmCollider;
    public HandData handRig;
    public Collider[] colliders;
    private List<Collider> interactableColliders;
    private Rigidbody rb;
    private Transform attach;
    public AudioClip grabAudio;
    private AudioSource audioSource;
    public GameObject handPresence;
    public GameObject handPhysics;
    private ConfigurableJoint configJoint;
    public bool isGrabbing;
    private GameObject objectGrabbing;
    private GameObject newColliderParent;
    protected override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        objectGrabbing = args.interactableObject.transform.gameObject;
        isGrabbing = true;
        interactableColliders = args.interactableObject.colliders;
        foreach (Collider collider in interactableColliders)
        {
            Physics.IgnoreCollision(collider, forearmCollider, true);
        }
        if (args.interactableObject is XRGrabInteractableTwoAttach
            || args.interactableObject is XRGrabInteractableMultiAttach
            || args.interactableObject is XRGrabInteractablePistol
            || args.interactableObject is XRGrabInteractableRifle
            || args.interactableObject is TwoHandInteractable
            || args.interactableObject is XRGrabDynamic
            || args.interactableObject is XRGrabInteractableShotgun)
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
                drive.positionDamper *= weight / 2;
                JointDrive driveHorizontal = handPhysics.GetComponent<ConfigurableJoint>().xDrive;
                driveHorizontal.positionDamper *= weight / 6;

                handPhysics.GetComponent<ConfigurableJoint>().xDrive = driveHorizontal;
                handPhysics.GetComponent<ConfigurableJoint>().yDrive = drive;
                handPhysics.GetComponent<ConfigurableJoint>().zDrive = driveHorizontal;
            }

            attach = args.interactableObject.transform.GetComponent<XRGrabInteractable>().attachTransform.transform;
            StartCoroutine(DelayEnter());
            configJoint = handPhysics.AddComponent<ConfigurableJoint>();
            configJoint.enableCollision = false;

            configJoint.xMotion = ConfigurableJointMotion.Locked;
            configJoint.yMotion = ConfigurableJointMotion.Locked;
            configJoint.zMotion = ConfigurableJointMotion.Locked;

            configJoint.angularXMotion = ConfigurableJointMotion.Locked;
            configJoint.angularYMotion = ConfigurableJointMotion.Locked;
            configJoint.angularZMotion = ConfigurableJointMotion.Locked;

            configJoint.connectedBody = rb;
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
            drive.positionDamper = 300;

            handPhysics.GetComponent<ConfigurableJoint>().xDrive = drive;
            handPhysics.GetComponent<ConfigurableJoint>().yDrive = drive;
            handPhysics.GetComponent<ConfigurableJoint>().zDrive = drive;
        }
        isGrabbing = false;
        weight = 0;
        StartCoroutine(DelayExit());
        Destroy(configJoint);
        handPhysics.GetComponent<Rigidbody>().mass = 1;
        foreach (Collider collider in args.interactableObject.colliders)
        {
            Physics.IgnoreCollision(collider, forearmCollider, false);
        }
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
        handPhysics.transform.position = attach.position;
        handPhysics.transform.rotation = attach.rotation;
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
        if(objectGrabbing.layer != 20)
        {
            foreach (Collider collider in interactableColliders)
            {
                collider.enabled = false;
            }
        }
        yield return new WaitForSeconds(0.1f);
        newColliderParent = Instantiate(colliders[0].transform.parent.transform.parent.gameObject, colliders[0].transform.parent.transform.parent.transform.position, colliders[0].transform.parent.transform.parent.transform.rotation);
        newColliderParent.transform.parent = objectGrabbing.transform;
        if (objectGrabbing)
        {
            if (objectGrabbing.layer != 20)
            {
                foreach (Collider collider in interactableColliders)
                {
                    collider.enabled = true;
                }
            }
        }
    }
    public IEnumerator DelayExit()
    {
        if (objectGrabbing)
        {
            Destroy(newColliderParent);
        }
        yield return new WaitForSeconds(1f);
        if(!isGrabbing)
        {
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }
        }
    }
}
