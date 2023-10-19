using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandPresencePhysics : MonoBehaviour
{
    public GameObject handColliderParent;
    public Transform target;
    public Collider[] handColliders;
    public ControllerInteractors controller;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Collider collider in handColliders)
        {
            collider.enabled = false;
        }
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (transform.position.y > 0.2)
        {
            foreach (Collider collider in handColliders)
            {
                collider.enabled = true;
            }
        }
        try
        {
            float gravityConstant = 0.981f; // You can adjust this constant as needed
            float weight = controller.weight;

            Vector3 gravityOffset = new Vector3(0, -weight * gravityConstant, 0);
            rb.velocity = (target.position - transform.position) / Time.fixedDeltaTime + gravityOffset;
        }
        catch { }

        Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        if (float.IsNaN(angleInDegree) || float.IsInfinity(angleInDegree))
        {
            // Handle the case where the angle is NaN or Infinity (e.g., set a default value)
            angleInDegree = 0f;
        }

        if (rotationAxis != Vector3.zero)
        {
            // Calculate the angular velocity
            Vector3 rotationDifferenceInDegree = angleInDegree * rotationAxis;

            if (!float.IsNaN(rotationDifferenceInDegree.x) && !float.IsInfinity(rotationDifferenceInDegree.x) &&
                !float.IsNaN(rotationDifferenceInDegree.y) && !float.IsInfinity(rotationDifferenceInDegree.y) &&
                !float.IsNaN(rotationDifferenceInDegree.z) && !float.IsInfinity(rotationDifferenceInDegree.z))
            {
                // All components of rotationDifferenceInDegree are valid, set the angular velocity
                rb.angularVelocity = rotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime;
            }
            else
            {
                // Handle the case where any component of rotationDifferenceInDegree is NaN or Infinity
                rb.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            // Handle the case where the rotation axis is zero (e.g., set a default angular velocity)
            rb.angularVelocity = Vector3.zero;
        }
    }
}




