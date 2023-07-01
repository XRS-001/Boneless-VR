using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandPresencePhysics : MonoBehaviour
{
    public Transform target;
    public GameObject colliderGroup;
    private Rigidbody rb;
    public Collider[] handColliders;
    public bool isColliding;
    private Collider otherCollider;
    public bool isGrabbing = false;
    public ControllerInteractors controllerInteractor;
    // Start is called before the first frame update
    void Start()
    {
        colliderGroup.SetActive(false);
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        isColliding = true;
        if (collision.gameObject.CompareTag("Interactable"))
        {
            otherCollider = collision.collider;
        }
        else
        {
            otherCollider = null;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }
    public void EnableHandCollider()
    {
        if(!isGrabbing)
        {
            foreach (var collider in handColliders)
            {
                Physics.IgnoreCollision(collider, otherCollider, false);
            }
        }
    }
    public void DisableHandCollider()
    {
        foreach (var collider in handColliders)
        {
            Physics.IgnoreCollision(collider, otherCollider, true);
        }
    }
    public void IsGrabbing()
    {
        isGrabbing = true;
    }
    public void IsNotGrabbing()
    {
        isGrabbing = false;
    }

    void FixedUpdate()
    {
        if(transform.position.y > 0.1)
        {
            colliderGroup.SetActive(true);
        }

        rb.velocity = (target.position - transform.position) / Time.fixedDeltaTime;
        Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        Vector3 rotationDifferenceInDegree = angleInDegree * rotationAxis;

        rb.angularVelocity = (rotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime);
    }
}
