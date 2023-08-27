using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandPresencePhysics : MonoBehaviour
{
    public Transform handRigModelTransform;
    private float maxDistance = 0.1f;
    public Transform target;
    public Collider[] handColliders;
    public ControllerInteractors controller;
    private float baseMoveSpeed = 1f;
    private Rigidbody rb;

    private float positionSmoothTime = 0.1f;
    private Vector3 targetPositionVelocity;

    public float weight = 0f;
    private float weightScale = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Collider collider in handColliders)
        {
            collider.enabled = false;
        }
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (transform.position.y > 0.2)
        {
            foreach(Collider collider in handColliders)
            {
                collider.enabled = true;
            }
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
    void Update()
    {
        if (controller.isGrabbing && Vector3.Distance(transform.position, handRigModelTransform.position) > maxDistance && weight > 5)
        {
            Vector3 direction = (transform.position - handRigModelTransform.position).normalized;
            Vector3 desiredPosition = handRigModelTransform.position + direction * maxDistance;
            Vector3 force = (desiredPosition - transform.position) * 100f;
            rb.AddForce(force);
        }
    }
}




