using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ContinuousMovementPhysics : MonoBehaviour
{
    public float speed = 1;
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

    // Update is called once per frame
    void Update()
    {
        inputMoveAxis = moveInputSource.action.ReadValue<Vector2>();
        inputTurnAxis = turnInputSource.action.ReadValue<Vector2>().x;

        bool jumpInput = jumpInputSource.action.WasPressedThisFrame();

        if(jumpInput && isGrounded)
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
            float rayLength = capsuleLeft.height / 2 - capsuleLeft.radius + 0.1f;

            if (Physics.SphereCast(start, capsuleLeft.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer))
            {
                hasHit = true;
            }
        }
        foreach (CapsuleCollider capsuleRight in rightFootDetection)
        {
            Vector3 start = capsuleRight.transform.TransformPoint(capsuleRight.center);
            float rayLength = capsuleRight.height / 2 - capsuleRight.radius + 0.1f;

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
