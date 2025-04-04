using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRig : MonoBehaviour
{
    public Rigidbody bodyRb;
    public ControllerInteractors leftController;
    public ControllerInteractors rightController;
    public Transform leftHand;
    public Transform rightHand;
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

    private float YspringStartRight;
    private float YspringStartLeft;

    private void Start()
    {
        YspringStartLeft = leftJoint.yDrive.positionSpring;
        YspringStartRight = rightJoint.yDrive.positionSpring;
        StartCoroutine(StartDelay());
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float weightLeft = Mathf.Clamp(1 / (leftController.weight * 2), float.NegativeInfinity, 1);
        Vector3 leftTargetPosition = leftController.transform.localPosition;
        leftTargetPosition.x = Mathf.Clamp(leftTargetPosition.x, leftHand.localPosition.x, leftHand.localPosition.x);
        leftTargetPosition.y = Mathf.Clamp(leftTargetPosition.y, leftHand.localPosition.y, leftHand.localPosition.y);
        leftTargetPosition.z = Mathf.Clamp(leftTargetPosition.y, leftHand.localPosition.z, leftHand.localPosition.z);
        if (leftController.weight != 0)
        {
            if (!leftController.objectGrabbing.CompareTag("Pierceable"))
            {
                Vector3 targetVelocity = leftJoint.targetVelocity;
                targetVelocity.y = -0.25f * leftController.weight;
                leftJoint.targetVelocity = targetVelocity;

                JointDrive newDrive = leftJoint.yDrive;
                newDrive.positionSpring = YspringStartLeft / (leftController.weight / 10);
                leftJoint.yDrive = newDrive;
            }
            else
            {
                Vector3 targetVelocity = leftJoint.targetVelocity;
                targetVelocity.y = -0.25f * leftController.weight / 75;
                leftJoint.targetVelocity = targetVelocity;

                JointDrive newDrive = leftJoint.yDrive;
                newDrive.positionSpring = YspringStartLeft / (leftController.weight / 10);
                leftJoint.yDrive = newDrive;
            }
        }
        else
        {
            Vector3 targetVelocity = leftJoint.targetVelocity;
            targetVelocity.y = 0;
            leftJoint.targetVelocity = targetVelocity;

            JointDrive newDrive = leftJoint.yDrive;
            newDrive.positionSpring = YspringStartLeft;
            leftJoint.yDrive = newDrive;
        }
        leftJoint.targetPosition = Vector3.Lerp(leftJoint.targetPosition, leftTargetPosition, weightLeft);


        if (leftController.isGrabbing)
        {
            Vector3 leftPosition = leftJoint.GetComponent<Rigidbody>().position;
            leftPosition.x = Mathf.Clamp(leftPosition.x, leftHand.position.x, leftHand.position.x);
            leftPosition.z = Mathf.Clamp(leftPosition.y, leftHand.position.z, leftHand.position.z);
            leftJoint.GetComponent<Rigidbody>().position = leftPosition;
        }

        Quaternion leftTargetRotation = Quaternion.Lerp(leftJoint.targetRotation, leftController.transform.localRotation, weightLeft);
        leftJoint.targetRotation = leftTargetRotation;

        float weightRight = Mathf.Clamp(1 / (rightController.weight * 2), float.NegativeInfinity, 1);
        Vector3 rightTargetPosition = rightController.transform.localPosition;
        rightTargetPosition.x = Mathf.Clamp(rightTargetPosition.x, rightHand.localPosition.x, rightHand.localPosition.x);
        rightTargetPosition.y = Mathf.Clamp(rightTargetPosition.y, rightHand.localPosition.y, rightHand.localPosition.y);
        rightTargetPosition.z = Mathf.Clamp(rightTargetPosition.y, rightHand.localPosition.z, rightHand.localPosition.z);
        if (rightController.weight != 0)
        {
            if (!rightController.objectGrabbing.CompareTag("Pierceable"))
            {
                Vector3 targetVelocity = rightJoint.targetVelocity;
                targetVelocity.y = -0.25f * rightController.weight;
                rightJoint.targetVelocity = targetVelocity;

                JointDrive newDrive = rightJoint.yDrive;
                newDrive.positionSpring = YspringStartRight / (rightController.weight / 10);
                rightJoint.yDrive = newDrive;
            }
            else
            {
                Vector3 targetVelocity = rightJoint.targetVelocity;
                targetVelocity.y = -0.25f * rightController.weight / 75;
                rightJoint.targetVelocity = targetVelocity;

                JointDrive newDrive = rightJoint.yDrive;
                newDrive.positionSpring = YspringStartRight / (rightController.weight / 10);
                rightJoint.yDrive = newDrive;
            }
        }
        else
        {
            Vector3 targetVelocity = rightJoint.targetVelocity;
            targetVelocity.y = 0;
            rightJoint.targetVelocity = targetVelocity;

            JointDrive newDrive = rightJoint.yDrive;
            newDrive.positionSpring = YspringStartRight;
            rightJoint.yDrive = newDrive;
        }
        rightJoint.targetPosition = Vector3.Lerp(rightJoint.targetPosition, rightTargetPosition, weightRight);

        if (rightController.isGrabbing)
        {
            Vector3 rightPosition = rightJoint.GetComponent<Rigidbody>().position;
            rightPosition.x = Mathf.Clamp(rightPosition.x, rightHand.position.x, rightHand.position.x);
            rightPosition.z = Mathf.Clamp(rightPosition.y, rightHand.position.z, rightHand.position.z);
            rightJoint.GetComponent<Rigidbody>().position = rightPosition;
        }

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
