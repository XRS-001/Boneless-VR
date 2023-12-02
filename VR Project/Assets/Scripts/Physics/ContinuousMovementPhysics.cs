using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ContinuousMovementPhysics : MonoBehaviour
{
    public Rigidbody leftHand;
    private Vector3 previousLeftPosition;
    public Rigidbody rightHand;
    private Vector3 previousRightPosition;
    public ControllerInteractors leftController;
    public ControllerInteractors rightController;
    public float minSpeed = 1f;
    public float maxSpeed = 2.5f;
    private float speed = 0;
    public float turnSpeed = 60;
    private float jumpVelocity = 7;
    public float jumpHeight = 30f;
    public bool onlyMoveWhenGrounded = false;
    public InputActionProperty turnInputSource;
    public InputActionProperty moveInputSource;
    public InputActionProperty jumpInputSource;
    public Rigidbody rb;
    private bool isGrounded;
    public LayerMask groundLayer;
    public Transform directionSource;
    private Vector2 inputMoveAxis;
    private float inputTurnAxis;
    public Transform turnSource;
    private Vector3 direction;
    public CheckCollision leftHandCollision;
    public CheckCollision rightHandCollision;
    public CapsuleCollider[] leftFootDetection;
    public CapsuleCollider[] rightFootDetection;
    private bool isMoving = false;
    private Vector3 offset;
    private void Start()
    {
        previousLeftPosition = leftHand.position;
        previousRightPosition = rightHand.position;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 leftVelocity = Quaternion.Inverse(rb.rotation) * (leftHand.position - previousLeftPosition);
        float leftVelocityMagnitude = leftVelocity.magnitude / Time.deltaTime;
        previousLeftPosition = leftHand.position;
        if(leftVelocityMagnitude < 2)
        {
            leftVelocityMagnitude = 0;
        }

        Vector3 rightVelocity = Quaternion.Inverse(rb.rotation) * (rightHand.position - previousRightPosition);
        float rightVelocityMagnitude = rightVelocity.magnitude / Time.deltaTime;
        if (rightVelocityMagnitude < 2)
        {
            rightVelocityMagnitude = 0;
        }
        previousRightPosition = rightHand.position;

        speed = Mathf.Lerp(minSpeed, maxSpeed, (leftVelocityMagnitude + rightVelocityMagnitude) / 10);

        inputMoveAxis = moveInputSource.action.ReadValue<Vector2>();
        inputTurnAxis = turnInputSource.action.ReadValue<Vector2>().x;

        bool jumpInput = jumpInputSource.action.WasPressedThisFrame();

        if(jumpInput && isGrounded && !leftController.isClimbing && !rightController.isClimbing)
        {
            if(leftHandCollision.colliding || rightHandCollision.colliding)
            {
                jumpVelocity = Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight * 3);
                rb.velocity = directionSource.forward + (Vector3.up * jumpVelocity);
            }
            else if (isMoving)
            {
                jumpVelocity = Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight);
                rb.velocity = direction + (Vector3.up * jumpVelocity);
            }
            else
            {
                jumpVelocity = Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight);
                rb.velocity = Vector3.up * jumpVelocity;
            }
        }
    }
    private void FixedUpdate()
    {
        isGrounded = CheckIfGrounded();

        Quaternion yaw = Quaternion.Euler(0, 1 * directionSource.eulerAngles.y, 0);
        direction = yaw * new Vector3(inputMoveAxis.x, 0, inputMoveAxis.y);

        if (!onlyMoveWhenGrounded || (onlyMoveWhenGrounded && isGrounded))
        {
            Vector3 targetMovePosition = rb.position + direction * Time.fixedDeltaTime * speed;

            Vector3 axis = Vector3.up;
            float angle = turnSpeed * Time.fixedDeltaTime * inputTurnAxis;

            Quaternion q = Quaternion.AngleAxis(angle, axis);
            rb.MoveRotation(rb.rotation * q);

            Vector3 newPosition = q * (targetMovePosition - turnSource.position) + turnSource.position;

            rb.MovePosition(newPosition);
            isMoving = true;
            if (leftController.isGrabbing || rightController.isGrabbing)
            {
                if (leftController.objectGrabbing)
                {
                    if (leftController.objectGrabbing.GetComponent<XRGrabDoorHandle>())
                    {
                        offset = Quaternion.Euler(0f, 0f, leftController.objectGrabbing.transform.eulerAngles.z) * leftController.objectGrabbing.transform.right * -0.2f;

                        Vector3 direction = rb.position - (leftController.objectGrabbing.transform.position + offset - new Vector3(turnSource.localPosition.x, 0, turnSource.localPosition.z));

                        if (direction.magnitude > 1.125f)
                        {
                            direction = direction.normalized * 1.125f;
                            rb.position = leftController.objectGrabbing.transform.position + offset - new Vector3(turnSource.localPosition.x, 0, turnSource.localPosition.z) + direction;
                        }
                        isMoving = true;
                    }
                    else if (leftController.objectGrabbing.GetComponent<XRGrabKey>())
                    {
                        if (leftController.objectGrabbing.GetComponent<XRGrabKey>().isInLock)
                        {
                            offset = Quaternion.Euler(0f, 0f, leftController.objectGrabbing.transform.eulerAngles.z) * leftController.objectGrabbing.transform.right * 0f;

                            Vector3 direction = rb.position - (leftController.objectGrabbing.transform.position + offset - new Vector3(turnSource.localPosition.x, 0, turnSource.localPosition.z));

                            if (direction.magnitude > 1.2f)
                            {
                                direction = direction.normalized * 1.2f;
                                rb.position = leftController.objectGrabbing.transform.position + offset - new Vector3(turnSource.localPosition.x, 0, turnSource.localPosition.z) + direction;
                            }
                            isMoving = true;
                        }
                    }
                }
                else if (rightController.objectGrabbing)
                {
                    if (rightController.objectGrabbing.GetComponent<XRGrabDoorHandle>())
                    {
                        offset = Quaternion.Euler(0f, 0f, rightController.objectGrabbing.transform.eulerAngles.z) * rightController.objectGrabbing.transform.right * -0.2f;

                        Vector3 direction = rb.position - (rightController.objectGrabbing.transform.position + offset - new Vector3(turnSource.localPosition.x, 0, turnSource.localPosition.z));

                        if (direction.magnitude > 1.125f)
                        {
                            direction = direction.normalized * 1.125f;
                            rb.position = rightController.objectGrabbing.transform.position + offset - new Vector3(turnSource.localPosition.x, 0, turnSource.localPosition.z) + direction;
                        }
                        isMoving = true;
                    }
                    else if (rightController.objectGrabbing.GetComponent<XRGrabKey>())
                    {
                        if (rightController.objectGrabbing.GetComponent<XRGrabKey>().isInLock)
                        {
                            offset = Quaternion.Euler(0f, 0f, rightController.objectGrabbing.transform.eulerAngles.z) * rightController.objectGrabbing.transform.right * 0f;

                            Vector3 direction = rb.position - (rightController.objectGrabbing.transform.position + offset - new Vector3(turnSource.localPosition.x, 0, turnSource.localPosition.z));

                            if (direction.magnitude > 1.2f)
                            {
                                direction = direction.normalized * 1.2f;
                                rb.position = rightController.objectGrabbing.transform.position + offset - new Vector3(turnSource.localPosition.x, 0, turnSource.localPosition.z) + direction;
                            }
                            isMoving = true;
                        }
                    }
                }
            }
        }
        else
        {
            isMoving = false;
        }
    }
    public bool CheckIfGrounded()
    {
        bool hasHit = false;
        foreach(CapsuleCollider capsuleLeft in leftFootDetection)
        {
            Vector3 start = capsuleLeft.transform.TransformPoint(capsuleLeft.center);
            float rayLength = capsuleLeft.height / 2 - capsuleLeft.radius + 0.4f;

            if (Physics.SphereCast(start, capsuleLeft.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer))
            {
                hasHit = true;
            }
        }
        foreach (CapsuleCollider capsuleRight in rightFootDetection)
        {
            Vector3 start = capsuleRight.transform.TransformPoint(capsuleRight.center);
            float rayLength = capsuleRight.height / 2 - capsuleRight.radius + 0.4f;

            if (Physics.SphereCast(start, capsuleRight.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer))
            {
                hasHit = true;
            }
        }
        if (leftHandCollision.colliding)
        {
            hasHit = true;
        }
        else if (rightHandCollision.colliding)
        {
            hasHit = true;
        }
        return hasHit;
    }
}
