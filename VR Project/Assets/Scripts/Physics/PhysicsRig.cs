using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRig : MonoBehaviour
{
    public Rigidbody bodyRb;
    public Transform leftController;
    public Transform rightController;
    public Transform leftPresence;
    public Transform rightPresence;
    public Transform playerHead;
    public CapsuleCollider bodyCollider;

    public ConfigurableJoint rightJoint;
    public Collider[] rightJointColliders;

    public ConfigurableJoint leftJoint;
    public Collider[] leftJointColliders;

    public ConfigurableJoint headJoint;
    public Transform headTarget;

    public ConfigurableJoint chestJoint;
    public Transform chestTarget;

    public ConfigurableJoint rightArmJoint;
    public Transform rightArmTarget;

    public ConfigurableJoint rightForearmJoint;
    public Transform rightForearmTarget;

    public ConfigurableJoint leftArmJoint;
    public Transform leftArmTarget;

    public ConfigurableJoint leftForearmJoint;
    public Transform leftForearmTarget;

    public ConfigurableJoint rightThighJoint;
    public Transform rightThighTarget;

    public ConfigurableJoint rightLegJoint;
    public Transform rightLegTarget;

    public ConfigurableJoint leftThighJoint;
    public Transform leftThighTarget;

    public ConfigurableJoint leftLegJoint;
    public Transform leftLegTarget;

    public float bodyHeightMin = 0.5f;
    public float bodyHeightMax = 2;
    public float distanceLeft;
    public float distanceRight;
    public float threshold = 1f;
    private void Start()
    {
        StartCoroutine(StartDelay());
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        bodyCollider.height = Mathf.Clamp(playerHead.localPosition.y, bodyHeightMin, bodyHeightMax);
        bodyCollider.center = new Vector3(playerHead.localPosition.x, bodyCollider.height / 2, playerHead.localPosition.z);

        leftJoint.targetPosition = leftController.localPosition;
        leftJoint.targetRotation = leftController.localRotation;

        rightJoint.targetPosition = rightController.localPosition;
        rightJoint.targetRotation = rightController.localRotation;

        headJoint.targetPosition = headTarget.localPosition;
        headJoint.targetRotation = headTarget.localRotation;

        chestJoint.targetPosition = chestTarget.localPosition;
        chestJoint.targetRotation = chestTarget.localRotation;

        rightArmJoint.targetPosition = rightArmTarget.localPosition;
        rightArmJoint.targetRotation = rightArmTarget.localRotation;

        rightForearmJoint.targetPosition = rightForearmTarget.localPosition;
        rightForearmJoint.targetRotation = rightForearmTarget.localRotation;

        leftArmJoint.targetPosition = leftArmTarget.localPosition;
        leftArmJoint.targetRotation = leftArmTarget.localRotation;

        leftForearmJoint.targetPosition = leftForearmTarget.localPosition;
        leftForearmJoint.targetRotation = leftForearmTarget.localRotation;

        rightThighJoint.targetPosition = rightThighTarget.localPosition;
        rightThighJoint.targetRotation = rightThighTarget.localRotation;

        rightLegJoint.targetPosition = rightLegTarget.localPosition;
        rightLegJoint.targetRotation = rightLegTarget.localRotation;

        leftThighJoint.targetPosition = leftThighTarget.localPosition;
        leftThighJoint.targetRotation = leftThighTarget.localRotation;

        leftLegJoint.targetPosition = leftLegTarget.localPosition;
        leftLegJoint.targetRotation = leftLegTarget.localRotation;

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
    public IEnumerator StartDelay()
    {
        bodyRb.isKinematic = true;
        yield return new WaitForSeconds(0.5f);
        bodyRb.isKinematic = false;
    }
}
