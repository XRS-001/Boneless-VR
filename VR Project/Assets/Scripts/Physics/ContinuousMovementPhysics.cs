using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    public CapsuleCollider bodyCollider;
    private Vector2 inputMoveAxis;
    private float inputTurnAxis;
    public Transform turnSource;
    private Vector3 direction;
    public CheckCollision leftHandCollision;
    public CheckCollision rightHandCollision;

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
                jumpVelocity = Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight * 1.7f);
                rb.velocity = directionSource.forward + (Vector3.up * jumpVelocity);
            }
            else
            {
                jumpVelocity = Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight);
                rb.velocity = direction + (Vector3.up * jumpVelocity);
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
        }
    }

    public bool CheckIfGrounded()
    {
        Vector3 start = bodyCollider.transform.TransformPoint(bodyCollider.center);
        float rayLength = bodyCollider.height / 2 - bodyCollider.radius + 0.1f;

        bool hasHit = Physics.SphereCast(start, bodyCollider.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
        if(!hasHit)
        {
            hasHit = leftHandCollision.colliding;
        }
        if (!hasHit)
        {
            hasHit = rightHandCollision.colliding;
        }
        return hasHit;
    }
}
