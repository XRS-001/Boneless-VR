using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ChestLock : MonoBehaviour
{
    private AudioSource audioSource;
    public Rigidbody hingeRigidbody;
    public Animator lockAnimator;
    private bool isInLock;
    private bool canEnterLock = true;
    private ConfigurableJoint lockJoint;
    public string keyName;
    public Transform attachPoint;
    public Transform target;
    private float angleDifference;
    private Collider otherCollider;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isInLock)
        {
            angleDifference = Quaternion.Angle(target.rotation, lockJoint.connectedBody.rotation);
            if(angleDifference < 1)
            {
                audioSource.Play();
                isInLock = false;
                lockAnimator.Play("Unlock");
                StartCoroutine(Delay());
                GetComponent<Rigidbody>().isKinematic = false;
                otherCollider.isTrigger = false;
                Destroy(lockJoint);
                hingeRigidbody.isKinematic = false;
                hingeRigidbody.gameObject.GetComponent<XRGrabInteractableMultiAttach>().enabled = true;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key") && keyName == other.GetComponent<Key>().keyName && canEnterLock)
        {
            audioSource.Play();
            canEnterLock = false;
            otherCollider = other;
            isInLock = true;
            lockJoint = gameObject.AddComponent<ConfigurableJoint>();
            lockJoint.anchor = new Vector3(0,-0.075f, 0);
            lockJoint.axis = new Vector3(-3.15f,0,0);
            SoftJointLimit lowXlimit = lockJoint.lowAngularXLimit;
            lowXlimit.limit = 1;
            lockJoint.lowAngularXLimit = lowXlimit;

            SoftJointLimit highXlimit = lockJoint.highAngularXLimit;
            highXlimit.limit = 50;
            lockJoint.highAngularXLimit = highXlimit;

            lockJoint.xMotion = ConfigurableJointMotion.Locked;
            lockJoint.yMotion = ConfigurableJointMotion.Locked;
            lockJoint.zMotion = ConfigurableJointMotion.Locked;

            lockJoint.angularXMotion = ConfigurableJointMotion.Limited;
            lockJoint.angularYMotion = ConfigurableJointMotion.Locked;
            lockJoint.angularZMotion = ConfigurableJointMotion.Locked;

            other.transform.position = attachPoint.position;
            other.transform.rotation = attachPoint.rotation;
            lockJoint.connectedBody = other.attachedRigidbody;
            other.isTrigger = true;
            lockJoint.enableCollision = false;
        }
    }
    public IEnumerator Delay()
    {
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(0.1f);
        GetComponent<Collider>().enabled = true;
    }
}
