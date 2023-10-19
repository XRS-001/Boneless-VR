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

    // Update is called once per frame
    void Update()
    {
        inputMoveAxis = moveInputSource.action.ReadValue<Vector2>();
        inputTurnAxis = turnInputSource.action.ReadValue<Vector2>().x;

        bool jumpInput = jumpInputSource.action.WasPressedThisFrame();

        if(jumpInput && isGrounded)
        {
            jumpVelocity = Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight);
            rb.velocity = Vector3.up * jumpVelocity;
        }
    }
    private void FixedUpdate()
    {
        isGrounded = CheckIfGrounded();
        if (!onlyMoveWhenGrounded || (onlyMoveWhenGrounded && isGrounded))
        {
            Quaternion yaw = Quaternion.Euler(0, 1 * directionSource.eulerAngles.y, 0);
            Vector3 direction = yaw * new Vector3(inputMoveAxis.x, 0, inputMoveAxis.y);
            Vector3 targetMovePosition = rb.position + direction * Time.fixedDeltaTime * speed;
            rb.MovePosition(targetMovePosition);
        }
    }

    public bool CheckIfGrounded()
    {
        Vector3 start = bodyCollider.transform.TransformPoint(bodyCollider.center);
        float rayLength = bodyCollider.height / 2 - bodyCollider.radius + 0.05f;

        bool hasHit = Physics.SphereCast(start, bodyCollider.radius, Vector3.down, out RaycastHit hitInfo, groundLayer);

        return hasHit;
    }
}
