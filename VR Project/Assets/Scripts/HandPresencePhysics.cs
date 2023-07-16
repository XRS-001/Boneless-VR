using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandPresencePhysics : MonoBehaviour
{
    public Transform target;
    public Collider[] handColliders;
    public bool isGrabbing;
    public ControllerInteractors controller;
    private float baseMoveSpeed = 1f;
    private Collider otherCollider;
    public GameObject colliderGroup;
    public GameObject colliderGroupJoint;
    private Rigidbody rb;

    private float positionSmoothTime = 0.1f;
    private Vector3 targetPositionVelocity;

    public float weight = 0f;
    private float weightScale = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        colliderGroup.SetActive(false);
        colliderGroupJoint.SetActive(false);
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Interactable"))
        {
            otherCollider = collision.gameObject.GetComponent<Collider>();
        }
    }

    public void EnableHandCollision()
    {
        if (otherCollider != null)
        {
            foreach (var collider in handColliders)
            {
                Physics.IgnoreCollision(otherCollider, collider, false);
            }
        }
    }

    public void DisableHandCollision()
    {
        if (otherCollider != null)
        {
            foreach (var collider in handColliders)
            {
                Physics.IgnoreCollision(otherCollider, collider, true);
            }
        }
    }

    void FixedUpdate()
    {
        if (transform.position.y > 0.2)
        {
            colliderGroup.SetActive(true);
            colliderGroupJoint.SetActive(true);
        }
        if(controller.isGrabbing)
        {
            weight = controller.neoMass;
            positionSmoothTime = 0.01f * weight;
            if(weight == 1)
            {
                weight = 0;
                positionSmoothTime = 0;
            }
        }
        else
        {
            weight = 0;
            positionSmoothTime = 0;
        }
        // Smooth out the target position using interpolation
        Vector3 smoothedTargetPosition = Vector3.SmoothDamp(transform.position, target.position, ref targetPositionVelocity, positionSmoothTime);

        // Calculate the desired velocity
        Vector3 velocity = (smoothedTargetPosition - transform.position) / Time.fixedDeltaTime;

        // Calculate the desired angular velocity
        Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);
        Vector3 rotationDifferenceInDegrees = angleInDegrees * rotationAxis;
        Vector3 angularVelocity = (rotationDifferenceInDegrees * Mathf.Deg2Rad / Time.fixedDeltaTime);

        // Adjust move speed based on weight, y-position, and direction of movement
        float currentMoveSpeed = baseMoveSpeed / (weight * weightScale);
        if (smoothedTargetPosition.y > transform.position.y)
        {
            currentMoveSpeed /= Mathf.Max(1f, transform.position.y);
        }

        // Apply interpolation to gradually move towards the desired velocity/angular velocity
        rb.velocity = Vector3.Lerp(rb.velocity, velocity, currentMoveSpeed * Time.fixedDeltaTime);
        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, angularVelocity, currentMoveSpeed * Time.fixedDeltaTime);
    }
}




