using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRig : MonoBehaviour
{
    public Rigidbody bodyRb;
    public ControllerInteractors leftController;
    public ControllerInteractors rightController;
    public Transform playerHead;

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
    private void Start()
    {
        StartCoroutine(StartDelay());
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float weightLeft = Mathf.Clamp(1 / (leftController.weight * 2), float.NegativeInfinity, 1);
        Vector3 leftTargetPosition = Vector3.Lerp(leftJoint.targetPosition, leftController.transform.localPosition, weightLeft);
        if (leftController.weight != 0)
        {
            leftTargetPosition.y -= 0.00025f * leftController.weight;
        }
        leftJoint.targetPosition = leftTargetPosition;

        Quaternion leftTargetRotation = Quaternion.Lerp(leftJoint.targetRotation, leftController.transform.localRotation, weightLeft);
        leftJoint.targetRotation = leftTargetRotation;

        float weightRight = Mathf.Clamp(1 / (rightController.weight * 2), float.NegativeInfinity, 1);
        Vector3 rightTargetPosition = Vector3.Lerp(rightJoint.targetPosition, rightController.transform.localPosition, weightRight);
        if(rightController.weight != 0)
        {
            rightTargetPosition.y -= 0.00025f * rightController.weight;
        }
        rightJoint.targetPosition = rightTargetPosition;

        Quaternion rightTargetRotation = Quaternion.Lerp(rightJoint.targetRotation, rightController.transform.localRotation, weightRight);
        rightJoint.targetRotation = rightTargetRotation;

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

    }
    public IEnumerator StartDelay()
    {
        bodyRb.isKinematic = true;
        yield return new WaitForSeconds(0.5f);
        bodyRb.isKinematic = false;
    }
}
