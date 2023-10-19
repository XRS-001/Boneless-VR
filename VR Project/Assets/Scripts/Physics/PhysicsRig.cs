using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRig : MonoBehaviour
{
    public Transform playerHead;
    public Transform leftController;
    public Transform rightController;
    public Transform leftPresence;
    public Transform rightPresence;
    public CapsuleCollider bodyCollider;

    public ConfigurableJoint rightJoint;
    public Collider[] rightJointColliders;
    public ConfigurableJoint leftJoint;
    public Collider[] leftJointColliders;

    public float bodyHeightMin = 0.5f;
    public float bodyHeightMax = 2;
    public float distanceLeft;
    public float distanceRight;
    public float threshold = 1f;
    // Update is called once per frame
    void FixedUpdate()
    {
        bodyCollider.height = Mathf.Clamp(playerHead.localPosition.y, bodyHeightMin, bodyHeightMax);
        bodyCollider.center = new Vector3 (playerHead.localPosition.x, bodyCollider.height / 2, playerHead.localPosition.z);

        leftJoint.targetPosition = leftController.localPosition;
        leftJoint.targetRotation = leftController.localRotation;

        rightJoint.targetPosition = rightController.localPosition;
        rightJoint.targetRotation = rightController.localRotation;

        distanceLeft = Vector3.Distance(leftPresence.position, leftJoint.transform.position);
        distanceRight = Vector3.Distance(rightPresence.position, rightJoint.transform.position);
        if(distanceLeft > threshold)
        {
            foreach(Collider collider in leftJointColliders)
            {
                collider.isTrigger = true;
            }
            StartCoroutine(Delay(leftJointColliders));
        }
        if(distanceRight > threshold)
        {
            foreach (Collider collider in rightJointColliders)
            {
                collider.isTrigger = true;
            }
            StartCoroutine(Delay(rightJointColliders));
        }
    }
    public IEnumerator Delay(Collider[] colliders)
    {
        yield return new WaitForSeconds(1f);

        foreach (Collider collider in colliders)
        {
            collider.isTrigger = false;
        }
    }
}
